using MaleFashion.Domain.Enums;

namespace MaleFashion.Application.DTOs;

public class OrderListItemDto
{
    public int Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public string? CustomerName { get; set; }
    public OrderStatus Status { get; set; }
    public PaymentStatus PaymentStatus { get; set; }
    public decimal TotalAmount { get; set; }
    public DateTime CreatedAt { get; set; }
}