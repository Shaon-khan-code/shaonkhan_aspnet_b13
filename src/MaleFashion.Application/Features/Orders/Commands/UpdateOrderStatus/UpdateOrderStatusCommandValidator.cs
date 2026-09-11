using FluentValidation;

namespace MaleFashion.Application.Features.Orders.Commands.UpdateOrderStatus;

public class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId).GreaterThan(0);

        RuleFor(x => x.Status)
            .IsInEnum()
            .WithMessage("Invalid order status.");
    }
}