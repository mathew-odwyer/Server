namespace Winterhaven.API.Core.Application.Requests.Users.RefreshToken;

using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Core.Application.Contexts.Users;
using Winterhaven.API.Core.Application.Services.Security;
using Winterhaven.API.Core.Application.Work;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Users;
using Winterhaven.Common.Exceptions;

/// <summary>
/// Provides a request handler used to refresh the JSON Web Token for the current <see cref="UserAccount"/>.
/// </summary>
public sealed class RefreshTokenRequestHandler : IRequestHandler<RefreshTokenRequest, RefreshTokenResponse>
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<RefreshTokenRequestHandler> logger;

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
    /// The actor context, used to fetch the currently authenticated actor.
    /// </summary>
    private readonly IActorContext actorContext;

    /// <summary>
    /// The user session token repository, used to store the currently active session for the <see cref="UserAccount"/>.
    /// </summary>
    private readonly IUserSessionTokenRepository userSessionTokenRepository;

    /// <summary>
    /// The user account repository, used to fetch the user account (if any) linked to the actor.
    /// </summary>
    private readonly IUserAccountRepository userAccountRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenRequestHandler"/> class.
    /// </summary>
    /// <param name="logger">The logger.</param>
    /// <param name="secureTokenFactory">The secure token factory used to generate a new JWT.</param>
    /// <param name="secureTokenHasher">The secure token hasher used to verify the current session.</param>
    /// <param name="unitOfWorkFactory">The unit of work factory.</param>
    /// <param name="userSessionTokenRepository">
    /// The user session token repository, used to store the currently active session for the <see cref="UserAccount"/>.
    /// </param>
    /// <param name="actorContext">
    /// The user account context, used to fetch the currently authenticated user.
    /// </param>
    /// <param name="userAccountRepository"></param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when one of the following parameters is <c>null</c>:
    /// <list type="bullet">
    /// <item>
    /// <description><paramref name="logger"/></description>
    /// </item>
    /// <item>
    /// <description><paramref name="secureTokenFactory"/></description>
    /// </item>
    /// <item>
    /// <description><paramref name="secureTokenHasher"/></description>
    /// </item>
    /// <item>
    /// <description><paramref name="unitOfWorkFactory"/></description>
    /// </item>
    /// <item>
    /// <description><paramref name="userSessionTokenRepository"/></description>
    /// </item>
    /// <item>
    /// <description><paramref name="actorContext"/></description>
    /// </item>
    /// <item>
    /// <description><paramref name="userAccountRepository"/></description>
    /// </item>
    /// </list>
    /// </exception>
    public RefreshTokenRequestHandler(
        ILogger<RefreshTokenRequestHandler> logger,
        ISecureTokenFactory secureTokenFactory,
        ISecureTokenHasher secureTokenHasher,
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserSessionTokenRepository userSessionTokenRepository,
        IActorContext actorContext,
        IUserAccountRepository userAccountRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.secureTokenFactory = secureTokenFactory ?? throw new ArgumentNullException(nameof(secureTokenFactory));
        this.secureTokenHasher = secureTokenHasher ?? throw new ArgumentNullException(nameof(secureTokenHasher));
        this.unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        this.userSessionTokenRepository = userSessionTokenRepository ?? throw new ArgumentNullException(nameof(userSessionTokenRepository));
        this.actorContext = actorContext ?? throw new ArgumentNullException(nameof(actorContext));
        this.userAccountRepository = userAccountRepository ?? throw new ArgumentNullException(nameof(userAccountRepository));
    }

    /// <inheritdoc/>
    public async Task<RefreshTokenResponse> Handle(RefreshTokenRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var actor = this.actorContext.Actor;
        var userAccount = await this.userAccountRepository.GetByIdAsync(actor.Id, cancellationToken).ConfigureAwait(false)
            ?? throw new ResourceNotFoundException(nameof(UserAccount), actor.Id);

        var work = this.unitOfWorkFactory.CreateUnitOfWork();
        var activeSession = await this.userSessionTokenRepository.GetActiveSessionAsync(userAccount!.Id, cancellationToken).ConfigureAwait(false);

        if (activeSession == null ||
            activeSession.HashedRefreshToken != this.secureTokenHasher.HashSecureToken(request.RefreshToken) ||
            activeSession.RefreshTokenExpirationDate < DateTime.UtcNow)
        {
            this.logger.LogWarning("Invalid or expired refresh token for user with ID: '{UserAccountId}'", userAccount.Id);
            throw new AuthorizationException("Invalid or expired refresh token.");
        }

        // Expire the old session before creating a new one just to be safe.
        activeSession.IsRevoked = true;

        this.logger.LogInformation("Generating new access and refresh tokens for user with ID: '{UserAccountId}'", userAccount.Id);

        var parameters = new UserTokenParameters(
            UserAccountId: userAccount.Id,
            Username: userAccount.Username);

        var userToken = this.secureTokenFactory.GenerateUserToken(parameters);

        var newSessionToken = new UserSessionToken()
        {
            UserAccount = userAccount,
            HashedRefreshToken = this.secureTokenHasher.HashSecureToken(userToken.RefreshToken),
            AccessTokenExpirationDate = userToken.AccessTokenExpiryDate,
            RefreshTokenExpirationDate = userToken.RefreshTokenExpiryDate,
        };

        try
        {
            await this.userSessionTokenRepository.AddAsync(newSessionToken, cancellationToken).ConfigureAwait(false);
            await work.SaveAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (EntityPersistenceException ex)
        {
            this.logger.LogError(ex, "Failed to refresh token for user with ID: '{UserAccountId}'", userAccount.Id);
            throw new AuthorizationException("Invalid or expired refresh token.");
        }

        this.logger.LogInformation("Refreshed JWT for user with ID: '{UserAccountId}'.", userAccount.Id);

        return new RefreshTokenResponse(
            AccessToken: userToken.AccessToken,
            RefreshToken: userToken.RefreshToken,
            ExpirationSeconds: newSessionToken.AccessTokenExpirationDate.Subtract(DateTime.UtcNow).TotalSeconds);
    }
}