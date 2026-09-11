namespace MaleFashion.Application.DTOs;

public class InventoryDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSku { get; set; } = string.Empty;

    public int QuantityInStock { get; set; }
    public int ReservedQuantity { get; set; }
    public int AvailableQuantity { get; set; }
    public int ReorderLevel { get; set; }
    public bool IsLowStock { get; set; }

    public DateTime LastUpdatedAt { get; set; }
}