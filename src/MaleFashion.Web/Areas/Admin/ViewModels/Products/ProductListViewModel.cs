using MaleFashion.Application.Common.Models;
using MaleFashion.Application.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MaleFashion.Web.Areas.Admin.ViewModels.Products;

public class ProductListViewModel
{
    public PagedResult<ProductDto> Products { get; set; } = new();

    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public bool? IsActive { get; set; }

    public List<SelectListItem> CategoryOptions { get; set; } = new();
}