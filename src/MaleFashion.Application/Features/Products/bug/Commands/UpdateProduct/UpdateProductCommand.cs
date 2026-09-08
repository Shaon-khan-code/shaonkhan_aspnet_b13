using MaleFashion.Application.DTOs;
using MaleFashion.Application.Features.Products.Commands.Common;
using MediatR;

namespace MaleFashion.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommand : IRequest<ProductDto>, IProductCommand
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public bool IsActive { get; set; }
    public int CategoryId { get; set; }

    public Stream? ImageStream { get; set; }
    public string? ImageFileName { get; set; }
    public string? ImageContentType { get; set; }
}