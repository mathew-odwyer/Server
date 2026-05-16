namespace Winterhaven.Gateway.Presentation.Validation;

using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using System;

internal sealed class ValidatorFactory : IValidatorFactory
{
    private readonly IServiceProvider serviceProvider;

    public ValidatorFactory(IServiceProvider serviceProvider)
    {
        this.serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    public IValidator<T>? GetValidator<T>()
    {
        return this.serviceProvider.GetService<IValidator<T>>();
    }
}