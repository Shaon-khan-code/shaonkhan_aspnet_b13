using MediatR;

namespace MaleFashion.Application.Features.Cart.Commands.UpdateCartItemQuantity;

public class UpdateCartItemQuantityCommand : IRequest<Unit>
{
    public string? UserId { get; set; }
    public Guid? GuestId { get; set; }
    public int CartItemId { get; set; }
    public int Quantity { get; set; }
}