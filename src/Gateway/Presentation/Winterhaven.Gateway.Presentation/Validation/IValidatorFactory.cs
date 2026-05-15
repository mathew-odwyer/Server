namespace Winterhaven.Gateway.Presentation.Validation;

using FluentValidation;

internal interface IValidatorFactory
{
    IValidator<T>? GetValidator<T>();
}