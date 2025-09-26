// <copyright file="ValidationBehaviour.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Behaviours;

using System.Diagnostics.CodeAnalysis;
using FluentValidation;
using MediatR;
using ValidationException = Exceptions.ValidationException;

/// <summary>
/// Provides a pipeline behaviour that is used to handle any validation errors by throwing a <see cref="ValidationException"/>.
/// </summary>
/// <typeparam name="TRequest">
/// The type of the request.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response.
/// </typeparam>
[ExcludeFromCodeCoverage]
public sealed class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
     where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// The validators used to query the request for validation errors.
    /// </summary>
    private readonly IEnumerable<IValidator<TRequest>> validators;

    /// <summary>
    /// Initializes a new instance of the <see cref="ValidationBehaviour{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="validators">
    /// The validators used to query the request for validation errors.
    /// </param>
    public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validators)
    {
        this.validators = validators;
    }

    /// <inheritdoc/>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(next);

        if (this.validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);

            var validationResults = await Task.WhenAll(this.validators.Select(x => x.ValidateAsync(context, cancellationToken))).ConfigureAwait(false);

            var failures = validationResults
                .Where(x => x.Errors.Count != 0)
                .SelectMany(x => x.Errors)
                .ToList();

            if (failures.Count != 0)
            {
                throw new ValidationException(failures);
            }
        }

        return await next(cancellationToken).ConfigureAwait(false);
    }
}
