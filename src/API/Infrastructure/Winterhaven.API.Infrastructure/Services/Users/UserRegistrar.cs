namespace Winterhaven.API.Infrastructure.Services.Users;

using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;
using Winterhaven.API.Core.Application.Services.Users;
using Winterhaven.API.Core.Domain.Entities.Players;
using Winterhaven.API.Core.Domain.Entities.Users;
using Winterhaven.API.Core.Domain.Exceptions;

[ExcludeFromCodeCoverage]
internal sealed class UserRegistrar : IUserRegistrar
{
    private readonly ILogger<UserRegistrar> logger;

    private readonly UserManager<IdentityUser<Guid>> userManager;

    public UserRegistrar(
        ILogger<UserRegistrar> logger,
        UserManager<IdentityUser<Guid>> userManager)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
    }

    public async Task<UserAccount> RegisterUserAsync(string emailAddress, string username, string password)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(emailAddress);
        ArgumentException.ThrowIfNullOrWhiteSpace(username);
        ArgumentException.ThrowIfNullOrWhiteSpace(password);

        this.logger.LogInformation("Attempting to register user with username: '{Username}'", username);

        var identityUser = new IdentityUser<Guid>
        {
            UserName = username,
            Email = emailAddress,
        };

        var result = await this.userManager.CreateAsync(identityUser, password).ConfigureAwait(false);

        if (!result.Succeeded)
        {
            var errors = result.Errors
                .GroupBy(x => x.Code)
                .ToDictionary(
                    x => x.Key,
                    x => x.Where(e => !string.IsNullOrWhiteSpace(e.Description))
                          .Select(e => e.Description)
                          .ToArray()
                );

            string message = string.Join("; ", errors.Select(kvp => $"{kvp.Key}: {string.Join(", ", kvp.Value)}"));
            this.logger.LogWarning("Failed to register user with username: '{Username}'. Errors: {Error}", username, message ?? "Unknown error");

            throw new ValidationException(errors);
        }

        var userAccount = new UserAccount()
        {
            Id = identityUser.Id,
            Username = identityUser.NormalizedUserName!,
            EmailAddress = identityUser.NormalizedEmail!.ToUpperInvariant(),

            Player = new Player()
            {
                Id = identityUser.Id,
                Name = identityUser.UserName!,
            },
        };

        this.logger.LogInformation("Successfully registered user with username: '{Username}'", username);

        return userAccount;
    }
}