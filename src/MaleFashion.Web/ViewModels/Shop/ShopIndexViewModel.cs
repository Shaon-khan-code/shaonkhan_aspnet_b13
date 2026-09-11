using MaleFashion.Application.Common.Models;
using MaleFashion.Application.DTOs;
using MaleFashion.Application.Features.Products.Queries.GetProductList;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MaleFashion.Web.ViewModels.Shop;

public class ShopIndexViewModel
{
    public PagedResult<ProductDto> Products { get; set; } = new();

    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public ProductSortOption SortBy { get; set; }

    public List<SelectListItem> CategoryOptions { get; set; } = new();
}