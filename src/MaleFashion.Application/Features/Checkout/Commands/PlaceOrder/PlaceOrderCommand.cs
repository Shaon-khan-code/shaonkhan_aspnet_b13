using MaleFashion.Application.Common.Models;
using MaleFashion.Application.DTOs;
using MediatR;

namespace MaleFashion.Application.Features.Checkout.Commands.PlaceOrder;

public class PlaceOrderCommand : IRequest<OrderDto>
{
    public string? UserId { get; set; }
    public Guid? GuestId { get; set; }

    public int ShippingAddressId { get; set; }

    public SimulatedCardDetails Card { get; set; } = new();
}