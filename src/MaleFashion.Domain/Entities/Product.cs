using MaleFashion.Domain.Common;

namespace MaleFashion.Domain.Entities;

public class Product : AuditableEntity
{
    public string Name { get; set; } = string.Empty;
    public string Slug { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Sku { get; set; } = string.Empty;

    public decimal Price { get; set; }
    public decimal? DiscountPrice { get; set; }

    public bool IsActive { get; set; } = true;

    public string? ImageKey { get; set; }

    public int CategoryId { get; set; }
    public Category Category { get; set; } = null!;

    public Inventory? Inventory { get; set; }

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}