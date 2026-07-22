using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace DevSkill.Shop.Domain.Entities
{
    public class Product : Entity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsActive { get; set; } = true;
        public Guid CategoryId { get; set; }
        public string? Brand { get; set; }
        public string? Sku { get; set; }
        public string? Color { get; set; }
        public double? Weight { get; set; }

        public Category Category { get; set; } = null!;
        public ICollection<ProductImage> ProductImages { get; set; } = new List<ProductImage>();
        public Stock? Stock { get; set; }
    }
}
