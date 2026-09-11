using MediatR;

namespace MaleFashion.Application.Features.Cart.Commands.AddToCart;

public class AddToCartCommand : IRequest<Unit>
{
    public string? UserId { get; set; }
    public Guid? GuestId { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
}