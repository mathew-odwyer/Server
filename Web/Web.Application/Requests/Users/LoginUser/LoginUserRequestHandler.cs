// <copyright file="LoginUserRequestHandler.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.LoginUser;

using System.Threading;
using System.Threading.Tasks;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Web.Application.Contexts;
using Web.Application.Contexts.Users;
using Web.Application.Exceptions;
using Web.Application.Options.Security;
using Web.Application.Services.Users;
using Web.Domain.Entities.Users;

public sealed class LoginUserRequestHandler : IRequestHandler<LoginUserRequest, Result<LoginUserResponse>>
{
    private readonly ILogger<LoginUserRequestHandler> logger;

    private readonly IOptions<JwtOptions> options;

    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    private readonly IUserAccountService userAccountService;

    private readonly IUserAccountTokenService userAccountTokenService;

    private readonly IUserSessionTokenRepository userSessionTokenRepository;

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

    public async Task<Result<LoginUserResponse>> Handle(LoginUserRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        string username = request.Username;
        string password = request.Password;

        this.logger.LogInformation("Handling login request for user: '{Username}'...", username);

        var result = await this.userAccountService.LoginUserAsync(
            username: username,
            password: password)
            .ConfigureAwait(false);

        if (result.IsFailed)
        {
            string errors = string.Join(", ", result.Errors.Select(x => x.Message));
            this.logger.LogWarning("Login failed for user: '{Username}': {Error}", username, errors);

            return Result.Fail(errors);
        }

        var work = this.unitOfWorkFactory.CreateUnitOfWork();
        var activeSession = await this.userSessionTokenRepository.GetActiveSessionAsync(result.Value.Id, cancellationToken).ConfigureAwait(false);

        // If there's currently an active sesion, reject the login.
        if (activeSession != null)
        {
            this.logger.LogInformation("Session already active for user: '{Username}'", result.Value.UserName);
            return Result.Fail("You must logout of your current session first.");
        }

        var jwt = this.userAccountTokenService.GenerateJwt(result.Value);

        var userSessionToken = new UserSessionToken()
        {
            UserAccount = result.Value,
            HashedRefreshToken = this.userAccountTokenService.HashRefreshToken(jwt.RefreshToken),
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
            this.logger.LogError(ex, "Failed to persist login session for {Username}", username);
            return Result.Fail("Login failed due to a system error. Please try again.");
        }

        this.logger.LogInformation("Login succeeded for user: {Username}", request.Username);

        return Result.Ok(new LoginUserResponse(
            AccessToken: jwt.AccessToken,
            RefreshToken: jwt.RefreshToken));
    }
}
