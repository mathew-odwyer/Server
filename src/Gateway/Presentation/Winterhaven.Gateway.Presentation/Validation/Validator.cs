namespace Winterhaven.Gateway.Presentation.Validation;

using FluentValidation;
using System.Linq;
using ValidationException = Core.Domain.Exceptions.ValidationException;

internal static class Validator
{
    public static void Validate<T>(IValidatorFactory factory, T parameters)
    {
        var validator = factory.GetValidator<T>();

        if (validator is null)
        {
            return;
        }

        var result = validator.Validate(parameters);

        if (!result.IsValid)
        {
            var errors = result.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage)
                          .Where(m => !string.IsNullOrWhiteSpace(m))
                          .ToArray()
                );

            throw new ValidationException(errors);
        }
    }
}