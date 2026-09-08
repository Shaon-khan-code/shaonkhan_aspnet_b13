using MaleFashion.Application.DTOs;
using MediatR;

namespace MaleFashion.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommand : IRequest<ProductDto>
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public bool IsActive { get; set; } = true;
    public int CategoryId { get; set; }
}