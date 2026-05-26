using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Winterhaven.API.Core.Application.Contexts.Users;
using Winterhaven.API.Core.Domain.Attributes.Users;

namespace Winterhaven.API.Core.Application.Behaviours;

/// <summary>
/// </summary>
/// <typeparam name="TRequest">
/// </typeparam>
/// <typeparam name="TResponse">
/// </typeparam>
public sealed class AuthorizationBehahviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly IActorContext actorContext;

    private readonly ILogger<AuthorizationBehahviour<TRequest, TResponse>> logger;

    /// <summary>
    /// </summary>
    /// <param name="logger">
    /// </param>
    /// <param name="actorContext">
    /// </param>
    public AuthorizationBehahviour(ILogger<AuthorizationBehahviour<TRequest, TResponse>> logger, IActorContext actorContext)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.actorContext = actorContext ?? throw new ArgumentNullException(nameof(actorContext));
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
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);

        var type = typeof(TRequest);
        bool isRequired = type.IsDefined(typeof(AuthorizeAttribute), inherit: true);

        // Determine whether this request type requires an authenticated user.
        if (isRequired && actorContext.Actor == null)
        {
            logger.LogError("Authenticated user required for request {RequestType} but no user was present.", type.FullName);

            // This is an unexpected server/configuration issue (controller should have enforced [Authorize]). Throwing InvalidOperationException should always surface as a 500.
            throw new InvalidOperationException("Authenticated user required but not present in the current context.");
        }

        return await next(cancellationToken).ConfigureAwait(false);
    }
}