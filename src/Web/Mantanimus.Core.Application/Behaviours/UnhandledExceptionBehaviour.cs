// <copyright file="UnhandledExceptionBehaviour.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Core.Application.Behaviours;

using System.Diagnostics.CodeAnalysis;
using MediatR;

[ExcludeFromCodeCoverage]
internal sealed class UnhandledExceptionBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
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
