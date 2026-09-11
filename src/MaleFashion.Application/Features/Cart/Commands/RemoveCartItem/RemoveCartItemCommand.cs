// RemoveCartItemCommand.cs
using MediatR;

namespace MaleFashion.Application.Features.Cart.Commands.RemoveCartItem;

public class RemoveCartItemCommand : IRequest<Unit>
{
    public string? UserId { get; set; }
    public Guid? GuestId { get; set; }
    public int CartItemId { get; set; }
}