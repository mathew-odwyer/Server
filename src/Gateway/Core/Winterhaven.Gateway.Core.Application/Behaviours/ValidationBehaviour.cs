namespace Winterhaven.Gateway.Core.Application.Behaviours;

using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ValidationException = Winterhaven.Common.Exceptions.ValidationException;

[ExcludeFromCodeCoverage]
public sealed class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : notnull
{
    private readonly IEnumerable<IValidator<TRequest>> validators;

    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        this.validators = validators;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(next);

        if (this.validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(this.validators.Select(x => x.ValidateAsync(context, cancellationToken))).ConfigureAwait(false);

            var errors = validationResults
                .Where(x => x.Errors.Count != 0)
                .SelectMany(x => x.Errors)
                .GroupBy(x => x.PropertyName)
                .ToDictionary(
                    x => x.Key,
                    x => x.Where(e => !string.IsNullOrWhiteSpace(e.ErrorMessage))
                          .Select(e => e.ErrorMessage)
                          .ToArray()
                );

            if (errors != null && errors.Count != 0)
            {
                throw new ValidationException(errors);
            }
        }

        return await next(cancellationToken).ConfigureAwait(false);
    }
}