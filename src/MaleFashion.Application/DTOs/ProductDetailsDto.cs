using System.Collections.Generic;

namespace MaleFashion.Application.DTOs;

public class ProductDetailsDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Sku { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }
    public string? ImageUrl { get; set; }

    public int CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;

    public bool IsInStock { get; set; }
    public int? AvailableQuantity { get; set; }

    public List<ProductDto> RelatedProducts { get; set; } = new();
}