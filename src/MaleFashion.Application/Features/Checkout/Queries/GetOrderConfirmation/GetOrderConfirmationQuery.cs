
using MaleFashion.Application.DTOs;
using MediatR;

namespace MaleFashion.Application.Features.Checkout.Queries.GetOrderConfirmation;

public class GetOrderConfirmationQuery : IRequest<OrderDto>
{
    public int OrderId { get; set; }
    public string? UserId { get; set; }
    public Guid? GuestId { get; set; }
}