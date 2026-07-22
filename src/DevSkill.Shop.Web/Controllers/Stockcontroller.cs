using DevSkill.Shop.Application.DTOs;
using DevSkill.Shop.Application.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DevSkill.Shop.Web.Controllers
{
    public class StockController : Controller
    {
        private readonly IStockService _stockService;
        private readonly IProductService _productService;

        public StockController(IStockService stock, IProductService product)
        {
            _stockService = stock;
            _productService = product;
        }

        public async Task<IActionResult> Index(
            string? search, string? sortColumn, string? sortOrder, int page = 1, int pageSize = 10)
        {
            var result = await _stockService.GetPagedStocksAsync(page, pageSize, search, sortColumn, sortOrder);

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
            await LoadProductsAsync(null);
            return View(new StockCreateDto());
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StockCreateDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadProductsAsync(dto.ProductId);
                TempData["Error"] = "Please fill in all required fields.";
                return View(dto);
            }
            try
            {
                await _stockService.CreateAsync(dto);
                TempData["Success"] = "Stock entry created successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await LoadProductsAsync(dto.ProductId);
                TempData["Error"] = $"Error: {ex.Message}";
                return View(dto);
            }
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            var dto = await _stockService.GetByIdAsync(id);
            if (dto == null) { TempData["Error"] = "Stock entry not found."; return RedirectToAction(nameof(Index)); }
            await LoadProductsAsync(dto.ProductId);
            return View(dto);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(StockUpdateDto dto)
        {
            if (!ModelState.IsValid)
            {
                await LoadProductsAsync(dto.ProductId);
                TempData["Error"] = "Please fix validation errors.";
                return View(dto);
            }
            try
            {
                await _stockService.UpdateAsync(dto);
                TempData["Success"] = "Stock entry updated successfully.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await LoadProductsAsync(dto.ProductId);
                TempData["Error"] = $"Error: {ex.Message}";
                return View(dto);
            }
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _stockService.DeleteAsync(id);
                TempData["Success"] = "Stock entry deleted successfully.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error: {ex.Message}";
            }
            return RedirectToAction(nameof(Index));
        }

        private async Task LoadProductsAsync(Guid? selected)
        {
            var prods = await _productService.GetPagedProductsAsync(1, 1000, null, null, null);
            ViewBag.Products = new SelectList(prods.Data, "Id", "Name", selected);
        }
    }
}