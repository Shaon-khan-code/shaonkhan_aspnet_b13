using MaleFashion.Application.Features.Products.Commands.CreateProduct;
using MaleFashion.Application.Tests.Common;
using Shouldly;
using Xunit;

namespace MaleFashion.Application.Tests.Products;

public class CreateProductCommandValidatorTests
{
    private readonly CreateProductCommandValidator _validator = new();

    private static CreateProductCommand ValidCommand() => new()
    {
        Name = "Cotton Shirt",
        Slug = "cotton-shirt",
        Sku = "SKU-001",
        Price = 29.99m,
        CategoryId = 1
    };

    [Fact]
    public void Validate_ValidCommand_IsValid()
    {
        _validator.ShouldBeValid(ValidCommand());
    }

    [Fact]
    public void Validate_EmptyName_HasValidationError()
    {
        var command = ValidCommand();
        command.Name = "";

        _validator.ShouldHaveValidationErrorFor(command, nameof(command.Name));
    }

    [Fact]
    public void Validate_ZeroPrice_HasValidationError()
    {
        var command = ValidCommand();
        command.Price = 0;

        _validator.ShouldHaveValidationErrorFor(command, nameof(command.Price));
    }

    [Fact]
    public void Validate_NegativePrice_HasValidationError()
    {
        var command = ValidCommand();
        command.Price = -10;

        _validator.ShouldHaveValidationErrorFor(command, nameof(command.Price));
    }

    [Fact]
    public void Validate_DiscountPriceGreaterThanPrice_HasValidationError()
    {
        var command = ValidCommand();
        command.Price = 20;
        command.DiscountPrice = 25;

        _validator.ShouldHaveValidationErrorFor(command, nameof(command.DiscountPrice));
    }

    [Fact]
    public void Validate_DiscountPriceNotSet_DoesNotValidateDiscountPrice()
    {
        var command = ValidCommand();
        command.DiscountPrice = null;

        _validator.ShouldNotHaveValidationErrorFor(command, nameof(command.DiscountPrice));
    }

    [Fact]
    public void Validate_CategoryIdZero_HasValidationError()
    {
        var command = ValidCommand();
        command.CategoryId = 0;

        _validator.ShouldHaveValidationErrorFor(command, nameof(command.CategoryId));
    }

    [Fact]
    public void Validate_ImageProvided_InvalidContentType_HasValidationError()
    {
        var command = ValidCommand();
        command.ImageStream = new MemoryStream(new byte[] { 1 });
        command.ImageFileName = "malware.exe";
        command.ImageContentType = "application/octet-stream";

        _validator.ShouldHaveValidationErrorFor(command, nameof(command.ImageContentType));
    }

    [Fact]
    public void Validate_NoImageProvided_ImageRulesAreSkipped()
    {
        var command = ValidCommand(); // ImageStream left null

        _validator.ShouldNotHaveValidationErrorFor(command, nameof(command.ImageContentType));
        _validator.ShouldNotHaveValidationErrorFor(command, nameof(command.ImageFileName));
    }
}