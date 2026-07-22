using DevSkill.Shop.Domain.Entities;

namespace DevSkill.Shop.Web.Models
{
    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsActive { get; set; }
        public int CategoryId { get; set; }
        public string? Brand { get; set; }
        public string? Sku { get; set; }
        public string? Color { get; set; }
        public double? Weight { get; set; }

        public Category Category { get; set; } = null!;
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public Stock? Stock { get; set; }
    }
}
