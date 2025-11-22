// <copyright file="UserRegistrar.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Infrastructure.Services.Users;

using System.Threading.Tasks;
using Winterhaven.Core.Application.Exceptions;
using Winterhaven.Core.Application.Exceptions.Database;
using Winterhaven.Core.Application.Services.Users;
using Winterhaven.Core.Application.Work;
using Winterhaven.Core.Application.Work.Users;
using Winterhaven.Core.Domain.Entities.Players;
using Winterhaven.Core.Domain.Entities.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

internal sealed class UserRegistrar : IUserRegistrar
{
    private readonly ILogger<UserRegistrar> logger;

    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    private readonly IUserAccountRepository userAccountRepository;

    private readonly UserManager<IdentityUser<Guid>> userManager;

    public UserRegistrar(
        ILogger<UserRegistrar> logger,
        UserManager<IdentityUser<Guid>> userManager,
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserAccountRepository userAccountRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        this.unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        this.userAccountRepository = userAccountRepository ?? throw new ArgumentNullException(nameof(userAccountRepository));
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
            var error = result.Errors.FirstOrDefault();

            this.logger.LogWarning("Failed to register user with username: '{Username}'. Error: {Error}", username, error?.Description ?? "Unknown error");
            throw new ValidationException(error?.Description ?? "An unknown error occurred during registration.");
        }

        try
        {
            var work = this.unitOfWorkFactory.CreateUnitOfWork();

            var userAccount = new UserAccount()
            {
                Id = identityUser.Id,
                Username = username.ToUpperInvariant(),
                EmailAddress = emailAddress.ToUpperInvariant(),
                Player = new Player()
                {
                    Name = username,
                    X = 128,
                    Y = 128,
                },
            };

            await this.userAccountRepository.AddAsync(userAccount).ConfigureAwait(false);
            await work.SaveAsync().ConfigureAwait(false);

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
