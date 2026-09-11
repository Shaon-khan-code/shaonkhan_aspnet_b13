using FluentValidation;

namespace MaleFashion.Application.Features.Inventory.Commands.CreateInventory;

public class CreateInventoryCommandValidator : AbstractValidator<CreateInventoryCommand>
{
    public CreateInventoryCommandValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);

        RuleFor(x => x.QuantityInStock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Quantity in stock cannot be negative.");

        RuleFor(x => x.ReorderLevel)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Reorder level cannot be negative.");
    }
}