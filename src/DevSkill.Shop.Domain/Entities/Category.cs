using System;
using System.Collections.Generic;
using System.Text;

namespace DevSkill.Shop.Domain.Entities
{
    public class Category : Entity<Guid>
    {
        public string Name { get; set; } = string.Empty;
        public bool IsActive { get; set; } = true;
        public string? ImageName { get; set; }

        public ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
