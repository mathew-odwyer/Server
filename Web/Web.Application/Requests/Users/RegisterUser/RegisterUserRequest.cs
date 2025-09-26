// <copyright file="RegisterUserRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.RegisterUser;

using System.Diagnostics.CodeAnalysis;
using FluentResults;
using MediatR;

/// <summary>
/// Represents a request to register a new user via the mediator pattern.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed class RegisterUserRequest : IRequest<Result>
{
    /// <summary>
    /// Gets the email address to associate with the new user account.
    /// </summary>
    /// <value>
    /// A valid, unique email address for the user.
    /// </value>
    public required string EmailAddress { get; init; }

    /// <summary>
    /// Gets the password for the new user account.
    /// </summary>
    /// <value>
    /// The password for the new user account.
    /// </value>
    public required string Password { get; init; }

    /// <summary>
    /// Gets the username for the new user account.
    /// </summary>
    /// <value>
    /// The username for the new user account.
    /// </value>
    public required string Username { get; init; }
}
