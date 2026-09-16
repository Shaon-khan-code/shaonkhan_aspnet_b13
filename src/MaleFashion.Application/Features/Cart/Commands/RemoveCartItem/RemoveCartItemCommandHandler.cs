
using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Domain.Entities;
using MediatR;

namespace MaleFashion.Application.Features.Cart.Commands.RemoveCartItem;

public class RemoveCartItemCommandHandler : IRequestHandler<RemoveCartItemCommand, Unit>
{
    private readonly IApplicationUnitOfWork _unitOfWork;

    public RemoveCartItemCommandHandler(IApplicationUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        var cartItem = await _unitOfWork.Carts.GetCartItemAsync(request.CartItemId)
            ?? throw new NotFoundException(nameof(CartItem), request.CartItemId);

        var isOwner = (request.UserId != null && cartItem.Cart.UserId == request.UserId) ||
                      (request.GuestId.HasValue && cartItem.Cart.GuestId == request.GuestId);

        if (!isOwner)
        {
            throw new NotFoundException(nameof(CartItem), request.CartItemId);
        }

        cartItem.Cart.CartItems.Remove(cartItem);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}