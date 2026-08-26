using MaleFashion.Domain.Enums;
using MaleFashion.Domain.Common;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace MaleFashion.Domain.Entities
{
    public class Order : AuditableEntity
    {
        public string OrderNumber { get; set; } = string.Empty;
        public string? UserId { get; set; }

        public OrderStatus Status { get; set; }
        public PaymentStatus PaymentStatus { get; set; }

        public decimal TotalAmount { get; set; }

        public int ShippingAddressId { get; set; }
        public Address ShippingAddress { get; set; } = null!;

        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
}
