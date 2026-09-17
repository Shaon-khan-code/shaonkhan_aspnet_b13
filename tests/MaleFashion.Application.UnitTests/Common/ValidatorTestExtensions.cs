using FluentValidation;
using FluentValidation.Results;
using Shouldly;

namespace MaleFashion.Application.Tests.Common;

public static class ValidatorTestExtensions
{
    public static void ShouldHaveValidationErrorFor<T>(
        this IValidator<T> validator, T instance, string propertyName)
    {
        var result = validator.Validate(instance);
        result.Errors.ShouldContain(e => e.PropertyName == propertyName);
    }

    public static void ShouldNotHaveValidationErrorFor<T>(
        this IValidator<T> validator, T instance, string propertyName)
    {
        var result = validator.Validate(instance);
        result.Errors.ShouldNotContain(e => e.PropertyName == propertyName);
    }

    public static void ShouldBeValid<T>(this IValidator<T> validator, T instance)
    {
        var result = validator.Validate(instance);
        result.IsValid.ShouldBeTrue(
            $"Expected valid, but got errors: {string.Join(", ", result.Errors.Select(e => e.ErrorMessage))}");
    }
}