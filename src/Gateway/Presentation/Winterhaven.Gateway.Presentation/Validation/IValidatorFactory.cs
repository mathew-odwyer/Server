namespace Winterhaven.Gateway.Presentation.Validation;

using FluentValidation;

public interface IValidatorFactory
{
    IValidator<T>? GetValidator<T>();
}