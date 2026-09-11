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
        // --- Outside the transaction: no DB writes yet ---

        var address = await _unitOfWork.Addresses.GetByIdAsync(request.ShippingAddressId)
            ?? throw new NotFoundException(nameof(Address), request.ShippingAddressId);

        var cart = await _unitOfWork.Carts.GetByOwnerAsync(request.UserId, request.GuestId);
        if (cart == null || !cart.CartItems.Any())
        {
            throw new ConflictException("Your cart is empty.");
        }

        // 1. Validate stock (pre-payment check — fail fast before charging the card).
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

        // Payment happens BEFORE the transaction starts and is NOT part of it —
        // it is an external call, not a database operation. If it fails, nothing
        // below runs and the database is untouched.
        var paymentResult = await _paymentService.ChargeAsync(request.Card, totalAmount, cancellationToken);
        if (!paymentResult.Success)
        {
            throw new PaymentFailedException(paymentResult.FailureReason ?? "Payment was declined.");
        }

        // --- Transactional boundary: steps 1 (re-check under transaction), 2-6 ---

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            // Re-validate stock again, now inside the transaction, immediately
            // before committing changes. Time has passed since the check above
            // (payment gateway round-trip) — another order could have consumed
            // the same stock in the meantime.
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

            // 2. Create Order + 3. Create OrderItems + 5. Set PaymentStatus
            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                UserId = request.UserId,
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

            // 4. Reduce Inventory
            foreach (var item in cart.CartItems)
            {
                item.Product.Inventory!.QuantityInStock -= item.Quantity;
                item.Product.Inventory!.LastUpdatedAt = DateTime.UtcNow;
            }

            cart.CartItems.Clear();

            // 6. Save everything safely — one atomic write inside the transaction.
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