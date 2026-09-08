using FluentValidation;
using MaleFashion.Application.Features.Products.Commands.Common;

namespace MaleFashion.Application.Features.Products.Commands.UpdateProduct;

public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(x => x.Id).GreaterThan(0);
        this.ApplyProductRules();
    }
}