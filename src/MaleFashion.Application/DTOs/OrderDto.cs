using MaleFashion.Domain.Enums;

namespace MaleFashion.Application.DTOs;

public class OrderDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }

    public AddressDto ShippingAddress { get; set; } = new();
    public List<OrderItemDto> Items { get; set; } = new();
}