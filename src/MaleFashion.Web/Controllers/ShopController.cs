using MaleFashion.Application.Features.Categories.Queries.GetCategoryList;
using MaleFashion.Application.Features.Products.Queries.GetProductList;
using MaleFashion.Web.ViewModels.Shop;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MaleFashion.Web.Controllers;

public class ShopController : Controller
{
    private readonly IMediator _mediator;

    public ShopController(IMediator mediator)
    {
        _mediator = mediator;
    }

    public async Task<IActionResult> Index(
        string? searchTerm,
        int? categoryId,
        decimal? minPrice,
        decimal? maxPrice,
        ProductSortOption sortBy = ProductSortOption.NameAsc,
        int pageNumber = 1)
    {
        var result = await _mediator.Send(new GetProductListQuery
        {
            SearchTerm = searchTerm,
            CategoryId = categoryId,
            IsActive = true, // storefront must never show inactive products
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            SortBy = sortBy,
            PageNumber = pageNumber,
            PageSize = 12
        });

        var categories = await _mediator.Send(new GetCategoryListQuery());

        var model = new ShopIndexViewModel
        {
            Products = result,
            SearchTerm = searchTerm,
            CategoryId = categoryId,
            MinPrice = minPrice,
            MaxPrice = maxPrice,
            SortBy = sortBy,
            CategoryOptions = categories
                .Select(c => new SelectListItem { Value = c.Id.ToString(), Text = c.Name })
                .ToList()
        };

        return View(model);
    }
}