using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.Contracts.Services;
using MaleFashion.Application.DTOs;
using MaleFashion.Domain.Entities;
using MaleFashion.Domain.Enums;
using MapsterMapper;
using MediatR;

namespace MaleFashion.Application.Features.Checkout.Commands.PlaceOrder;

public class PlaceOrderCommandHandler : IRequestHandler<PlaceOrderCommand, OrderDto>
{
    private readonly IApplicationUnitOfWork _unitOfWork;
    private readonly ISimulatedPaymentService _paymentService;
    private readonly IMapper _mapper;

    public PlaceOrderCommandHandler(
        IApplicationUnitOfWork unitOfWork, ISimulatedPaymentService paymentService, IMapper mapper)
    {
        _unitOfWork = unitOfWork;
        _paymentService = paymentService;
        _mapper = mapper;
    }

    public async Task<OrderDto> Handle(PlaceOrderCommand request, CancellationToken cancellationToken)
    {
        var address = await _unitOfWork.Addresses.GetByIdAsync(request.ShippingAddressId)
            ?? throw new NotFoundException(nameof(Address), request.ShippingAddressId);

        // Ownership check (Security Fix #4): a customer/guest may only ship to
        // their own saved address, never one belonging to someone else.
        var ownsAddress =
            (request.UserId != null && address.UserId == request.UserId) ||
            (request.GuestId.HasValue && address.GuestId == request.GuestId);

        if (!ownsAddress)
        {
            throw new NotFoundException(nameof(Address), request.ShippingAddressId);
        }

        var cart = await _unitOfWork.Carts.GetByOwnerAsync(request.UserId, request.GuestId);
        if (cart == null || !cart.CartItems.Any())
        {
            throw new ConflictException("Your cart is empty.");
        }

        foreach (var item in cart.CartItems)
        {
            var available = item.Product.Inventory?.AvailableQuantity ?? 0;
            if (item.Quantity > available)
            {
                throw new ConflictException(
                    $"\"{item.Product.Name}\" only has {available} unit(s) available. Please update your cart.");
            }
        }

        var totalAmount = cart.CartItems.Sum(ci => ci.UnitPrice * ci.Quantity);

        var paymentResult = await _paymentService.ChargeAsync(request.Card, totalAmount, cancellationToken);
        if (!paymentResult.Success)
        {
            throw new PaymentFailedException(paymentResult.FailureReason ?? "Payment was declined.");
        }

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            foreach (var item in cart.CartItems)
            {
                var available = item.Product.Inventory?.AvailableQuantity ?? 0;
                if (item.Quantity > available)
                {
                    await _unitOfWork.RollbackTransactionAsync();
                    throw new ConflictException(
                        $"\"{item.Product.Name}\" only has {available} unit(s) available. Please update your cart.");
                }
            }

            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                UserId = request.UserId,
                GuestId = request.GuestId,
                Status = OrderStatus.Processing,
                PaymentStatus = PaymentStatus.Paid,
                TotalAmount = totalAmount,
                ShippingAddressId = address.Id,
                ShippingAddress = address,
                CreatedAt = DateTime.UtcNow,
                OrderItems = cart.CartItems.Select(ci => new OrderItem
                {
                    ProductId = ci.ProductId,
                    Product = ci.Product,
                    ProductName = ci.Product.Name,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.UnitPrice
                }).ToList()
            };

            await _unitOfWork.Orders.AddAsync(order);

            foreach (var item in cart.CartItems)
            {
                item.Product.Inventory!.QuantityInStock -= item.Quantity;
                item.Product.Inventory!.LastUpdatedAt = DateTime.UtcNow;
            }

            cart.CartItems.Clear();

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

            return _mapper.Map<OrderDto>(order);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
    }
}