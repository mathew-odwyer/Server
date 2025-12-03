// <copyright file="RegisterUserRequestHandler.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Requests.Users.RegisterUser;

using System.Threading;
using System.Threading.Tasks;
using Winterhaven.Core.Application.Services.Users;
using MediatR;
using Microsoft.Extensions.Logging;
using Winterhaven.Core.Application.Work;
using Winterhaven.Core.Application.Work.Users;

/// <summary>
/// Handles <see cref="RegisterUserRequest"/> messages to register new user accounts.
/// </summary>
/// <seealso cref="IRequestHandler{TRequest}"/>
public sealed class RegisterUserRequestHandler : IRequestHandler<RegisterUserRequest>
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<RegisterUserRequestHandler> logger;

    /// <summary>
    /// The user registrar, used to register new users.
    /// </summary>
    private readonly IUserRegistrar userRegistrar;

    /// <summary>
    /// The unit of work factory, used to persist the user acccount.
    /// </summary>
    private readonly IUnitOfWorkFactory unitOfWorkFactory;

    /// <summary>
    /// The user account repository, used to add the new user account.
    /// </summary>
    private readonly IUserAccountRepository userAccountRepository;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterUserRequestHandler"/> class.
    /// </summary>
    /// <param name="logger">
    /// Specifies a <see cref="ILogger{TCategoryName}"/> that is used to log messages.
    /// </param>
    /// <param name="userRegistrar">
    /// Specifies a <see cref="IUserRegistrar"/> that is used to register new users.
    /// </param>
    /// <param name="unitOfWorkFactory">
    /// Specifies a <see cref="IUnitOfWorkFactory"/> that is used to persist the new user to the database.
    /// </param>
    /// <param name="userAccountRepository">
    /// Specifies a <see cref="IUserAccountRepository"/> that is used to add the new user to the databse.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="logger"/> or <paramref name="userRegistrar"/> is <c>null</c>.
    /// </exception>
    public RegisterUserRequestHandler(
        ILogger<RegisterUserRequestHandler> logger,
        IUserRegistrar userRegistrar,
        IUnitOfWorkFactory unitOfWorkFactory,
        IUserAccountRepository userAccountRepository)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userRegistrar = userRegistrar ?? throw new ArgumentNullException(nameof(userRegistrar));
        this.unitOfWorkFactory = unitOfWorkFactory ?? throw new ArgumentNullException(nameof(unitOfWorkFactory));
        this.userAccountRepository = userAccountRepository ?? throw new ArgumentNullException(nameof(userAccountRepository));
    }

    /// <inheritdoc/>
    public async Task Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        this.logger.LogInformation("Handling user registration for new user: '{Username}'", request.Username);

        var work = this.unitOfWorkFactory.CreateUnitOfWork();

        var userAccount = await this.userRegistrar.RegisterUserAsync(
            emailAddress: request.EmailAddress,
            username: request.Username,
            password: request.Password)
            .ConfigureAwait(false);

        await this.userAccountRepository.AddAsync(userAccount, cancellationToken).ConfigureAwait(false);
        await work.SaveAsync(cancellationToken).ConfigureAwait(false);

        this.logger.LogInformation("User registration succeeded for username: {Username}", request.Username);
    }
}
