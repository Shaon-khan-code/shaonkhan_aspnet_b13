using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.Contracts.Services;
using MaleFashion.Application.DTOs;
using MediatR;

namespace MaleFashion.Application.Features.Cart.Queries.GetCart;

public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto>
{
    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly IFileStorageService _fileStorageService;

    public GetCartQueryHandler(IApplicationUnitOfWork unitOfWork, IFileStorageService fileStorageService)
    {
        _unitOfWork = unitOfWork;
        _fileStorageService = fileStorageService;
    }

    public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
    {
        var cart = await _unitOfWork.Carts.GetByOwnerAsync(request.UserId, request.GuestId);

        if (cart == null)
        {
            return new CartDto();
        }

        return new CartDto
        {
            Id = cart.Id,
            Items = cart.CartItems.Select(ci => new CartItemDto
            {
                Id = ci.Id,
                ProductId = ci.ProductId,
                ProductName = ci.Product.Name,
                ImageUrl = ci.Product.ImageKey != null ? _fileStorageService.GetFileUrl(ci.Product.ImageKey) : null,
                UnitPrice = ci.UnitPrice,
                Quantity = ci.Quantity,
                AvailableQuantity = ci.Product.Inventory?.AvailableQuantity ?? 0
            }).ToList()
        };
    }
}