// <copyright file="ValidationBehaviour.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Behaviours;

using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using MediatR;
using ValidationException = Exceptions.ValidationException;

[ExcludeFromCodeCoverage]
internal sealed class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
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

            var failure = validationResults
                .Where(x => x.Errors.Count != 0)
                .SelectMany(x => x.Errors)
                .FirstOrDefault();

            if (failure != null)
            {
                throw new ValidationException($"{failure.PropertyName}: {failure.ErrorMessage}");
            }
        }

        return await next(cancellationToken).ConfigureAwait(false);
    }
}
