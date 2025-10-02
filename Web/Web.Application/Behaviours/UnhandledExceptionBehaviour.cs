// <copyright file="UnhandledExceptionBehaviour.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Behaviours;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using MediatR;

/// <summary>
/// Provides a pipeline behaviour that is used to handle unhandled exceptions.
/// </summary>
/// <typeparam name="TRequest">
/// The type of the request.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response.
/// </typeparam>
[ExcludeFromCodeCoverage(Justification = "Behaviour")]
public sealed class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UnhandledExceptionBehaviour{TRequest, TResponse}"/> class.
    /// </summary>
    public UnhandledExceptionBehaviour()
    {
    }

    /// <inheritdoc/>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(next);

        try
        {
            return await next(cancellationToken).ConfigureAwait(false);
        }
        catch (Exception)
        {
            throw;
        }
    }
}
