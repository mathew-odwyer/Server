// <copyright file="RegisterUserRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.RegisterUser;

using System.Diagnostics.CodeAnalysis;
using MediatR;

/// <summary>
/// Represents a request to register a new user via the mediator pattern.
/// </summary>
[ExcludeFromCodeCoverage]
public sealed record RegisterUserRequest
    (string EmailAddress,
    string Username,
    string Password) : IRequest;
