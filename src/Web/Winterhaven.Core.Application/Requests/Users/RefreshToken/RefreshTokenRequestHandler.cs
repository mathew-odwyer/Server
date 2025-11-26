// <copyright file="RefreshTokenRequestHandler.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Requests.Users.RefreshToken;

using System.Threading;
using System.Threading.Tasks;
using Winterhaven.Core.Application.Contexts.Users;
using Winterhaven.Core.Application.Exceptions;
using Winterhaven.Core.Application.Exceptions.Database;
using Winterhaven.Core.Application.Options.Security;
using Winterhaven.Core.Application.Services.Security;
using Winterhaven.Core.Application.Work;
using Winterhaven.Core.Application.Work.Users;
using Winterhaven.Core.Domain.Entities.Users;

using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// Provides a request handler used used to refresh the JSON Web Token for the current <see cref="UserAccount"/>.
/// </summary>
public sealed class RefreshTokenRequestHandler : IRequestHandler<RefreshTokenRequest, RefreshTokenResponse>
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<RefreshTokenRequestHandler> logger;

    /// <summary>
    /// The options, used when refreshing the JSON Web Token.
    /// </summary>
    private readonly IOptions<JwtOptions> options;

    /// <summary>
    /// The secure token factory, used to generate a new JWT.
    /// </summary>
    private readonly ISecureTokenFactory secureTokenFactory;

    /// <summary>
    /// The secure token hasher, used to verify the current session.
    /// </summary>
    private readonly ISecureTokenHasher secureTokenHasher;

    /// <summary>
    /// The unit of work factory.
    /// </summary>
    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    /// <summary>
    /// The user account context, used to fetch the currently authenticated user.
    /// </summary>
    private readonly IUserAccountContext userAccountContext;

    /// <summary>
    /// The user session token repository, used to store the currently active session for the <see cref="UserAccount"/>.
    /// </summary>
    private readonly IUserSessionTokenRepository userSessionTokenRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenRequestHandler"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger.
    /// </param>
    /// <param name="options">
    /// The options, used when refreshing the JSON Web Token.
    /// </param>
    /// <param name="secureTokenFactory">
    /// The secure token factory used to generate a new JWT.
    /// </param>
    /// <param name="secureTokenHasher">
    /// The secure token hasher used to verify the current session.
    /// </param>
    /// <param name="unitOfWorkFactory">
    /// The unit of work factory.
    /// </param>
    /// <param name="userSessionTokenRepository">
    /// The user session token repository, used to store the currently active session for the <see cref="UserAccount"/>.
    /// </param>
    /// <param name="userAccountContext">
    /// The user account context, used to fetch the currently authenticated user.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when one of the following parameters is <c>null</c>:
    /// <list type="bullet">
    ///   <item><description><paramref name="logger"/></description></item>
    ///   <item><description><paramref name="options"/></description></item>
    ///   <item><description><paramref name="secureTokenFactory"/></description></item>
    ///   <item><description><paramref name="secureTokenHasher"/></description></item>
    ///   <item><description><paramref name="unitOfWorkFactory"/></description></item>
    ///   <item><description><paramref name="userSessionTokenRepository"/></description></item>
    ///   <item><description><paramref name="userAccountContext"/></description></item>
    /// </list>
    /// </exception>
    public RefreshTokenRequestHandler(
        ILogger<RefreshTokenRequestHandler> logger,
        IOptions<JwtOptions> options,
        ISecureTokenFactory secureTokenFactory,
        ISecureTokenHasher secureTokenHasher,
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserSessionTokenRepository userSessionTokenRepository,
        IUserAccountContext userAccountContext)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.secureTokenFactory = secureTokenFactory ?? throw new ArgumentNullException(nameof(secureTokenFactory));
        this.secureTokenHasher = secureTokenHasher ?? throw new ArgumentNullException(nameof(secureTokenHasher));
        this.unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        this.userSessionTokenRepository = userSessionTokenRepository ?? throw new ArgumentNullException(nameof(userSessionTokenRepository));
        this.userAccountContext = userAccountContext ?? throw new ArgumentNullException(nameof(userAccountContext));
    }

    /// <inheritdoc/>
    public async Task<RefreshTokenResponse> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var userAccount = this.userAccountContext.User;

        var work = this.unitOfWorkFactory.CreateUnitOfWork();
        var activeSession = await this.userSessionTokenRepository.GetActiveSessionAsync(userAccount!.Id, cancellationToken).ConfigureAwait(false);

        if (activeSession == null ||
            activeSession.HashedRefreshToken != this.secureTokenHasher.HashSecureToken(request.RefreshToken) ||
            activeSession.CreatedOn.AddDays(this.options.Value.RefreshTokenExpiryDays) < DateTime.UtcNow)
        {
            this.logger.LogWarning("Invalid or expired resfresh token for user with ID: '{UserAccountId}'", userAccount.Id);
            throw new AuthorizationException("Invalid or expired refresh token.");
        }

        // Expire the old session before creating a new one just to be safe.
        activeSession.IsRevoked = true;

        this.logger.LogInformation("Generating new access and refresh tokens for user with ID: '{UserAccountId}'", userAccount.Id);

        var parameters = new JwtParameters(
            UserAccountId: userAccount.Id,
            Username: userAccount.Username);

        var jwt = this.secureTokenFactory.GenerateJwt(parameters);

        var newSessionToken = new UserSessionToken()
        {
            UserAccount = userAccount,
            HashedRefreshToken = this.secureTokenHasher.HashSecureToken(jwt.RefreshToken),
            ExpirationDate = DateTime.UtcNow.AddMinutes(this.options.Value.AccessTokenExpiryMinutes),
        };

        try
        {
            await this.userSessionTokenRepository.AddAsync(newSessionToken, cancellationToken).ConfigureAwait(false);
            await work.SaveAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DatabaseUpdateException ex)
        {
            this.logger.LogError(ex, "Failed to refresh token for user with ID: '{UserAccountId}'", userAccount.Id);
            throw new AuthorizationException("Invalid or expired refresh token.");
        }

        this.logger.LogInformation("Refreshed JWT for user with ID: '{UserAccountId}'.", userAccount.Id);

        return new RefreshTokenResponse(
            AccessToken: jwt.AccessToken,
            RefreshToken: jwt.RefreshToken,
            ExpirationSeconds: newSessionToken.ExpirationDate.Subtract(DateTime.UtcNow).TotalSeconds);
    }
}
