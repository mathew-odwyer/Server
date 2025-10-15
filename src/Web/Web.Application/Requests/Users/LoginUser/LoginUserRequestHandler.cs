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
    /// The client token options, used to configure the client token expiriration time.
    /// </summary>
    private readonly IOptions<ClientTokenOptions> options;

    /// <summary>
    /// The unit of work factory.
    /// </summary>
    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    /// <summary>
    /// The user account service, used to attempt to authenticate.
    /// </summary>
    private readonly IUserAccountService userAccountService;

    /// <summary>
    /// The user account token service, used to generate a short-lived client token.
    /// </summary>
    private readonly IUserAccountTokenService userAccountTokenService;

    /// <summary>
    /// The user client token repository, used to persist the client token.
    /// </summary>
    private readonly IUserClientTokenRepository userClientTokenRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="LoginUserRequestHandler"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger.
    /// </param>
    /// <param name="userAccountService">
    /// The user account service, used to attempt to login.
    /// </param>
    /// <param name="options">
    /// The client token options, used to configure the client token expiriration time.
    /// </param>
    /// <param name="unitOfWorkFactory">
    /// The unit of work factory.
    /// </param>
    /// <param name="userAccountTokenService">
    /// The user account token service, used to generate a short-lived client token.
    /// </param>
    /// <param name="userClientTokenRepository">
    /// The user client token repository, used to persist the client token.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown when one of the following parameters is <c>null</c>:
    /// <list type="bullet">
    ///   <item><description><paramref name="logger"/></description></item>
    ///   <item><description><paramref name="options"/></description></item>
    ///   <item><description><paramref name="userAccountService"/></description></item>
    ///   <item><description><paramref name="unitOfWorkFactory"/></description></item>
    ///   <item><description><paramref name="userAccountTokenService"/></description></item>
    ///   <item><description><paramref name="userClientTokenRepository"/></description></item>
    /// </list>
    /// </exception>
    public LoginUserRequestHandler(
        ILogger<LoginUserRequestHandler> logger,
        IOptions<ClientTokenOptions> options,
        IUserAccountService userAccountService,
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserAccountTokenService userAccountTokenService,
        IUserClientTokenRepository userClientTokenRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.options = options ?? throw new ArgumentNullException(nameof(options));
        this.userAccountService = userAccountService ?? throw new ArgumentNullException(nameof(userAccountService));
        this.unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        this.userAccountTokenService = userAccountTokenService ?? throw new ArgumentNullException(nameof(userAccountTokenService));
        this.userClientTokenRepository = userClientTokenRepository ?? throw new ArgumentNullException(nameof(userClientTokenRepository));
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
        string token = this.userAccountTokenService.GenerateSecureToken();

        var userClientToken = new UserClientToken()
        {
            UserAccountId = userAccount.Id,
            HashedToken = this.userAccountTokenService.HashSecureToken(token),
            ExpirationDate = DateTime.UtcNow.AddSeconds(this.options.Value.ClientTokenExpirySeconds),
        };

        try
        {
            await this.userClientTokenRepository.AddAsync(userClientToken, cancellationToken).ConfigureAwait(false);
            await work.SaveAsync(cancellationToken).ConfigureAwait(false);
        }
        catch (DatabaseUpdateConcurrencyException ex)
        {
            // There's really no need for concurrency handling, but a DB failure will bubble up as a 500 without context.
            // So let's just be extra safe here.
            this.logger.LogError(ex, "Failed to persist client token for {Username}", username);
            throw new UnauthorizedException("Login failed due to a system error, please try again in a few moments.", ex);
        }

        this.logger.LogInformation("Login succeeded for user: {Username}", request.Username);

        return new LoginUserResponse(
            ClientToken: token);
    }
}
