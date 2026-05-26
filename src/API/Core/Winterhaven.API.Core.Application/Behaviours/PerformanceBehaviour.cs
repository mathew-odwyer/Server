using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Winterhaven.API.Core.Application.Behaviours;

/// <summary>
/// </summary>
/// <typeparam name="TRequest">
/// </typeparam>
/// <typeparam name="TResponse">
/// </typeparam>
[ExcludeFromCodeCoverage]
public sealed class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<PerformanceBehaviour<TRequest, TResponse>> logger;

    private readonly Stopwatch watch;

    /// <summary>
    /// </summary>
    /// <param name="logger">
    /// </param>
    public PerformanceBehaviour(ILogger<PerformanceBehaviour<TRequest, TResponse>> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        watch = new Stopwatch();
    }

    /// <summary>
    /// </summary>
    /// <param name="request">
    /// </param>
    /// <param name="next">
    /// </param>
    /// <param name="cancellationToken">
    /// </param>
    /// <returns>
    /// </returns>
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(next);

        watch.Start();
        var response = await next(cancellationToken).ConfigureAwait(false);
        watch.Stop();

        const long waitMs = 1000;
        long elapsed = watch.ElapsedMilliseconds;

        if (elapsed >= waitMs)
        {
            logger.LogWarning("Long Running Request: '{Name}': ({Elapsed} milliseconds)", typeof(TRequest).Name, elapsed);
        }

        return response;
    }
}