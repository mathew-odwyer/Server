// <copyright file="ValidateUserRequestHandler.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.ValidateUser;

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

public sealed class ValidateUserRequestHandler : IRequestHandler<ValidateUserRequest, ValidateUserResponse>
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<ValidateUserRequestHandler> logger;

    /// <summary>
    /// The JWT options used when generating a JSON Web Token.
    /// </summary>
    private readonly IOptions<JwtOptions> options;

    /// <summary>
    /// The unit of work factory.
    /// </summary>
    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    /// <summary>
    /// The user account repository, used to fetch the <see cref="UserAccount"/> when validating the user.
    /// </summary>
    private readonly IUserAccountRepository userAccountRepository;

    /// <summary>
    /// The user account token service, used to generate a <see cref="UserSessionToken"/> and ensure single-session login.
    /// </summary>
    private readonly IUserAccountTokenService userAccountTokenService;

    /// <summary>
    /// The user client token repository, used to fetch the client token.
    /// </summary>
    private readonly IUserClientTokenRepository userClientTokenRepository;

    /// <summary>
    /// The user session token repository, used to add a <see cref="UserSessionToken"/> when the login is successful.
    /// </summary>
    private readonly IUserSessionTokenRepository userSessionTokenRepository;

    public ValidateUserRequestHandler(
        ILogger<ValidateUserRequestHandler> logger,
        IOptions<JwtOptions> options,
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserAccountTokenService userAccountTokenService,
        IUserSessionTokenRepository userSessionTokenRepository,
        IUserClientTokenRepository userClientTokenRepository,
        IUserAccountRepository userAccountRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        this.userAccountTokenService = userAccountTokenService ?? throw new ArgumentNullException(nameof(userAccountTokenService));
        this.userSessionTokenRepository = userSessionTokenRepository ?? throw new ArgumentNullException(nameof(userSessionTokenRepository));
        this.userClientTokenRepository = userClientTokenRepository ?? throw new ArgumentNullException(nameof(userClientTokenRepository));
        this.userAccountRepository = userAccountRepository ?? throw new ArgumentNullException(nameof(userAccountRepository));
    }

    public async Task<ValidateUserResponse> Handle(ValidateUserRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        var work = this.unitOfWorkFactory.CreateUnitOfWork();
        var userClientToken = await this.userClientTokenRepository.GetByHashedToken(this.userAccountTokenService.HashSecureToken(request.ClientToken), cancellationToken).ConfigureAwait(false);

        if (userClientToken == null)
        {
            this.logger.LogWarning("The provided client token is invalid.");
            throw new UnauthorizedException("Invalid or expired client token.");
        }

        var userAccount = await this.userAccountRepository.GetByIdAsync(userClientToken.UserAccountId, cancellationToken).ConfigureAwait(false);

        if (userAccount == null)
        {
            this.logger.LogError("The user account with ID '{UserAccountId}' associated with the client token '{ClientTokenId}' was not found.", userClientToken.UserAccountId, userClientToken.Id);
            throw new UnauthorizedException("Invalid or expired client token.");
        }

        // From here the token is valid, we need to revoke it.
        userClientToken.IsRevoked = true;

        var activeSession = await this.userSessionTokenRepository.GetActiveSessionAsync(userAccount.Id, cancellationToken).ConfigureAwait(false);

        // If there's currently an active sesion, reject the validation.
        if (activeSession != null)
        {
            this.logger.LogInformation("Session already active for user with ID: '{UserAccountId}'", userClientToken.UserAccountId);
            throw new UnauthorizedException("You must logout of your current session first.");
        }

        this.logger.LogInformation("Client token validated for user with ID: '{UserAccountId}'", userClientToken.UserAccountId);

        var parameters = new JwtParameters(
            UserAccountId: userAccount.Id,
            Username: userAccount.UserName!);

        var jwt = this.userAccountTokenService.GenerateJwt(parameters);

        this.logger.LogInformation("Creating new session for user with ID: '{UserAccountId}'", userClientToken.UserAccountId);

        var userSessionToken = new UserSessionToken()
        {
            UserAccountId = userAccount.Id,
            HashedRefreshToken = this.userAccountTokenService.HashSecureToken(jwt.RefreshToken),
            ExpirationDate = DateTime.UtcNow.AddMinutes(this.options.Value.AccessTokenExpiryMinutes),
            SessionId = jwt.SessionId,
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
            this.logger.LogError(ex, "Failed to persist login session for {Username}", userAccount.UserName);
            throw new UnauthorizedException("Login failed due to a system error, please try again in a few moments.", ex);
        }

        this.logger.LogInformation("User with ID: '{UserAccountId}' has validated successfully.", userClientToken.UserAccountId);

        return new ValidateUserResponse(
            AccessToken: jwt.AccessToken,
            RefreshToken: jwt.RefreshToken);
    }
}
