// <copyright file="RefreshTokenRequestHandler.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.RefreshToken;

using System.Threading;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Web.Application.Contexts;
using Web.Application.Contexts.Users;
using Web.Application.Exceptions;
using Web.Application.Exceptions.Database;
using Web.Application.Options.Security;
using Web.Application.Services.Users;
using Web.Domain.Entities.Users;

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
    /// The unit of work factory.
    /// </summary>
    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    /// <summary>
    /// The user account repository, used to fetch the current <see cref="UserAccount"/>.
    /// </summary>
    private readonly IUserAccountRepository userAccountRepository;

    /// <summary>
    /// The user account token service, used to generate a new JSON Web Token.
    /// </summary>
    private readonly IUserAccountTokenService userAccountTokenService;

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
    /// <param name="userAccountTokenService">
    /// The user account token service, used to generate a new JSON Web Token.
    /// </param>
    /// <param name="unitOfWorkFactory">
    /// The unit of work factory.
    /// </param>
    /// <param name="userAccountRepository">
    /// The user account repository, used to fetch the current <see cref="UserAccount"/>.
    /// </param>
    /// <param name="userSessionTokenRepository">
    /// The user session token repository, used to store the currently active session for the <see cref="UserAccount"/>.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when one of the following parameters is <c>null</c>:
    /// <list type="bullet">
    ///   <item><description><paramref name="logger"/></description></item>
    ///   <item><description><paramref name="options"/></description></item>
    ///   <item><description><paramref name="userAccountTokenService"/></description></item>
    ///   <item><description><paramref name="unitOfWorkFactory"/></description></item>
    ///   <item><description><paramref name="userAccountRepository"/></description></item>
    ///   <item><description><paramref name="userSessionTokenRepository"/></description></item>
    /// </list>
    /// </exception>
    public RefreshTokenRequestHandler(
        ILogger<RefreshTokenRequestHandler> logger,
        IOptions<JwtOptions> options,
        IUserAccountTokenService userAccountTokenService,
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserAccountRepository userAccountRepository,
        IUserSessionTokenRepository userSessionTokenRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.userAccountRepository = userAccountRepository ?? throw new ArgumentNullException(nameof(userAccountRepository));
        this.userAccountTokenService = userAccountTokenService ?? throw new ArgumentNullException(nameof(userAccountTokenService));
        this.unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        this.userSessionTokenRepository = userSessionTokenRepository ?? throw new ArgumentNullException(nameof(userSessionTokenRepository));
    }

    /// <inheritdoc/>
    public async Task<RefreshTokenResponse> Handle(RefreshTokenRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        var work = this.unitOfWorkFactory.CreateUnitOfWork();
        var activeSession = await this.userSessionTokenRepository.GetActiveSessionAsync(request.UserAccountId, cancellationToken).ConfigureAwait(false);

        if (activeSession == null ||
            activeSession.HashedRefreshToken != this.userAccountTokenService.HashRefreshToken(request.RefreshToken) ||
            activeSession.CreatedOn.AddDays(this.options.Value.RefreshTokenExpiryDays) < DateTime.UtcNow)
        {
            this.logger.LogWarning("Invalid or expired resfresh token for user with ID: '{UserAccountId}'", request.UserAccountId);
            throw new UnauthorizedException("Invalid or expired refresh token.");
        }

        // Expire the old session before creating a new one - just to be safe and enforce single-session logins.
        activeSession.ExpirationDate = DateTime.UtcNow;

        this.logger.LogInformation("Generating new access and refresh tokens for user with ID: '{UserAccountId}'", activeSession.UserAccountId);

        var userAccount = await this.userAccountRepository.GetByIdAsync(activeSession.UserAccountId, cancellationToken).ConfigureAwait(false);

        if (userAccount == null)
        {
            this.logger.LogError("Failed to locate user account with ID: '{UserAccountId}'", activeSession.UserAccountId);
            throw new EntityNotFoundException(nameof(UserAccount), activeSession.UserAccountId);
        }

        var parameters = new JwtParameters(
            UserAccountId: userAccount.Id,
            Username: userAccount.UserName!);

        var jwt = this.userAccountTokenService.GenerateJwt(parameters);

        var newSessionToken = new UserSessionToken()
        {
            UserAccountId = userAccount.Id,
            HashedRefreshToken = this.userAccountTokenService.HashRefreshToken(jwt.RefreshToken),
            ExpirationDate = DateTime.UtcNow.AddMinutes(this.options.Value.AccessTokenExpiryMinutes),
            SessionId = jwt.SessionId,
        };

        try
        {
            await this.userSessionTokenRepository.AddAsync(newSessionToken, cancellationToken).ConfigureAwait(false);
            await work.SaveAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DatabaseUpdateConcurrencyException)
        {
            this.logger.LogWarning("Concurrent refresh attempt detected for token {Token}", request.RefreshToken);
            throw new UnauthorizedException("Invalid or expired refresh token.");
        }

        this.logger.LogInformation("Refreshed JWT for user with ID: '{UserAccountId}'.", activeSession.UserAccountId);

        return new RefreshTokenResponse(
            AccessToken: jwt.AccessToken,
            RefreshToken: jwt.RefreshToken);
    }
}
