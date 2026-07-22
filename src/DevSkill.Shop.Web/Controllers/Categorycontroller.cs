using DevSkill.Shop.Application.DTOs;
using DevSkill.Shop.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevSkill.Shop.Web.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _service;
        public CategoryController(ICategoryService service) => _service = service;

        // GET /Category
        public async Task<IActionResult> Index(
            string? search, string? sortColumn, string? sortOrder, int page = 1, int pageSize = 10)
        {
            var result = await _service.GetPagedCategoriesAsync(page, pageSize, search, sortColumn, sortOrder);

            ViewBag.Search = search;
            ViewBag.SortColumn = sortColumn ?? "Id";
            ViewBag.SortOrder = sortOrder ?? "asc";
            ViewBag.Page = page;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalCount = result.TotalDisplay;
            ViewBag.TotalPages = (int)Math.Ceiling(result.TotalDisplay / (double)pageSize);

            return View(result.Data);
        }

        // GET /Category/Create
        public IActionResult Create() => View(new CategoryCreateDto());

        // POST /Category/Create
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fill in all required fields.";
                return View(dto);
            }
            try
            {
                await _service.CreateAsync(dto);
                TempData["Success"] = $"Category '{dto.Name}' created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return View(dto);
            }
        }

        // GET /Category/Edit/id
        public async Task<IActionResult> Edit(Guid id)
        {
            var dto = await _service.GetByIdAsync(id);
            if (dto == null) { TempData["Error"] = "Category not found."; return RedirectToAction(nameof(Index)); }
            return View(dto);
        }

        // POST /Category/Edit
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CategoryUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                TempData["Error"] = "Please fix validation errors.";
                return View(dto);
            }
            try
            {
                await _service.UpdateAsync(dto);
                TempData["Success"] = $"Category '{dto.Name}' updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
                return View(dto);
            }
        }

        // POST /Category/Delete
        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _service.DeleteAsync(id);
                TempData["Success"] = "Category deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}