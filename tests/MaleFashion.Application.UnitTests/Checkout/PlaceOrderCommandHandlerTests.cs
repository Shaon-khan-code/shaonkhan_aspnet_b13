using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Common.Models;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.Contracts.Services;
using MaleFashion.Application.Features.Checkout.Commands.PlaceOrder;
using MaleFashion.Application.Tests.Common;
using MaleFashion.Domain.Entities;
using MaleFashion.Domain.Enums;
using Moq;
using Shouldly;
using System.Timers;
using Xunit;

namespace MaleFashion.Application.Tests.Checkout;

public class PlaceOrderCommandHandlerTests
{
    private readonly Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IAddressRepository> _addressRepositoryMock;
    private readonly Mock<ICartRepository> _cartRepositoryMock;
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<ISimulatedPaymentService> _paymentServiceMock;
    private readonly PlaceOrderCommandHandler _handler;

    public PlaceOrderCommandHandlerTests()
    {
        _unitOfWorkMock = TestUnitOfWorkFactory.CreateUnitOfWorkMock();
        _addressRepositoryMock = Mock.Get(_unitOfWorkMock.Object.Addresses);
        _cartRepositoryMock = Mock.Get(_unitOfWorkMock.Object.Carts);
        _orderRepositoryMock = Mock.Get(_unitOfWorkMock.Object.Orders);
        _paymentServiceMock = new Mock<ISimulatedPaymentService>();

        // FIX: Moq returns null (not Task.CompletedTask) for any Task-returning
        // method that has no explicit Setup. Awaiting that null Task throws
        // NullReferenceException. TestUnitOfWorkFactory doesn't configure these,
        // so every happy-path test that reaches the transaction/save calls was
        // failing. Giving them explicit, real completions here fixes that for
        // this test class regardless of what the shared factory does.
        _unitOfWorkMock.Setup(u => u.BeginTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.SaveChangesAsync()).ReturnsAsync(1);
        _unitOfWorkMock.Setup(u => u.CommitTransactionAsync()).Returns(Task.CompletedTask);
        _unitOfWorkMock.Setup(u => u.RollbackTransactionAsync()).Returns(Task.CompletedTask);
        _orderRepositoryMock.Setup(r => r.AddAsync(It.IsAny<Order>())).Returns(Task.CompletedTask);
        var mapper = TestUnitOfWorkFactory.CreateRealMapper();

        _handler = new PlaceOrderCommandHandler(_unitOfWorkMock.Object, _paymentServiceMock.Object, mapper);
    }

    // ---- Shared test data builders ----

    private static Address ValidAddress(string? userId = "user-1") => new()
    {
        Id = 10,
        UserId = userId,
        FullName = "Test Customer",
        Phone = "0123456789",
        Line1 = "123 Main St",
        City = "Dhaka",
        PostalCode = "1200",
        Country = "Bangladesh"
    };

    private static Product ProductWithStock(int id, decimal price, int quantityInStock, int reserved = 0) => new()
    {
        Id = id,
        Name = $"Product {id}",
        Sku = $"SKU-{id}",
        Price = price,
        CategoryId = 1,
        Category = new Category { Id = 1, Name = "Category", Slug = "category" },
        Inventory = new Inventory
        {
            ProductId = id,
            QuantityInStock = quantityInStock,
            ReservedQuantity = reserved
        }
    };

    private static Cart CartWithItems(params CartItem[] items) => new()
    {
        Id = 1,
        UserId = "user-1",
        CartItems = items.ToList()
    };

    private static CartItem LineItem(Product product, int quantity) => new()
    {
        Id = product.Id,
        ProductId = product.Id,
        Product = product,
        Quantity = quantity,
        UnitPrice = product.Price
    };

    private static PlaceOrderCommand ValidCommand(int addressId = 10) => new()
    {
        UserId = "user-1",
        ShippingAddressId = addressId,
        Card = new SimulatedCardDetails
        {
            CardholderName = "Test Customer",
            CardNumber = "4242424242424242",
            ExpiryMonth = "12",
            ExpiryYear = "2030",
            Cvv = "123"
        }
    };

    private void SetupSuccessfulPayment(decimal? expectedAmount = null)
    {
        _paymentServiceMock
            .Setup(p => p.ChargeAsync(
                It.IsAny<SimulatedCardDetails>(),
                expectedAmount.HasValue ? expectedAmount.Value : It.IsAny<decimal>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentResult { Success = true, TransactionId = "SIM-TEST-123" });
    }

    // ---- 1. Successful order ----

    [Fact]
    public async Task Handle_ValidOrder_ReturnsOrderDtoWithProcessingAndPaidStatus()
    {
        var address = ValidAddress();
        var product = ProductWithStock(id: 1, price: 50m, quantityInStock: 10);
        var cart = CartWithItems(LineItem(product, quantity: 2));

        _addressRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(address);
        _cartRepositoryMock.Setup(r => r.GetByOwnerAsync("user-1", null)).ReturnsAsync(cart);
        SetupSuccessfulPayment();

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.ShouldNotBeNull();
        result.Status.ShouldBe(OrderStatus.Processing);
        result.PaymentStatus.ShouldBe(PaymentStatus.Paid); // ---- 5. Payment status ----
        result.OrderNumber.ShouldStartWith("ORD-");
    }

    [Fact]
    public async Task Handle_ValidOrder_ClearsCartAfterSuccess()
    {
        var address = ValidAddress();
        var product = ProductWithStock(id: 1, price: 50m, quantityInStock: 10);
        var cart = CartWithItems(LineItem(product, quantity: 2));

        _addressRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(address);
        _cartRepositoryMock.Setup(r => r.GetByOwnerAsync("user-1", null)).ReturnsAsync(cart);
        SetupSuccessfulPayment();

        await _handler.Handle(ValidCommand(), CancellationToken.None);

        cart.CartItems.ShouldBeEmpty();
    }

    [Fact]
    public async Task Handle_ValidOrder_CommitsTransactionAndSavesOnce()
    {
        var address = ValidAddress();
        var product = ProductWithStock(id: 1, price: 50m, quantityInStock: 10);
        var cart = CartWithItems(LineItem(product, quantity: 2));

        _addressRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(address);
        _cartRepositoryMock.Setup(r => r.GetByOwnerAsync("user-1", null)).ReturnsAsync(cart);
        SetupSuccessfulPayment();

        await _handler.Handle(ValidCommand(), CancellationToken.None);

        _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.CommitTransactionAsync(), Times.Once);
        _unitOfWorkMock.Verify(u => u.RollbackTransactionAsync(), Times.Never);
        _orderRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Once);
    }

    // ---- 2. Insufficient stock ----

    [Fact]
    public async Task Handle_RequestedQuantityExceedsAvailableStock_ThrowsConflictExceptionBeforePayment()
    {
        var address = ValidAddress();
        var product = ProductWithStock(id: 1, price: 50m, quantityInStock: 1); // only 1 available
        var cart = CartWithItems(LineItem(product, quantity: 5)); // wants 5

        _addressRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(address);
        _cartRepositoryMock.Setup(r => r.GetByOwnerAsync("user-1", null)).ReturnsAsync(cart);

        var exception = await Should.ThrowAsync<ConflictException>(
            () => _handler.Handle(ValidCommand(), CancellationToken.None));

        exception.Message.ShouldContain("only has 1 unit(s) available");

        // Payment must never be attempted for a request that can't be fulfilled.
        _paymentServiceMock.Verify(
            p => p.ChargeAsync(It.IsAny<SimulatedCardDetails>(), It.IsAny<decimal>(), It.IsAny<CancellationToken>()),
            Times.Never);

        _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Never);
        _orderRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
    }

    [Fact]
    public async Task Handle_ProductHasNoInventoryRecord_TreatsAvailableAsZero_ThrowsConflictException()
    {
        var address = ValidAddress();
        var product = new Product { Id = 1, Name = "No Stock Record", Price = 10m, CategoryId = 1, Inventory = null };
        var cart = CartWithItems(LineItem(product, quantity: 1));

        _addressRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(address);
        _cartRepositoryMock.Setup(r => r.GetByOwnerAsync("user-1", null)).ReturnsAsync(cart);

        await Should.ThrowAsync<ConflictException>(
            () => _handler.Handle(ValidCommand(), CancellationToken.None));
    }

    [Fact]
    public async Task Handle_EmptyCart_ThrowsConflictException()
    {
        var address = ValidAddress();
        var emptyCart = new Cart { Id = 1, UserId = "user-1", CartItems = new List<CartItem>() };

        _addressRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(address);
        _cartRepositoryMock.Setup(r => r.GetByOwnerAsync("user-1", null)).ReturnsAsync(emptyCart);

        var exception = await Should.ThrowAsync<ConflictException>(
            () => _handler.Handle(ValidCommand(), CancellationToken.None));

        exception.Message.ShouldBe("Your cart is empty.");
    }

    // ---- 3. Correct total calculation ----

    [Fact]
    public async Task Handle_MultipleLineItems_ChargesAndStoresCorrectTotal()
    {
        var address = ValidAddress();
        var productA = ProductWithStock(id: 1, price: 25.50m, quantityInStock: 10);
        var productB = ProductWithStock(id: 2, price: 10.00m, quantityInStock: 10);

        // 2 x 25.50 + 3 x 10.00 = 51.00 + 30.00 = 81.00
        var cart = CartWithItems(
            LineItem(productA, quantity: 2),
            LineItem(productB, quantity: 3));

        _addressRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(address);
        _cartRepositoryMock.Setup(r => r.GetByOwnerAsync("user-1", null)).ReturnsAsync(cart);
        SetupSuccessfulPayment(expectedAmount: 81.00m);

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.TotalAmount.ShouldBe(81.00m);

        // Confirms the exact amount charged to the (mocked) payment gateway
        // matches the order total — not just that SOME amount was charged.
        _paymentServiceMock.Verify(
            p => p.ChargeAsync(It.IsAny<SimulatedCardDetails>(), 81.00m, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_UsesCartItemSnapshotPrice_NotLiveProductPrice()
    {
        var address = ValidAddress();
        var product = ProductWithStock(id: 1, price: 100m, quantityInStock: 10); // current live price is 100

        // Snapshot price captured when added to cart was 40 (e.g. price changed since)
        var cartItem = LineItem(product, quantity: 1);
        cartItem.UnitPrice = 40m;
        var cart = CartWithItems(cartItem);

        _addressRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(address);
        _cartRepositoryMock.Setup(r => r.GetByOwnerAsync("user-1", null)).ReturnsAsync(cart);
        SetupSuccessfulPayment(expectedAmount: 40m);

        var result = await _handler.Handle(ValidCommand(), CancellationToken.None);

        result.TotalAmount.ShouldBe(40m); // must honor the snapshot, not live Product.Price
    }

    // ---- 4. Inventory reduction ----

    [Fact]
    public async Task Handle_ValidOrder_ReducesInventoryQuantityByOrderedAmount()
    {
        var address = ValidAddress();
        var product = ProductWithStock(id: 1, price: 50m, quantityInStock: 10);
        var cart = CartWithItems(LineItem(product, quantity: 4));

        _addressRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(address);
        _cartRepositoryMock.Setup(r => r.GetByOwnerAsync("user-1", null)).ReturnsAsync(cart);
        SetupSuccessfulPayment();

        await _handler.Handle(ValidCommand(), CancellationToken.None);

        product.Inventory!.QuantityInStock.ShouldBe(6); // 10 - 4
    }

    [Fact]
    public async Task Handle_MultipleProducts_ReducesEachProductsInventoryIndependently()
    {
        var address = ValidAddress();
        var productA = ProductWithStock(id: 1, price: 20m, quantityInStock: 10);
        var productB = ProductWithStock(id: 2, price: 15m, quantityInStock: 5);

        var cart = CartWithItems(
            LineItem(productA, quantity: 3),
            LineItem(productB, quantity: 5));

        _addressRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(address);
        _cartRepositoryMock.Setup(r => r.GetByOwnerAsync("user-1", null)).ReturnsAsync(cart);
        SetupSuccessfulPayment();

        await _handler.Handle(ValidCommand(), CancellationToken.None);

        productA.Inventory!.QuantityInStock.ShouldBe(7);  // 10 - 3
        productB.Inventory!.QuantityInStock.ShouldBe(0);  // 5 - 5, fully depleted is valid
    }

    // ---- 5. Payment status (failure path) ----

    [Fact]
    public async Task Handle_PaymentDeclined_ThrowsPaymentFailedExceptionAndCreatesNoOrder()
    {
        var address = ValidAddress();
        var product = ProductWithStock(id: 1, price: 50m, quantityInStock: 10);
        var cart = CartWithItems(LineItem(product, quantity: 1));

        _addressRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(address);
        _cartRepositoryMock.Setup(r => r.GetByOwnerAsync("user-1", null)).ReturnsAsync(cart);

        _paymentServiceMock
            .Setup(p => p.ChargeAsync(It.IsAny<SimulatedCardDetails>(), It.IsAny<decimal>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PaymentResult { Success = false, FailureReason = "The card was declined." });

        var exception = await Should.ThrowAsync<PaymentFailedException>(
            () => _handler.Handle(ValidCommand(), CancellationToken.None));

        exception.Message.ShouldBe("The card was declined.");

        // No database side effects at all when payment fails.
        _unitOfWorkMock.Verify(u => u.BeginTransactionAsync(), Times.Never);
        _orderRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Order>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
        product.Inventory!.QuantityInStock.ShouldBe(10); // untouched
        cart.CartItems.ShouldNotBeEmpty(); // untouched
    }

    // ---- Address ownership (regression guard for the security-review fix) ----

    [Fact]
    public async Task Handle_AddressBelongsToDifferentUser_ThrowsNotFoundException()
    {
        var someoneElsesAddress = ValidAddress(userId: "different-user");
        _addressRepositoryMock.Setup(r => r.GetByIdAsync(10)).ReturnsAsync(someoneElsesAddress);

        await Should.ThrowAsync<NotFoundException>(
            () => _handler.Handle(ValidCommand(), CancellationToken.None));

        _cartRepositoryMock.Verify(r => r.GetByOwnerAsync(It.IsAny<string>(), It.IsAny<Guid?>()), Times.Never);
    }
}
