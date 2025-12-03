// <copyright file="UserRegistrar.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Infrastructure.Services.Users;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Winterhaven.Core.Application.Exceptions;
using Winterhaven.Core.Application.Exceptions.Database;
using Winterhaven.Core.Application.Services.Users;
using Winterhaven.Core.Domain.Entities.Players;
using Winterhaven.Core.Domain.Entities.Users;

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

        try
        {
            var userAccount = new UserAccount()
            {
                Id = identityUser.Id,
                Username = username.ToUpperInvariant(),
                EmailAddress = emailAddress.ToUpperInvariant(),
                Player = new Player()
                {
                    Name = username,
                },
            };

            return userAccount;
        }
        catch (DatabaseUpdateException ex)
        {
            await this.userManager.DeleteAsync(identityUser).ConfigureAwait(false);

            this.logger.LogError(ex, "An error occurred while registering user with username: '{Username}'", username);
            throw new InvalidOperationException("An error occurred while registering the user.", ex);
        }
    }
}
