using FluentValidation;

namespace MaleFashion.Application.Features.Cart.Commands.AddToCart;

public class AddToCartCommandValidator : AbstractValidator<AddToCartCommand>
{
    public AddToCartCommandValidator()
    {
        RuleFor(x => x.ProductId).GreaterThan(0);
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be at least 1.");

        RuleFor(x => x)
            .Must(x => x.UserId != null || x.GuestId != null)
            .WithMessage("A cart owner (user or guest) must be specified.");
    }
}