using FluentValidation;

namespace MaleFashion.Application.Features.Cart.Commands.UpdateCartItemQuantity;

public class UpdateCartItemQuantityCommandValidator : AbstractValidator<UpdateCartItemQuantityCommand>
{
    public UpdateCartItemQuantityCommandValidator()
    {
        RuleFor(x => x.CartItemId).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0)
            .WithMessage("Quantity must be at least 1. Use Remove to delete an item.");

        RuleFor(x => x)
            .Must(x => x.UserId != null || x.GuestId != null)
            .WithMessage("A cart owner (user or guest) must be specified.");
    }


}