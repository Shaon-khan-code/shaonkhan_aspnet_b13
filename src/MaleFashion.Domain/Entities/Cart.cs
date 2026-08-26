using MaleFashion.Domain.Common;

namespace MaleFashion.Domain.Entities;

public class Cart : BaseEntity
{
    public string? UserId { get; set; }

    public ICollection<CartItem> CartItems { get; set; } = new List<CartItem>();
}