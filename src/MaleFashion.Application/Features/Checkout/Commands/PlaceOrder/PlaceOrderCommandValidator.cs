using FluentValidation;

namespace MaleFashion.Application.Features.Checkout.Commands.PlaceOrder;

public class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        RuleFor(x => x.ShippingAddressId).GreaterThan(0);

        RuleFor(x => x)
            .Must(x => x.UserId != null || x.GuestId != null)
            .WithMessage("A cart owner (user or guest) must be specified.");

        RuleFor(x => x.Card).NotNull().WithMessage("Card details are required.");
    }
}