using FluentValidation;

namespace MaleFashion.Application.Features.Products.Commands.CreateProduct;

public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    public CreateProductCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(200);

        RuleFor(x => x.Slug)
            .NotEmpty()
            .MaximumLength(220);

        RuleFor(x => x.Description)
            .MaximumLength(2000);

        RuleFor(x => x.Sku)
            .NotEmpty()
            .MaximumLength(50);

        RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than zero.");

        RuleFor(x => x.DiscountPrice)
            .GreaterThan(0)
            .LessThan(x => x.Price)
            .WithMessage("Discount price must be greater than zero and less than the regular price.")
            .When(x => x.DiscountPrice.HasValue);

        RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("A category must be selected.");
    }
}