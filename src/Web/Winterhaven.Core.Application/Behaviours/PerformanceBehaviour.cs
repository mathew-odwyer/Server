// <copyright file="PerformanceBehaviour.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Behaviours;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MediatR;
using Microsoft.Extensions.Logging;

[ExcludeFromCodeCoverage]
internal sealed class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<PerformanceBehaviour<TRequest, TResponse>> logger;

    private readonly Stopwatch watch;

    public PerformanceBehaviour(ILogger<PerformanceBehaviour<TRequest, TResponse>> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.watch = new Stopwatch();
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(next);

        this.watch.Start();
        var response = await next(cancellationToken).ConfigureAwait(false);
        this.watch.Stop();

        const long waitMs = 1000;
        long elapsed = this.watch.ElapsedMilliseconds;

        if (elapsed >= waitMs)
        {
            this.logger.LogWarning("Long Running Request: '{Name}': ({Elapsed} milliseconds)", typeof(TRequest).Name, elapsed);
        }

        return response;
    }
}
