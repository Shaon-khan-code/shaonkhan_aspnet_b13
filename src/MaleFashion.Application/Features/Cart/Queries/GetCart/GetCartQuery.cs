using MaleFashion.Application.DTOs;
using MediatR;

namespace MaleFashion.Application.Features.Cart.Queries.GetCart;

public class GetCartQuery : IRequest<CartDto>
{
    public string? UserId { get; set; }
    public Guid? GuestId { get; set; }
}