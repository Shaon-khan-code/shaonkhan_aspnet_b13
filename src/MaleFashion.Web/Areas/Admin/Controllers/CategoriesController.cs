using FluentValidation;
using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Features.Categories.Commands.CreateCategory;
using MaleFashion.Application.Features.Categories.Commands.DeleteCategory;
using MaleFashion.Application.Features.Categories.Commands.UpdateCategory;
using MaleFashion.Application.Features.Categories.Queries.GetCategoryById;
using MaleFashion.Application.Features.Categories.Queries.GetCategoryList;
using MaleFashion.Domain.Constants;
using MaleFashion.Web.Areas.Admin.ViewModels.Categories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MaleFashion.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = RoleNames.Admin)]
public class CategoriesController : Controller
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index()
    {
        var categories = await _mediator.Send(new GetCategoryListQuery());
        return View(categories);
    }

    public async Task<IActionResult> Create()
    {
        var model = new CategoryFormViewModel
        {
            ParentCategoryOptions = await GetParentOptionsAsync(excludeId: null)
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormViewModel model)
    {
        if (!ModelState.IsValid)
        {
            model.ParentCategoryOptions = await GetParentOptionsAsync(excludeId: null);
            return View(model);
        }

        try
        {
            await _mediator.Send(new CreateCategoryCommand
            {
                Name = model.Name,
                Slug = model.Slug,
                Description = model.Description,
                ParentCategoryId = model.ParentCategoryId
            });

            TempData["Success"] = "Category created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            AddValidationErrors(ex);
            model.ParentCategoryOptions = await GetParentOptionsAsync(excludeId: null);
            return View(model);
        }
    }

    public async Task<IActionResult> Edit(int id)
    {
        CategoryDtoResult category;
        try
        {
            category = await _mediator.Send(new GetCategoryByIdQuery { Id = id });
        }
        catch (NotFoundException)
        {
            return NotFound();
        }

        var model = new CategoryFormViewModel
        {
            Id = category.Id,
            Name = category.Name,
            Slug = category.Slug,
            Description = category.Description,
            ParentCategoryId = category.ParentCategoryId,
            ParentCategoryOptions = await GetParentOptionsAsync(excludeId: id)
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryFormViewModel model)
    {
        if (id != model.Id)
        {
            return BadRequest();
        }

        if (!ModelState.IsValid)
        {
            model.ParentCategoryOptions = await GetParentOptionsAsync(excludeId: id);
            return View(model);
        }

        try
        {
            await _mediator.Send(new UpdateCategoryCommand
            {
                Id = model.Id,
                Name = model.Name,
                Slug = model.Slug,
                Description = model.Description,
                ParentCategoryId = model.ParentCategoryId
            });

            TempData["Success"] = "Category updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (ValidationException ex)
        {
            AddValidationErrors(ex);
            model.ParentCategoryOptions = await GetParentOptionsAsync(excludeId: id);
            return View(model);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var category = await _mediator.Send(new GetCategoryByIdQuery { Id = id });
            return View(category);
        }
        catch (NotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        try
        {
            await _mediator.Send(new DeleteCategoryCommand { Id = id });
            TempData["Success"] = "Category deleted successfully.";
        }
        catch (NotFoundException)
        {
            return NotFound();
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task<List<SelectListItem>> GetParentOptionsAsync(int? excludeId)
    {
        var categories = await _mediator.Send(new GetCategoryListQuery());

        return categories
            .Where(c => c.Id != excludeId)
            .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
            .ToList();
    }
}