using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Domain.Entities;
using MediatR;

namespace MaleFashion.Application.Features.Cart.Commands.AddToCart;

public class AddToCartCommandHandler : IRequestHandler<AddToCartCommand, Unit>
{
    private readonly IApplicationUnitOfWork _unitOfWork;

    public AddToCartCommandHandler(IApplicationUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
    
        var product = await _unitOfWork.Products.GetByIdWithInventoryAsync(request.ProductId)
            ?? throw new NotFoundException(nameof(Domain.Entities.Product), request.ProductId);

        if (!product.IsActive)
        {
            throw new ConflictException("This product is not currently available.");
        }

        var cart = await _unitOfWork.Carts.GetByOwnerAsync(request.UserId, request.GuestId);
        if (cart == null)
        {
            cart = new Domain.Entities.Cart { UserId = request.UserId, GuestId = request.GuestId };
            await _unitOfWork.Carts.AddAsync(cart);
        }

        var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == request.ProductId);
        var requestedTotalQuantity = (existingItem?.Quantity ?? 0) + request.Quantity;

        var availableQuantity = product.Inventory?.AvailableQuantity ?? 0;
        if (requestedTotalQuantity > availableQuantity)
        {
            throw new ConflictException(
                $"Only {availableQuantity} unit(s) of \"{product.Name}\" are available.");
        }

        var unitPrice = product.DiscountPrice ?? product.Price;

        if (existingItem != null)
        {
            existingItem.Quantity = requestedTotalQuantity;
            existingItem.UnitPrice = unitPrice; // refresh snapshot to current price on every add
        }
        else
        {
            cart.CartItems.Add(new CartItem
            {
                ProductId = product.Id,
                Product = product,
                Quantity = request.Quantity,
                UnitPrice = unitPrice
            });
        }

        await _unitOfWork.SaveChangesAsync();
        return Unit.Value;
    }
}