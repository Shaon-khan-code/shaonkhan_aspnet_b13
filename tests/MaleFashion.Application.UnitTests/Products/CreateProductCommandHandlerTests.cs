using MaleFashion.Application.Common.Exceptions;
using MaleFashion.Application.Contracts.Repositories;
using MaleFashion.Application.Contracts.Services;
using MaleFashion.Application.Features.Products.Commands.CreateProduct;
using MaleFashion.Application.Tests.Common;
using MaleFashion.Domain.Entities;
using Moq;
using Shouldly;
using System.Timers;
using Xunit;

namespace MaleFashion.Application.Tests.Products;

public class CreateProductCommandHandlerTests
{
    private readonly Mock<IApplicationUnitOfWork> _unitOfWorkMock;
    private readonly Mock<ICategoryRepository> _categoryRepositoryMock;
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IFileStorageService> _fileStorageServiceMock;
    private readonly CreateProductCommandHandler _handler;

    public CreateProductCommandHandlerTests()
    {
        _unitOfWorkMock = TestUnitOfWorkFactory.CreateUnitOfWorkMock();
        _categoryRepositoryMock = Mock.Get(_unitOfWorkMock.Object.Categories);
        _productRepositoryMock = Mock.Get(_unitOfWorkMock.Object.Products);
        _fileStorageServiceMock = new Mock<IFileStorageService>();

        var mapper = TestUnitOfWorkFactory.CreateRealMapper();

        _handler = new CreateProductCommandHandler(
            _unitOfWorkMock.Object, _fileStorageServiceMock.Object, mapper);
    }

    private static Category ValidCategory() => new()
    {
        Id = 1,
        Name = "Shirts",
        Slug = "shirts"
    };

    private static CreateProductCommand ValidCommand() => new()
    {
        Name = "Cotton Shirt",
        Slug = "cotton-shirt",
        Sku = "SKU-001",
        Price = 29.99m,
        CategoryId = 1,
        IsActive = true
    };

    // ---- 1. Valid product creation ----

    [Fact]
    public async Task Handle_ValidCommand_ReturnsProductDtoWithCorrectFields()
    {
        var category = ValidCategory();
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);

        var command = ValidCommand();

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ShouldNotBeNull();
        result.Name.ShouldBe("Cotton Shirt");
        result.Slug.ShouldBe("cotton-shirt");
        result.Sku.ShouldBe("SKU-001");
        result.Price.ShouldBe(29.99m);
        result.IsActive.ShouldBeTrue();
        result.CategoryId.ShouldBe(1);
        result.CategoryName.ShouldBe("Shirts");
    }

    [Fact]
    public async Task Handle_ValidCommand_NoImage_ImageUrlIsNull()
    {
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ValidCategory());

        var command = ValidCommand(); // ImageStream left null

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ImageUrl.ShouldBeNull();
        _fileStorageServiceMock.Verify(
            f => f.UploadFileAsync(It.IsAny<Stream>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task Handle_ValidCommandWithImage_UploadsImageAndSetsImageUrl()
    {
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ValidCategory());

        _fileStorageServiceMock
            .Setup(f => f.UploadFileAsync(It.IsAny<Stream>(), "shirt.jpg", "image/jpeg", "products", It.IsAny<CancellationToken>()))
            .ReturnsAsync("products/generated-key-shirt.jpg");

        _fileStorageServiceMock
            .Setup(f => f.GetFileUrl("products/generated-key-shirt.jpg"))
            .Returns("https://cdn.example.com/products/generated-key-shirt.jpg");

        var command = ValidCommand();
        command.ImageStream = new MemoryStream(new byte[] { 1, 2, 3 });
        command.ImageFileName = "shirt.jpg";
        command.ImageContentType = "image/jpeg";

        var result = await _handler.Handle(command, CancellationToken.None);

        result.ImageUrl.ShouldBe("https://cdn.example.com/products/generated-key-shirt.jpg");
        _fileStorageServiceMock.Verify(
            f => f.UploadFileAsync(command.ImageStream, "shirt.jpg", "image/jpeg", "products", It.IsAny<CancellationToken>()),
            Times.Once);
    }

    // ---- 2. Invalid product scenario (handler-level: category doesn't exist) ----

    [Fact]
    public async Task Handle_CategoryDoesNotExist_ThrowsNotFoundException()
    {
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync((Category?)null);

        var command = ValidCommand();

        var exception = await Should.ThrowAsync<NotFoundException>(
            () => _handler.Handle(command, CancellationToken.None));

        exception.Message.ShouldContain("Category");

        _productRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Never);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Never);
    }

    // ---- 3. Repository interaction ----

    [Fact]
    public async Task Handle_ValidCommand_CallsAddAsyncAndSaveChangesExactlyOnce()
    {
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(ValidCategory());

        var command = ValidCommand();

        await _handler.Handle(command, CancellationToken.None);

        _productRepositoryMock.Verify(r => r.AddAsync(It.IsAny<Product>()), Times.Once);
        _unitOfWorkMock.Verify(u => u.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_ValidCommand_AddsProductWithCorrectCategoryNavigationSet()
    {
        var category = ValidCategory();
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(category);

        Product? capturedProduct = null;
        _productRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<Product>()))
            .Callback<Product>(p => capturedProduct = p)
            .Returns(Task.CompletedTask);

        var command = ValidCommand();

        await _handler.Handle(command, CancellationToken.None);

        capturedProduct.ShouldNotBeNull();
        capturedProduct!.CategoryId.ShouldBe(1);
        capturedProduct.Category.ShouldBe(category); // regression guard for the earlier navigation-property bug
        capturedProduct.CreatedAt.ShouldNotBe(default);
    }

    [Fact]
    public async Task Handle_CallsCategoryRepository_WithRequestedCategoryId()
    {
        _categoryRepositoryMock.Setup(r => r.GetByIdAsync(It.IsAny<int>())).ReturnsAsync(ValidCategory());

        var command = ValidCommand();
        command.CategoryId = 1;

        await _handler.Handle(command, CancellationToken.None);

        _categoryRepositoryMock.Verify(r => r.GetByIdAsync(1), Times.Once);
    }
}