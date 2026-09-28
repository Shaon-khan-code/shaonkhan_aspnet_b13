using DomainInventory = MaleFashion.Domain.Entities.Inventory;

namespace MaleFashion.Domain.Entities;

public class Inventory : Common.BaseEntity
{
    public int ProductId { get; set; }
    public Product Product { get; set; } = null!;

    public int QuantityInStock { get; set; }
    public int ReservedQuantity { get; set; }
    public int ReorderLevel { get; set; }

    public DateTime LastUpdatedAt { get; set; }

    public int AvailableQuantity => QuantityInStock - ReservedQuantity;
    public bool IsLowStock => AvailableQuantity <= ReorderLevel;
}