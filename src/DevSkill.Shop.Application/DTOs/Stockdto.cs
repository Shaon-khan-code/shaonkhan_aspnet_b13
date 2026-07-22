using System.ComponentModel.DataAnnotations;

namespace DevSkill.Shop.Application.DTOs
{
    public class StockCreateDto
    {
        [Required]
        public Guid ProductId { get; set; }
        [Range(0, int.MaxValue)]
        public int Quantity { get; set; }
        [Range(0, int.MaxValue)]
        public int LowStockQuantity { get; set; }
    }

    public class StockUpdateDto : StockCreateDto
    {
        public Guid Id { get; set; }
    }

    public class StockListDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string? ProductName { get; set; }
        public int Quantity { get; set; }
        public int LowStockQuantity { get; set; }
        public bool IsLowStock => Quantity <= LowStockQuantity;
    }
}