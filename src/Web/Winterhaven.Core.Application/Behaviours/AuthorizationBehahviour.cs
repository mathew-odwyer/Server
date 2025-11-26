// <copyright file="AuthorizationBehahviour.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Behaviours;

using System.Diagnostics.CodeAnalysis;
using Winterhaven.Core.Application.Attributes;
using Winterhaven.Core.Application.Contexts.Users;
using MediatR;
using Microsoft.Extensions.Logging;

[ExcludeFromCodeCoverage]
internal sealed class AuthorizationBehahviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    private readonly ILogger<AuthorizationBehahviour<TRequest, TResponse>> logger;

    private readonly IUserAccountContext userAccountContext;

    public AuthorizationBehahviour(ILogger<AuthorizationBehahviour<TRequest, TResponse>> logger, IUserAccountContext userAccountContext)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userAccountContext = userAccountContext ?? throw new ArgumentNullException(nameof(userAccountContext));
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);
        ArgumentNullException.ThrowIfNull(next);

        var type = typeof(TRequest);
        bool isRequired = type.IsDefined(typeof(AuthorizeAttribute), inherit: true);

        // Determine whether this request type requires an authenticated user.
        if (isRequired && this.userAccountContext.User == null)
        {
            this.logger.LogError("Authenticated user required for request {RequestType} but no user was present.", type.FullName);

            // This is an unexpected server/configuration issue (controller should have enforced [Authorize]).
            // Throwing InvalidOperationException should always surface as a 500.
            throw new InvalidOperationException("Authenticated user required but not present in the current context.");
        }

        return await next(cancellationToken).ConfigureAwait(false);
    }
}
