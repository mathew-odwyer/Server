// <copyright file="UserAccountService.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Infrastructure.Services.Users;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Web.Application.Exceptions;
using Web.Application.Services.Users;
using Web.Domain.Entities.Users;

internal sealed class UserAccountService : IUserAccountService
{
    private readonly ILogger<UserAccountService> logger;

    private readonly SignInManager<UserAccount> signInManager;

    private readonly UserManager<UserAccount> userManager;

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
            throw new BadRequestException(errors);
        }

        this.logger.LogInformation("Successfully registered user with username: '{Username}'", username);
        return userAccount;
    }
}
