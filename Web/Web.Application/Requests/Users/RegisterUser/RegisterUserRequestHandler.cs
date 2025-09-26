// <copyright file="RegisterUserRequestHandler.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.RegisterUser;

using System.Threading;
using FluentResults;
using MediatR;
using Microsoft.Extensions.Logging;
using Web.Application.Services.Users;

/// <summary>
/// Handles <see cref="RegisterUserRequest"/> messages to register new user accounts.
/// </summary>
/// <remarks>
/// Uses <see cref="IUserAccountService"/> to perform registration and logs outcomes.
/// Returns a <see cref="Result"/> indicating success or failure, with error details if applicable.
/// </remarks>
public sealed class RegisterUserRequestHandler : IRequestHandler<RegisterUserRequest, Result>
{
    /// <summary>
    /// The logger.
    /// </summary>
    private readonly ILogger<RegisterUserRequestHandler> logger;

    /// <summary>
    /// The user account service, used to register the new user.
    /// </summary>
    private readonly IUserAccountService userAccountService;

    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterUserRequestHandler"/> class.
    /// </summary>
    /// <param name="logger">
    /// The logger used for recording registration events and errors.
    /// </param>
    /// <param name="userAccountService">
    /// The user account service responsible for user registration logic.
    /// </param>
    /// <exception cref="ArgumentNullException">
    /// Thrown if <paramref name="logger"/> or <paramref name="userAccountService"/> is <c>null</c>.
    /// </exception>
    public RegisterUserRequestHandler(
        ILogger<RegisterUserRequestHandler> logger,
        IUserAccountService userAccountService)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userAccountService = userAccountService ?? throw new ArgumentNullException(nameof(userAccountService));
    }

    /// <summary>
    /// Handles the registration of a new user account.
    /// </summary>
    /// <param name="request">
    /// The registration request containing user credentials.
    /// </param>
    /// <param name="cancellationToken">
    /// A token to monitor for cancellation requests.
    /// </param>
    /// <returns>
    /// A <see cref="Result"/> indicating the outcome of the registration.
    /// On failure, contains error messages describing the reason(s).
    /// </returns>
    public async Task<Result> Handle(RegisterUserRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        this.logger.LogInformation("Handling user registration for new user: '{Username}'", request.Username);

        var result = await this.userAccountService.RegisterUserAsync(
            emailAddress: request.EmailAddress,
            username: request.Username,
            password: request.Password)
            .ConfigureAwait(false);

        if (result.IsFailed)
        {
            this.logger.LogWarning("User registration failed for username: {Username}", request.Username);
            return Result.Fail(result.Errors.Select(x => x.Message));
        }

        this.logger.LogInformation("User registration succeeded for username: {Username}", request.Username);

        return Result.Ok();
    }
}
