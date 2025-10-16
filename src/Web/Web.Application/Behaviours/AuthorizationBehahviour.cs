// <copyright file="AuthorizationBehahviour.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Behaviours;

using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.Extensions.Logging;
using Web.Application.Attributes;
using Web.Application.Contexts;

public sealed class AuthorizationBehahviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
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

        // Determine whether this request type requires an authenticated user.
        var type = typeof(TRequest);
        bool isRequired = type.IsDefined(typeof(AuthorizeAttribute), inherit: true);

        if (isRequired && this.userAccountContext.User == null)
        {
            this.logger.LogError("Authenticated user required for request {RequestType} but none was present in the IUserContext.", type.FullName);

            // This is an unexpected server/configuration issue (controller should have enforced [Authorize]).
            // Throwing InvalidOperationException should always surface as a 500.
            throw new InvalidOperationException("Authenticated user required but not present in the current context.");
        }

        return await next(cancellationToken).ConfigureAwait(false);
    }
}
