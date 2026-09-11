using FluentValidation;

namespace MaleFashion.Application.Features.Inventory.Commands.UpdateInventory;

public class UpdateInventoryCommandValidator : AbstractValidator<UpdateInventoryCommand>
{
    public UpdateInventoryCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);

        RuleFor(x => x.QuantityInStock)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Quantity in stock cannot be negative.");

        RuleFor(x => x.ReorderLevel)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Reorder level cannot be negative.");
    }
}