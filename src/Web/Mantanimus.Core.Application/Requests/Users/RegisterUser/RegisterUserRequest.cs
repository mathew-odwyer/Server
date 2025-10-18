// <copyright file="RegisterUserRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Core.Application.Requests.Users.RegisterUser;

using System.Diagnostics.CodeAnalysis;
using MediatR;

/// <summary>
/// Represents a request to register a new user.
/// </summary>
/// <param name="Username">
/// Specifies a <see cref="string"/> that represents the username of the user to be registered.
/// </param>
/// <param name="Password">
/// Specifies a <see cref="string"/> that represents the password of the user to be registered.
/// </param>
/// <param name="EmailAddress">
/// Specifies a <see cref="string"/> that represents the email address of the user to be registered.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record RegisterUserRequest
    (string EmailAddress,
    string Username,
    string Password) : IRequest;
