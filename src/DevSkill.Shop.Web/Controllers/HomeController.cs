using DevSkill.Shop.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace DevSkill.Shop.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IProductService _productService;
        private readonly IStockService _stockService;

        public HomeController(ICategoryService cat, IProductService prod, IStockService stock)
        {
            _categoryService = cat;
            _productService = prod;
            _stockService = stock;
        }

        public async Task<IActionResult> Index()
        {
            var cats = await _categoryService.GetPagedCategoriesAsync(1, 1000, null, null, null);
            var prods = await _productService.GetPagedProductsAsync(1, 1000, null, null, null);
            var stocks = await _stockService.GetPagedStocksAsync(1, 1000, null, null, null);

            ViewBag.CategoryCount = cats.Total;
            ViewBag.ProductCount = prods.Total;
            ViewBag.StockCount = stocks.Total;
            ViewBag.LowStockCount = stocks.Data.Count(s => s.IsLowStock);
            return View();
        }
    }
}