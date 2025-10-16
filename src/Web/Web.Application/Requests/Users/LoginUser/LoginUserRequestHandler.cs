// <copyright file="LoginUserRequestHandler.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.LoginUser;

using System.Threading;
using System.Threading.Tasks;
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
    /// The unit of work factory.
    /// </summary>
    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    /// <summary>
    /// The user account service, used to attempt to login.
    /// </summary>
    private readonly IUserAccountService userAccountService;

    /// <summary>
    /// The user account token service, used to generate a <see cref="UserSessionToken"/> and ensure single-session login.
    /// </summary>
    private readonly IUserAccountTokenService userAccountTokenService;

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
    /// <param name="userAccountService">
    /// The user account service, used to attempt to login.
    /// </param>
    /// <param name="userAccountTokenService">
    /// The user account token service, used to generate a <see cref="UserSessionToken"/> and ensure single-session login.
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
    ///   <item><description><paramref name="userAccountService"/></description></item>
    ///   <item><description><paramref name="userAccountTokenService"/></description></item>
    ///   <item><description><paramref name="unitOfWorkFactory"/></description></item>
    ///   <item><description><paramref name="userSessionTokenRepository"/></description></item>
    /// </list>
    /// </exception>
    public LoginUserRequestHandler(
        ILogger<LoginUserRequestHandler> logger,
        IOptions<JwtOptions> options,
        IUserAccountService userAccountService,
        IUserAccountTokenService userAccountTokenService,
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserSessionTokenRepository userSessionTokenRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.userAccountService = userAccountService ?? throw new ArgumentNullException(nameof(userAccountService));
        this.userAccountTokenService = userAccountTokenService ?? throw new ArgumentNullException(nameof(userAccountTokenService));
        this.unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        this.userSessionTokenRepository = userSessionTokenRepository ?? throw new ArgumentNullException(nameof(userSessionTokenRepository));
    }

    /// <inheritdoc/>
    public async Task<LoginUserResponse> Handle(LoginUserRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        string username = request.Username;
        string password = request.Password;

        this.logger.LogInformation("Handling login request for user: '{Username}'...", username);

        var userAccount = await this.userAccountService.LoginUserAsync(
            username: username,
            password: password)
            .ConfigureAwait(false);

        var work = this.unitOfWorkFactory.CreateUnitOfWork();
        var activeSession = await this.userSessionTokenRepository.GetActiveSessionAsync(userAccount.Id, cancellationToken).ConfigureAwait(false);

        // If there's currently an active sesion, reject the login.
        if (activeSession != null)
        {
            this.logger.LogInformation("Session already active for user: '{Username}'", userAccount.UserName);
            throw new UnauthorizedException("You must logout of your current session first.");
        }

        var parameters = new JwtParameters(
            UserAccountId: userAccount.Id,
            Username: userAccount.UserName!);

        var jwt = this.userAccountTokenService.GenerateJwt(parameters);

        var userSessionToken = new UserSessionToken()
        {
            UserAccount = userAccount,
            HashedRefreshToken = this.userAccountTokenService.HashSecureToken(jwt.RefreshToken),
            ExpirationDate = DateTime.UtcNow.AddMinutes(this.options.Value.AccessTokenExpiryMinutes),
            StartDate = DateTime.UtcNow,
        };

        try
        {
            await this.userSessionTokenRepository.AddAsync(userSessionToken, cancellationToken).ConfigureAwait(false);
            await work.SaveAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DatabaseUpdateConcurrencyException ex)
        {
            // There's really no need for concurrency handling, but a DB failure will bubble up as a 500 without context.
            // So let's just be extra safe here.
            this.logger.LogError(ex, "Failed to persist login session for {Username}", username);
            throw new UnauthorizedException("Login failed due to a system error, please try again in a few moments.", ex);
        }

        this.logger.LogInformation("Login succeeded for user: {Username}", request.Username);

        return new LoginUserResponse(
            AccessToken: jwt.AccessToken,
            RefreshToken: jwt.RefreshToken);
    }
}
