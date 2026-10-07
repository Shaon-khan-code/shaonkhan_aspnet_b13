using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.Features.Inventory.Queries.GetLowStockProducts;
using MaleFashion.Application.Tests.Common;
using DomainInventory = MaleFashion.Domain.Entities.Inventory;
using Microsoft.Extensions.Logging;
using Moq;
using Shouldly;
using Xunit;

namespace MaleFashion.Application.UnitTests.Inventory;

public class GetLowStockProductsQueryHandlerTests
{
    private readonly Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private readonly Mock<IInventoryRepository> _inventoryRepositoryMock;
    private readonly GetLowStockProductsQueryHandler _handler;

    public GetLowStockProductsQueryHandlerTests()
    {
        _unitOfWorkMock = TestUnitOfWorkFactory.CreateUnitOfWorkMock();
        _inventoryRepositoryMock = Mock.Get(_unitOfWorkMock.Object.Inventory);

        var mapper = TestUnitOfWorkFactory.CreateRealMapper();
        var logger = new Mock<ILogger<GetLowStockProductsQueryHandler>>().Object;

        _handler = new GetLowStockProductsQueryHandler(_unitOfWorkMock.Object, mapper, logger);
    }

    private static DomainInventory LowStockItem(int productId, string name, string sku, int inStock, int reserved, int reorderLevel) => new()
    {
        Id = productId,
        ProductId = productId,
        Product = new Domain.Entities.Product { Id = productId, Name = name, Sku = sku, CategoryId = 1 },
        QuantityInStock = inStock,
        ReservedQuantity = reserved,
        ReorderLevel = reorderLevel
    };

    // ---- 1. Low stock detected ----

    [Fact]
    public async Task Handle_LowStockItemsExist_ReturnsCorrectlyMappedList()
    {
        // FIX: the two items were being created and discarded instead of
        // being added to the list, and the mock was returning an empty
        // list regardless. Both are fixed below.
        var items = new List<DomainInventory>
        {
            LowStockItem(1, "Cotton Shirt", "SKU-001", inStock: 2, reserved: 0, reorderLevel: 10),
            LowStockItem(2, "Denim Jeans", "SKU-002", inStock: 5, reserved: 3, reorderLevel: 5)
        };

        _inventoryRepositoryMock.Setup(r => r.GetAllLowStockAsync()).ReturnsAsync(items);

        var result = await _handler.Handle(new GetLowStockProductsQuery(), CancellationToken.None);

        result.Count.ShouldBe(2);
        result[0].ProductName.ShouldBe("Cotton Shirt");
        result[0].ProductSku.ShouldBe("SKU-001");
        result[0].QuantityInStock.ShouldBe(2);
        result[1].ProductName.ShouldBe("Denim Jeans");
        result[1].AvailableQuantity.ShouldBe(2); // 5 - 3
    }

    [Fact]
    public async Task Handle_LowStockItemsExist_CallsRepositoryExactlyOnce()
    {
        _inventoryRepositoryMock.Setup(r => r.GetAllLowStockAsync()).ReturnsAsync(new List<DomainInventory>
            {
            LowStockItem(1, "Cotton Shirt", "SKU-001", inStock: 1, reserved: 0, reorderLevel: 5)
            });

        await _handler.Handle(new GetLowStockProductsQuery(), CancellationToken.None);

        _inventoryRepositoryMock.Verify(r => r.GetAllLowStockAsync(), Times.Once);
    }

    // ---- 2. No low stock products ----

    [Fact]
    public async Task Handle_NoLowStockItems_ReturnsEmptyList()
    {
        _inventoryRepositoryMock.Setup(r => r.GetAllLowStockAsync()).ReturnsAsync(new List<DomainInventory>());

        var result = await _handler.Handle(new GetLowStockProductsQuery(), CancellationToken.None);

        result.ShouldNotBeNull();
        result.ShouldBeEmpty();
    }

    // ---- 3. Repository/service failure ----

    [Fact]
    public async Task Handle_RepositoryThrows_ExceptionPropagatesToCaller()
    {
        _inventoryRepositoryMock
            .Setup(r => r.GetAllLowStockAsync())
            .ThrowsAsync(new InvalidOperationException("Database connection failed."));

        var exception = await Should.ThrowAsync<InvalidOperationException>(
            () => _handler.Handle(new GetLowStockProductsQuery(), CancellationToken.None));

        exception.Message.ShouldBe("Database connection failed.");
    }
}
