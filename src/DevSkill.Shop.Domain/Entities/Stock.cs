using System;
using System.Collections.Generic;
using System.Text;

namespace DevSkill.Shop.Domain.Entities
{
    public class Stock : Entity<Guid>
    {
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public int LowStockQuantity { get; set; }

        public Product Product { get; set; } = null!;
    }
}
