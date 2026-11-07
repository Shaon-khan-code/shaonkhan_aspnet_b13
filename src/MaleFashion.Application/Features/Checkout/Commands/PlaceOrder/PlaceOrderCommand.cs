using MaleFashion.Application.Common.Models;
using MaleFashion.Application.DTOs;
using MaleFashion.Domain.Enums;
using MediatR;

namespace MaleFashion.Application.Features.Checkout.Commands.PlaceOrder;

public class PlaceOrderCommand : IRequest<OrderDto>
{
    public string? UserId { get; set; }
    public Guid? GuestId { get; set; }


    public int ShippingAddressId { get; set; }


    public ShippingAddressInput? NewShippingAddress { get; set; }


    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Card;

    public SimulatedCardDetails Card { get; set; } = new();


    public string? WalletNumber { get; set; }


    public string? TransactionId { get; set; }
}
