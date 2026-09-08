namespace MaleFashion.Application.Features.Products.Commands.Common;

public interface IProductCommand
{
    string Name { get; }
    string Slug { get; }
    string? Description { get; }
    string Sku { get; }
    decimal Price { get; }
    decimal? DiscountPrice { get; }
    bool IsActive { get; }
    int CategoryId { get; }

    Stream? ImageStream { get; }
    string? ImageFileName { get; }
    string? ImageContentType { get; }
}