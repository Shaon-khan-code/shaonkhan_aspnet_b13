using MaleFashion.Application.Common.Models;
using MaleFashion.Application.DTOs;
using MediatR;

namespace MaleFashion.Application.Features.Products.Queries.GetProductList;

public class GetProductListQuery : IRequest<PagedResult<ProductDto>>
{
    public string? SearchTerm { get; set; }
    public int? CategoryId { get; set; }
    public bool? IsActive { get; set; }
    public decimal? MinPrice { get; set; }
    public decimal? MaxPrice { get; set; }
    public ProductSortOption SortBy { get; set; } = ProductSortOption.NameAsc;
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}