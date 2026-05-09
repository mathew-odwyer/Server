namespace Winterhaven.API.Core.Application.Behaviours;

using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Core.Application.Contexts.Users;
using Winterhaven.API.Core.Domain.Attributes.Users;

[ExcludeFromCodeCoverage]
public sealed class AuthorizationBehahviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<AuthorizationBehahviour<TRequest, TResponse>> logger;

    private readonly IActorContext actorContext;

    public AuthorizationBehahviour(ILogger<AuthorizationBehahviour<TRequest, TResponse>> logger, IActorContext actorContext)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.actorContext = actorContext ?? throw new ArgumentNullException(nameof(actorContext));
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);

        var type = typeof(TRequest);
        bool isRequired = type.IsDefined(typeof(AuthorizeAttribute), inherit: true);

        // Determine whether this request type requires an authenticated user.
        if (isRequired && this.actorContext.Actor == null)
        {
            this.logger.LogError("Authenticated user required for request {RequestType} but no user was present.", type.FullName);

            // This is an unexpected server/configuration issue (controller should have enforced
            // [Authorize]). Throwing InvalidOperationException should always surface as a 500.
            throw new InvalidOperationException("Authenticated user required but not present in the current context.");
        }

        return await next(cancellationToken).ConfigureAwait(false);
    }
}