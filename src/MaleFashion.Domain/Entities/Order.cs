using MaleFashion.Domain.Common;
using MaleFashion.Domain.Enums;

namespace MaleFashion.Domain.Entities;

public class Order : AuditableEntity
{
    public string OrderNumber { get; set; } = string.Empty;
    public string? UserId { get; set; }
    public Guid? GuestId { get; set; }

    public OrderStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }

   
    public PaymentMethod PaymentMethod { get; set; }


    public string? PaymentReference { get; set; }


    public string? PaymentAccount { get; set; }

    public decimal TotalAmount { get; set; }

    public int ShippingAddressId { get; set; }
    public Address ShippingAddress { get; set; } = null!;

    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
