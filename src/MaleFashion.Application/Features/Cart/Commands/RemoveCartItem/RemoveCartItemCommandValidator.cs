
using FluentValidation;

namespace MaleFashion.Application.Features.Cart.Commands.RemoveCartItem;

public class RemoveCartItemCommandValidator : AbstractValidator<RemoveCartItemCommand>
{
    public RemoveCartItemCommandValidator()
    {
        RuleFor(x => x.CartItemId).GreaterThan(0);
    }
}