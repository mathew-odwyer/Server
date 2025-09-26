// <copyright file="PerformanceBehaviour.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Behaviours;

using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using MediatR;
using Microsoft.Extensions.Logging;

/// <summary>
/// Provides a pipeline behaviour that is used to measure the time it takes for a request to complete.
/// </summary>
/// <typeparam name="TRequest">
/// The type of the request.
/// </typeparam>
/// <typeparam name="TResponse">
/// The type of the response.
/// </typeparam>
[ExcludeFromCodeCoverage]
public class PerformanceBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<PerformanceBehaviour<TRequest, TResponse>> logger;

    /// <summary>
    /// The watch, used to determine how long a request has taken.
    /// </summary>
    private readonly Stopwatch watch;

    /// <summary>
    /// Initializes a new instance of the <see cref="PerformanceBehaviour{TRequest, TResponse}"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when <paramref name="logger"/> is <c>null</c>.
    /// </exception>
    public PerformanceBehaviour(ILogger<PerformanceBehaviour<TRequest, TResponse>> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.watch = new Stopwatch();
    }

    /// <inheritdoc/>
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
            this.logger.LogWarning("Long Running Request: {Name} ({Elapsed} milliseconds) {Request}", typeof(TRequest).Name, elapsed, request);
        }

        return response;
    }
}
