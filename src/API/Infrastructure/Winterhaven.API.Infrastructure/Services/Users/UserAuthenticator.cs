namespace Winterhaven.API.Infrastructure.Services.Users;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;
using Winterhaven.API.Core.Application.Services.Users;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Users;
using Winterhaven.API.Core.Domain.Exceptions;

[ExcludeFromCodeCoverage]
internal sealed class UserAuthenticator : IUserAuthenticator
{
    private readonly ILogger<UserAuthenticator> logger;

    private readonly SignInManager<IdentityUser<Guid>> signInManager;

    private readonly IUserAccountRepository userAccountRepository;

    private readonly UserManager<IdentityUser<Guid>> userManager;

    public UserAuthenticator(
        ILogger<UserAuthenticator> logger,
        UserManager<IdentityUser<Guid>> userManager,
        SignInManager<IdentityUser<Guid>> signInManager,
        IUserAccountRepository userAccountRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        this.signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        this.userAccountRepository = userAccountRepository ?? throw new ArgumentNullException(nameof(userAccountRepository));
    }

    public async Task<UserAccount> LoginUserAsync(string username, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        this.logger.LogInformation("User attempting to login with username: '{Username}'", username);

        var identityUser = await this.userManager.FindByNameAsync(username).ConfigureAwait(false);

        if (identityUser == null)
        {
            this.logger.LogWarning("Login failed: user not found for user: '{Username}'", username);
            throw new AuthorizationException("Invalid credentials. Please check your details and try again.");
        }

        var result = await this.signInManager.CheckPasswordSignInAsync(identityUser, password, false).ConfigureAwait(false);

        if (!result.Succeeded)
        {
            this.logger.LogWarning("Login failed: invalid password for user: '{Username}'", username);
            throw new AuthorizationException("Invalid credentials. Please check your details and try again.");
        }

        var userAccount = await this.userAccountRepository.GetByIdAsync(identityUser.Id).ConfigureAwait(false);

        if (userAccount == null)
        {
            this.logger.LogError("Failed to locate {UserAccount} with ID: '{UserAccountId}'", nameof(UserAccount), identityUser.Id);
            throw new AuthorizationException("Invalid Credentials. Please check your details and try again.");
        }

        this.logger.LogInformation("Login succeeded for user: '{Username}'", username);

        return userAccount;
    }
}