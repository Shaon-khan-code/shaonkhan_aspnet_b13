using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Common.Helpers;
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
        Address? existingAddress = null;

        if (request.ShippingAddressId > 0)
        {
            existingAddress = await _unitOfWork.Addresses.GetByIdAsync(request.ShippingAddressId)
                ?? throw new NotFoundException(nameof(Address), request.ShippingAddressId);

            var ownsAddress =
                (request.UserId != null && existingAddress.UserId == request.UserId) ||
                (request.GuestId.HasValue && existingAddress.GuestId == request.GuestId);

            if (!ownsAddress)
            {
                throw new NotFoundException(nameof(Address), request.ShippingAddressId);
            }
        }
        else if (request.NewShippingAddress == null)
        {
            throw new ConflictException("A shipping address is required.");
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

        var payment = await TakePaymentAsync(request, totalAmount, cancellationToken);

        // শুধু Card এ সাথে সাথে পেমেন্ট নেওয়া হয় (simulated)।
        // bKash/Nagad এ শুধু TrxID জমা নেওয়া হয়, Admin যাচাই না করা পর্যন্ত Pending থাকে।
        var paidNow = request.PaymentMethod == PaymentMethod.Card;
        var isWallet = request.PaymentMethod is PaymentMethod.Bkash or PaymentMethod.Nagad;

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            foreach (var item in cart.CartItems)
            {
                var available = item.Product.Inventory?.AvailableQuantity ?? 0;
                if (item.Quantity > available)
                {
                    throw new ConflictException(
                        $"\"{item.Product.Name}\" only has {available} unit(s) available. Please update your cart.");
                }
            }

            var address = existingAddress;
            if (address == null)
            {
                var input = request.NewShippingAddress!;
                address = new Address
                {
                    UserId = request.UserId,
                    GuestId = request.GuestId,
                    FullName = input.FullName.Trim(),
                    Phone = input.Phone.Trim(),
                    Line1 = input.Line1.Trim(),
                    Line2 = string.IsNullOrWhiteSpace(input.Line2) ? null : input.Line2.Trim(),
                    City = input.City.Trim(),
                    State = string.IsNullOrWhiteSpace(input.State) ? null : input.State.Trim(),
                    PostalCode = input.PostalCode.Trim(),
                    Country = input.Country.Trim()
                };

                await _unitOfWork.Addresses.AddAsync(address);
            }

            var order = new Order
            {
                OrderNumber = GenerateOrderNumber(),
                UserId = request.UserId,
                GuestId = request.GuestId,
                Status = paidNow ? OrderStatus.Processing : OrderStatus.Pending,
                PaymentStatus = paidNow ? PaymentStatus.Paid : PaymentStatus.Pending,
                PaymentMethod = request.PaymentMethod,
                PaymentReference = payment?.TransactionId,
                PaymentAccount = isWallet ? MobileWalletValidator.Normalize(request.WalletNumber) : null,
                TotalAmount = totalAmount,
                ShippingAddressId = existingAddress?.Id ?? 0,
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

    private async Task<PaymentResult?> TakePaymentAsync(
        PlaceOrderCommand request, decimal totalAmount, CancellationToken cancellationToken)
    {
        switch (request.PaymentMethod)
        {
            case PaymentMethod.CashOnDelivery:
                return null;

            case PaymentMethod.Bkash:
            case PaymentMethod.Nagad:
                {
                    var error = MobileWalletValidator.Validate(request.WalletNumber, request.TransactionId);
                    if (error != null)
                    {
                        throw new PaymentFailedException(error);
                    }

                    var trxId = request.TransactionId!.Trim().ToUpperInvariant();

                    // একই TrxID দিয়ে দুইবার অর্ডার ঠেকানো
                    if (await _unitOfWork.Orders.PaymentReferenceExistsAsync(trxId))
                    {
                        throw new PaymentFailedException("This Transaction ID has already been used.");
                    }

                    return new PaymentResult
                    {
                        Success = true,
                        TransactionId = trxId
                    };
                }

            default:
                {
                    var result = await _paymentService.ChargeAsync(request.Card, totalAmount, cancellationToken);
                    if (!result.Success)
                    {
                        throw new PaymentFailedException(result.FailureReason ?? "Payment was declined.");
                    }

                    return result;
                }
        }
    }

    private static string GenerateOrderNumber()
    {
        return $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString("N")[..6].ToUpperInvariant()}";
    }
}