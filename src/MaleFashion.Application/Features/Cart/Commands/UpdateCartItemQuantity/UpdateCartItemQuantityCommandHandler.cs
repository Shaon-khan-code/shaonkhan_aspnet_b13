// UpdateCartItemQuantityCommandHandler.cs
using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Domain.Entities;
using MediatR;

namespace MaleFashion.Application.Features.Cart.Commands.UpdateCartItemQuantity;

public class UpdateCartItemQuantityCommandHandler : IRequestHandler<UpdateCartItemQuantityCommand, Unit>
{
    private readonly IApplicationUnitOfWork _unitOfWork;

    public UpdateCartItemQuantityCommandHandler(IApplicationUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(UpdateCartItemQuantityCommand request, CancellationToken cancellationToken)
    {
        var cartItem = await _unitOfWork.Carts.GetCartItemAsync(request.CartItemId)
            ?? throw new NotFoundException(nameof(CartItem), request.CartItemId);

        var isOwner = cartItem.Cart.UserId == request.UserId ||
                      (cartItem.Cart.GuestId.HasValue && cartItem.Cart.GuestId == request.GuestId);


        if (!isOwner)
        {
            throw new NotFoundException(nameof(CartItem), request.CartItemId);
        }

        var availableQuantity = cartItem.Product.Inventory?.AvailableQuantity ?? 0;
        if (request.Quantity > availableQuantity)
        {
            throw new ConflictException(
                $"Only {availableQuantity} unit(s) of \"{cartItem.Product.Name}\" are available.");
        }

        cartItem.Quantity = request.Quantity;
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }


}