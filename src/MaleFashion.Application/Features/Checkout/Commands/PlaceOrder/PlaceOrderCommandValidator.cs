using FluentValidation;
using MaleFashion.Application.Common.Helpers;
using MaleFashion.Domain.Enums;

namespace MaleFashion.Application.Features.Checkout.Commands.PlaceOrder;

public class PlaceOrderCommandValidator : AbstractValidator<PlaceOrderCommand>
{
    public PlaceOrderCommandValidator()
    {
        RuleFor(x => x.ShippingAddressId).GreaterThanOrEqualTo(0);

        RuleFor(x => x)
            .Must(x => x.ShippingAddressId > 0 || x.NewShippingAddress != null)
            .WithMessage("A shipping address is required.");

        RuleFor(x => x)
            .Must(x => x.UserId != null || x.GuestId != null)
            .WithMessage("A cart owner (user or guest) must be specified.");

        RuleFor(x => x.PaymentMethod).IsInEnum();

        RuleFor(x => x.Card).NotNull().WithMessage("Card details are required.");

        When(x => x.NewShippingAddress != null, () =>
        {
            RuleFor(x => x.NewShippingAddress!.FullName).NotEmpty().MaximumLength(200);
            RuleFor(x => x.NewShippingAddress!.Phone).NotEmpty().MaximumLength(30);
            RuleFor(x => x.NewShippingAddress!.Line1).NotEmpty().MaximumLength(200);
            RuleFor(x => x.NewShippingAddress!.City).NotEmpty().MaximumLength(100);
            RuleFor(x => x.NewShippingAddress!.PostalCode).NotEmpty().MaximumLength(20);
            RuleFor(x => x.NewShippingAddress!.Country).NotEmpty().MaximumLength(100);
        });

        When(x => x.PaymentMethod is PaymentMethod.Bkash or PaymentMethod.Nagad, () =>
        {
            RuleFor(x => x.WalletNumber)
                .Must(MobileWalletValidator.IsValidWalletNumber)
                .WithMessage("Enter a valid Bangladeshi mobile number, for example 01712345678.");

            RuleFor(x => x.TransactionId)
                .Must(MobileWalletValidator.IsValidTransactionId)
                .WithMessage("Enter the Transaction ID (TrxID) from your payment message: 8 to 12 letters or digits.");
        });
    }
}
