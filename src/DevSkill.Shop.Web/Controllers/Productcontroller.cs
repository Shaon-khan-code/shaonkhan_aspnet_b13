using DevSkill.Shop.Application.DTOs;
using DevSkill.Shop.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevSkill.Shop.Web.Controllers
{
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;

        public ProductController(IProductService product, ICategoryService category)
        {
            _productService = product;
            _categoryService = category;
        }

        public async Task<IActionResult> Index(
            string? search, string? sortColumn, string? sortOrder, int page = 1, int pageSize = 10)
        {
            var result = await _productService.GetPagedProductsAsync(page, pageSize, search, sortColumn, sortOrder);

            ViewBag.Search = search;
            ViewBag.SortColumn = sortColumn ?? "Id";
            ViewBag.SortOrder = sortOrder ?? "asc";
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = result.TotalDisplay;
            ViewBag.TotalPages = (int)Math.Ceiling(result.TotalDisplay / (double)pageSize);

            return View(result.Data);
        }

        public async Task<IActionResult> Create()
        {
            await LoadCategoriesAsync(null);
            return View(new ProductCreateDto());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(dto.CategoryId);
                TempData["Error"] = "Please fill in all required fields.";
                return View(dto);
            }
            try
            {
                await _productService.CreateAsync(dto);
                TempData["Success"] = $"Product '{dto.Name}' created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await LoadCategoriesAsync(dto.CategoryId);
                TempData["Error"] = $"Error: {ex.Message}";
                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var dto = await _productService.GetByIdAsync(id);
            if (dto == null) { TempData["Error"] = "Product not found."; return RedirectToAction(nameof(Index)); }
            await LoadCategoriesAsync(dto.CategoryId);
            return View(dto);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadCategoriesAsync(dto.CategoryId);
                TempData["Error"] = "Please fix validation errors.";
                return View(dto);
            }
            try
            {
                await _productService.UpdateAsync(dto);
                TempData["Success"] = $"Product '{dto.Name}' updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await LoadCategoriesAsync(dto.CategoryId);
                TempData["Error"] = $"Error: {ex.Message}";
                return View(dto);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _productService.DeleteAsync(id);
                TempData["Success"] = "Product deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadCategoriesAsync(Guid? selected)
        {
            var cats = await _categoryService.GetAllActiveAsync();
            ViewBag.Categories = new SelectList(cats, "Id", "Name", selected);
        }
    }
}