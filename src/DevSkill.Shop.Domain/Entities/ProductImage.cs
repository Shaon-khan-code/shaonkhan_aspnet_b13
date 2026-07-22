using System;
using System.Collections.Generic;
using System.Text;

namespace DevSkill.Shop.Domain.Entities
{
    public class ProductImage : Entity<Guid>
    {
        public Guid ProductId { get; set; }
        public int Serial { get; set; }
        public string? ImageName { get; set; }

        public Product Product { get; set; } = null!;
    }
}
