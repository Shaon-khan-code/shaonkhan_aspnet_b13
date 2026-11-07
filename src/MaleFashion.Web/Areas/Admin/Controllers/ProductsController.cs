using FluentValidation;
using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Features.Categories.Queries.GetCategoryList;
using MaleFashion.Application.Features.Products.Commands.CreateProduct;
using MaleFashion.Application.Features.Products.Commands.DeleteProduct;
using MaleFashion.Application.Features.Products.Commands.UpdateProduct;
using MaleFashion.Application.Features.Products.Queries.GetProductById;
using MaleFashion.Application.Features.Products.Queries.GetProductList;
using MaleFashion.Domain.Constants;
using MaleFashion.Web.Areas.Admin.ViewModels.Products;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MaleFashion.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class ProductsController : Controller
{
    private const string DuplicateMessage =
        "A product with the same Slug or SKU already exists. Please use different values.";

    private const string StorageMessage =
        "The image could not be uploaded to AWS S3. Check the AWS credentials, bucket name and region in the configuration.";

    private readonly IMediator _mediator;
    private readonly ILogger<ProductsController> _logger;

    public ProductsController(IMediator mediator, ILogger<ProductsController> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task<IActionResult> Index(
        string? searchTerm,
        int? categoryId,
        bool? isActive,
        int pageNumber = 1)
    {
        var result = await _mediator.Send(new GetProductListQuery
        {
            SearchTerm = searchTerm,
            CategoryId = categoryId,
            IsActive = isActive,
            PageNumber = pageNumber,
            PageSize = 10
        });

        var model = new ProductListViewModel
        {
            Products = result,
            SearchTerm = searchTerm,
            CategoryId = categoryId,
            IsActive = isActive,
            CategoryOptions = await GetCategoryOptionsAsync()
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        var model = new ProductFormViewModel
        {
            CategoryOptions = await GetCategoryOptionsAsync()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ProductFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateAsync(model);
            return View(model);
        }

        var image = ExtractImage(model.ImageFile);

        try
        {
            await _mediator.Send(new CreateProductCommand
            {
                Name = model.Name,
                Slug = model.Slug,
                Description = model.Description,
                Sku = model.Sku,
                Price = model.Price,
                DiscountPrice = model.DiscountPrice,
                IsActive = model.IsActive,
                CategoryId = model.CategoryId,
                InitialStock = model.InitialStock,
                ReorderLevel = model.ReorderLevel,

                ImageStream = image.Stream,
                ImageFileName = image.FileName,
                ImageContentType = image.ContentType
            });

            TempData["Success"] = "Product created successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            AddValidationErrors(ex);
        }
        catch (NotFoundException)
        {
            ModelState.AddModelError(
                nameof(model.CategoryId),
                "The selected category no longer exists.");
        }
        catch (Exception ex) when (IsStorageError(ex))
        {
            _logger.LogError(ex, "Product image upload to AWS S3 failed.");
            ModelState.AddModelError(nameof(model.ImageFile), StorageMessage);
        }
        catch (Exception ex) when (IsDuplicateKey(ex))
        {
            _logger.LogWarning(ex, "Duplicate product slug or SKU while creating a product.");
            ModelState.AddModelError(string.Empty, DuplicateMessage);
        }
        catch (InvalidOperationException ex)
        {
           
            _logger.LogWarning(ex, "Product image could not be uploaded.");
            ModelState.AddModelError(nameof(model.ImageFile), ex.Message);
        }
        finally
        {
            image.Stream?.Dispose();
        }

        await PopulateAsync(model);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        try
        {
            var product = await _mediator.Send(
                new GetProductByIdQuery { Id = id });

            var model = new ProductFormViewModel
            {
                Id = product.Id,
                Name = product.Name,
                Slug = product.Slug,
                Description = product.Description,
                Sku = product.Sku,
                Price = product.Price,
                DiscountPrice = product.DiscountPrice,
                IsActive = product.IsActive,
                CategoryId = product.CategoryId,
                CurrentImageUrl = product.ImageUrl,
                CategoryOptions = await GetCategoryOptionsAsync()
            };

            return View(model);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        ProductFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            await PopulateAsync(model);
            return View(model);
        }

        var image = ExtractImage(model.ImageFile);

        try
        {
            await _mediator.Send(new UpdateProductCommand
            {
                Id = model.Id,
                Name = model.Name,
                Slug = model.Slug,
                Description = model.Description,
                Sku = model.Sku,
                Price = model.Price,
                DiscountPrice = model.DiscountPrice,
                IsActive = model.IsActive,
                CategoryId = model.CategoryId,

                
                ImageStream = image.Stream,
                ImageFileName = image.FileName,
                ImageContentType = image.ContentType
            });

            TempData["Success"] = "Product updated successfully.";

            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            AddValidationErrors(ex);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (Exception ex) when (IsStorageError(ex))
        {
            _logger.LogError(ex, "Product image upload to AWS S3 failed.");
            ModelState.AddModelError(nameof(model.ImageFile), StorageMessage);
        }
        catch (Exception ex) when (IsDuplicateKey(ex))
        {
            _logger.LogWarning(ex, "Duplicate product slug or SKU while updating product {ProductId}.", model.Id);
            ModelState.AddModelError(string.Empty, DuplicateMessage);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "Product image could not be uploaded for product {ProductId}.", model.Id);
            ModelState.AddModelError(nameof(model.ImageFile), ex.Message);
        }
        finally
        {
            image.Stream?.Dispose();
        }

        await PopulateAsync(model);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var product = await _mediator.Send(
                new GetProductByIdQuery { Id = id });

            return View(product);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _mediator.Send(new DeleteProductCommand { Id = id });

            TempData["Success"] = "Product deleted successfully.";
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
        catch (ConflictException ex)
        {
            TempData["Error"] = ex.Message;
        }

        return RedirectToAction(nameof(Index));
    }

    private void AddValidationErrors(ValidationException ex)
    {
        foreach (var error in ex.Errors)
        {
            ModelState.AddModelError(
                error.PropertyName,
                error.ErrorMessage);
        }
    }

    private async Task PopulateAsync(ProductFormViewModel model)
    {
        model.CategoryOptions = await GetCategoryOptionsAsync();

        if (model.Id > 0 && string.IsNullOrEmpty(model.CurrentImageUrl))
        {
            try
            {
                var product = await _mediator.Send(
                    new GetProductByIdQuery { Id = model.Id });

                model.CurrentImageUrl = product.ImageUrl;
            }
            catch (NotFoundException)
            {
                
            }
        }
    }

    private async Task<List<SelectListItem>> GetCategoryOptionsAsync()
    {
        var categories = await _mediator.Send(
            new GetCategoryListQuery());

        return categories
            .Select(c => new SelectListItem
            {
                Value = c.Id.ToString(),
                Text = c.Name
            })
            .ToList();
    }

    
    private static bool IsStorageError(Exception ex)
    {
        for (var current = ex; current != null; current = current.InnerException!)
        {
            var ns = current.GetType().Namespace;
            if (ns != null && ns.StartsWith("Amazon", StringComparison.Ordinal))
            {
                return true;
            }

            if (current.InnerException == null)
            {
                break;
            }
        }

        return false;
    }


    private static bool IsDuplicateKey(Exception ex)
    {
        for (var current = ex; current != null; current = current.InnerException!)
        {
            if (current.Message.Contains("duplicate key", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            if (current.InnerException == null)
            {
                break;
            }
        }

        return false;
    }

    private static (
        Stream? Stream,
        string? FileName,
        string? ContentType
    ) ExtractImage(IFormFile? file)
    {
        if (file == null || file.Length == 0)
        {
            return (null, null, null);
        }

        return (
            file.OpenReadStream(),
            file.FileName,
            file.ContentType
        );
    }
}
