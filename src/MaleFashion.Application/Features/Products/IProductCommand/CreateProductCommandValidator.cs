using FluentValidation;
using MaleFashion.Application.Features.Products.Commands.Common;

namespace MaleFashion.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        this.ApplyProductRules();
    }
}