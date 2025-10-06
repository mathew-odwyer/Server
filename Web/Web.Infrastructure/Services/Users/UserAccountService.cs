// <copyright file="UserAccountService.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Infrastructure.Services.Users;

using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Web.Application.Exceptions;
using Web.Application.Services.Users;
using Web.Domain.Entities.Users;

/// <summary>
/// Provides an implementation of <see cref="IUserAccountService"/> for managing user accounts using ASP.NET Core Identity.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class UserAccountService : IUserAccountService
{
    /// <summary>
    /// The logger used for recording events and errors.
    /// </summary>
    private readonly ILogger<UserAccountService> logger;

    /// <summary>
    /// The ASP.NET Core Identity sign-in manager for handling user logins.
    /// </summary>
    private readonly SignInManager<UserAccount> signInManager;

    /// <summary>
    /// The ASP.NET Core Identity user manager for handling user persistence and validation.
    /// </summary>
    private readonly UserManager<UserAccount> userManager;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserAccountService"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger used for recording events and errors.
    /// </param>
    /// <param name="userManager">
    /// The ASP.NET Core Identity user manager for handling user persistence and validation.
    /// </param>
    /// <param name="signInManager"></param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="logger"/> or <paramref name="userManager"/> is <c>null</c>.
    /// </exception>
    public UserAccountService(
        ILogger<UserAccountService> logger,
        UserManager<UserAccount> userManager,
        SignInManager<UserAccount> signInManager)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        this.signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
    }

    public async Task<UserAccount> LoginUserAsync(string username, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        this.logger.LogInformation("User attempting to login with username: '{Username}'", username);

        var userAccount = await this.userManager.FindByNameAsync(username).ConfigureAwait(false);

        if (userAccount == null)
        {
            this.logger.LogWarning("Login failed: user not found for user: '{Username}'", username);
            throw new UnauthorizedException("Invalid credentials. Please check your details and try again.");
        }

        var result = await this.signInManager.CheckPasswordSignInAsync(userAccount, password, false).ConfigureAwait(false);

        if (!result.Succeeded)
        {
            this.logger.LogWarning("Login failed: invalid password for user: '{Username}'", username);
            throw new UnauthorizedException("Invalid credentials. Please check your details and try again.");
        }

        this.logger.LogInformation("Login succeeded for user: '{Username}'", username);
        return userAccount;
    }

    /// <inheritdoc/>
    public async Task<UserAccount> RegisterUserAsync(string emailAddress, string username, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(emailAddress);
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        this.logger.LogInformation("Attempting to register user with username: '{Username}'", username);

        var userAccount = new UserAccount
        {
            UserName = username,
            Email = emailAddress,
            EmailConfirmed = false,
        };

        var result = await this.userManager.CreateAsync(userAccount, password).ConfigureAwait(false);

        if (!result.Succeeded)
        {
            string errors = string.Join("\n", result.Errors.Select(x => x.Description));

            this.logger.LogWarning("Failed to register user with username: '{Username}': {Error}", username, errors);
            throw new ConflictException(result.Errors.ToDictionary(x => x.Code, x => x.Description));
        }

        this.logger.LogInformation("Successfully registered user with username: '{Username}'", username);
        return userAccount;
    }
}
