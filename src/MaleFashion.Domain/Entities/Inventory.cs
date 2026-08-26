using System;
using MaleFashion.Domain.Common;
using System.Collections.Generic;
using System.Text;

namespace MaleFashion.Domain.Entities
{
    public class Inventory : BaseEntity
    {
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;

        public int QuantityInStock { get; set; }
        public int ReservedQuantity { get; set; }
    }
}
