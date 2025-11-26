// <copyright file="LoginUserRequestHandler.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Requests.Users.LoginUser;

using System.Threading;
using System.Threading.Tasks;
using Winterhaven.Core.Application.Exceptions;
using Winterhaven.Core.Application.Exceptions.Database;
using Winterhaven.Core.Application.Options.Security;
using Winterhaven.Core.Application.Services.Security;
using Winterhaven.Core.Application.Services.Users;
using Winterhaven.Core.Application.Work;
using Winterhaven.Core.Application.Work.Users;
using Winterhaven.Core.Domain.Entities.Users;

using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

/// <summary>
/// Provides a request handler used used to authenticate and enforce single-session login for a potential <see cref="UserAccount"/>.
/// </summary>
public sealed class LoginUserRequestHandler : IRequestHandler<LoginUserRequest, LoginUserResponse>
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<LoginUserRequestHandler> logger;

    /// <summary>
    /// The JWT options used when generating a JSON Web Token.
    /// </summary>
    private readonly IOptions<JwtOptions> options;

    /// <summary>
    /// The secure token factory, used to generate the JWT.
    /// </summary>
    private readonly ISecureTokenFactory secureTokenFactory;

    /// <summary>
    /// The secure token hasher, used when storing refresh token.
    /// </summary>
    private readonly ISecureTokenHasher secureTokenHasher;

    /// <summary>
    /// The unit of work factory.
    /// </summary>
    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    /// <summary>
    /// The user authenticator, used to authenticate the user.
    /// </summary>
    private readonly IUserAuthenticator userAuthenticator;

    /// <summary>
    /// The user session token repository, used to add a <see cref="UserSessionToken"/> when the login is successful.
    /// </summary>
    private readonly IUserSessionTokenRepository userSessionTokenRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginUserRequestHandler"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger.
    /// </param>
    /// <param name="options">
    /// The JWT options used when generating a JSON Web Token.
    /// </param>
    /// <param name="userAuthenticator">
    /// The user authenticator, used to authenticate the user.
    /// </param>
    /// <param name="secureTokenFactory">
    /// The secure token factory used to generate the JWT.
    /// </param>
    /// <param name="secureTokenHasher">
    /// The secure token hasher used to hash the refresh token before storing it in the database.
    /// </param>
    /// <param name="unitOfWorkFactory">
    /// The unit of work factory.
    /// </param>
    /// <param name="userSessionTokenRepository">
    /// The user session token repository, used to add a <see cref="UserSessionToken"/> when the login is successful.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when one of the following parameters is <c>null</c>:
    /// <list type="bullet">
    ///   <item><description><paramref name="logger"/></description></item>
    ///   <item><description><paramref name="options"/></description></item>
    ///   <item><description><paramref name="userAuthenticator"/></description></item>
    ///   <item><description><paramref name="secureTokenFactory"/></description></item>
    ///   <item><description><paramref name="secureTokenHasher"/></description></item>
    ///   <item><description><paramref name="unitOfWorkFactory"/></description></item>
    ///   <item><description><paramref name="userSessionTokenRepository"/></description></item>
    /// </list>
    /// </exception>
    public LoginUserRequestHandler(
        ILogger<LoginUserRequestHandler> logger,
        IOptions<JwtOptions> options,
        IUserAuthenticator userAuthenticator,
        ISecureTokenFactory secureTokenFactory,
        ISecureTokenHasher secureTokenHasher,
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserSessionTokenRepository userSessionTokenRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.userAuthenticator = userAuthenticator ?? throw new ArgumentNullException(nameof(userAuthenticator));
        this.secureTokenFactory = secureTokenFactory ?? throw new ArgumentNullException(nameof(secureTokenFactory));
        this.secureTokenHasher = secureTokenHasher ?? throw new ArgumentNullException(nameof(secureTokenHasher));
        this.unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        this.userSessionTokenRepository = userSessionTokenRepository ?? throw new ArgumentNullException(nameof(userSessionTokenRepository));
    }

    /// <inheritdoc/>
    public async Task<LoginUserResponse> Handle(LoginUserRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        string username = request.Username;
        string password = request.Password;

        this.logger.LogInformation("Handling login request for user: '{Username}'...", username);

        var userAccount = await this.userAuthenticator.LoginUserAsync(
            username: username,
            password: password)
            .ConfigureAwait(false);

        var work = this.unitOfWorkFactory.CreateUnitOfWork();
        var activeSession = await this.userSessionTokenRepository.GetActiveSessionAsync(userAccount.Id, cancellationToken).ConfigureAwait(false);

        // If there's currently an active sesion, reject the login.
        if (activeSession != null)
        {
            this.logger.LogInformation("Session already active for user: '{Username}'", userAccount.Username);
            throw new AuthorizationException("You must logout of your current session first.");
        }

        var parameters = new JwtParameters(
          UserAccountId: userAccount.Id,
          Username: userAccount.Username);

        var jwt = this.secureTokenFactory.GenerateJwt(parameters);

        var userSessionToken = new UserSessionToken()
        {
            UserAccount = userAccount,
            HashedRefreshToken = this.secureTokenHasher.HashSecureToken(jwt.RefreshToken),
            ExpirationDate = DateTime.UtcNow.AddMinutes(this.options.Value.AccessTokenExpiryMinutes),
        };

        try
        {
            await this.userSessionTokenRepository.AddAsync(userSessionToken, cancellationToken).ConfigureAwait(false);
            await work.SaveAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DatabaseUpdateException ex)
        {
            // There's really no need for concurrency handling, but a DB failure will bubble up as a 500 without context.
            // So let's just be extra safe here.
            this.logger.LogError(ex, "Failed to persist login session for {Username}", username);
            throw new AuthorizationException("Login failed due to a system error, please try again in a few moments.", ex);
        }

        this.logger.LogInformation("Login succeeded for user: {Username}", request.Username);

        return new LoginUserResponse(
            AccessToken: jwt.AccessToken,
            RefreshToken: jwt.RefreshToken,
            ExpirationSeconds: userSessionToken.ExpirationDate.Subtract(DateTime.UtcNow).TotalSeconds);
    }
}
