using FluentValidation;

namespace MaleFashion.Application.Features.Products.Commands.Common;

public static class ProductCommandValidationRules
{
    private static readonly string[] AllowedContentTypes = { "image/jpeg", "image/png", "image/webp" };
    private static readonly string[] AllowedExtensions = { ".jpg", ".jpeg", ".png", ".webp" };
    private const long MaxImageSizeBytes = 5 * 1024 * 1024; // 5 MB

    public static void ApplyProductRules<T>(this AbstractValidator<T> validator) where T : IProductCommand
    {
        validator.RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
        validator.RuleFor(x => x.Slug).NotEmpty().MaximumLength(220);
        validator.RuleFor(x => x.Description).MaximumLength(2000);
        validator.RuleFor(x => x.Sku).NotEmpty().MaximumLength(50);

        validator.RuleFor(x => x.Price)
            .GreaterThan(0)
            .WithMessage("Price must be greater than zero.");

        validator.RuleFor(x => x.DiscountPrice)
            .GreaterThan(0)
            .LessThan(x => x.Price)
            .WithMessage("Discount price must be greater than zero and less than the regular price.")
            .When(x => x.DiscountPrice.HasValue);

        validator.RuleFor(x => x.CategoryId)
            .GreaterThan(0)
            .WithMessage("A category must be selected.");

        validator.RuleFor(x => x.ImageContentType)
            .Must(ct => ct != null && AllowedContentTypes.Contains(ct.ToLowerInvariant()))
            .WithMessage("Only JPEG, PNG, or WEBP images are allowed.")
            .When(x => x.ImageStream != null);

        validator.RuleFor(x => x.ImageFileName)
            .Must(name => name != null && AllowedExtensions.Contains(Path.GetExtension(name).ToLowerInvariant()))
            .WithMessage("Invalid image file extension.")
            .When(x => x.ImageStream != null);

        validator.RuleFor(x => x.ImageStream)
            .Must(stream => stream != null && stream.Length <= MaxImageSizeBytes)
            .WithMessage("Image file must not exceed 5 MB.")
            .When(x => x.ImageStream != null);
    }
}