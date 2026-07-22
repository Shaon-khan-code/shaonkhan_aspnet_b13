using System.ComponentModel.DataAnnotations;

namespace DevSkill.Shop.Application.DTOs
{
    public class ProductCreateDto
    {
        [Required, MaxLength(300)]
        public string Name { get; set; } = string.Empty;
        [Range(0, double.MaxValue)]
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
        [Required]
        public Guid CategoryId { get; set; }
        [MaxLength(200)]
        public string? Brand { get; set; }
        [MaxLength(100)]
        public string? Sku { get; set; }
        [MaxLength(100)]
        public string? Color { get; set; }
        public double? Weight { get; set; }
    }

    public class ProductUpdateDto : ProductCreateDto
    {
        public Guid Id { get; set; }
    }

    public class ProductListDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public string? CategoryName { get; set; }
        public string? Brand { get; set; }
        public string? Sku { get; set; }
        public string? Color { get; set; }
        public double? Weight { get; set; }
        public int? StockQuantity { get; set; }
    }
}