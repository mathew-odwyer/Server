// <copyright file="LoginUserRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.LoginUser;

using System.Diagnostics.CodeAnalysis;
using MediatR;
using Web.Domain.Entities.Users;

/// <summary>
/// Represents a request used to authenticate an existing <see cref="UserAccount"/>.
/// </summary>
/// <seealso cref="IRequest{LoginUserResponse}" />
/// <seealso cref="IBaseRequest" />
/// <param name="Username">
/// The username associated with the <see cref="UserAccount"/> attempting to log in. This value must not be empty.
/// </param>
/// <param name="Password">
/// The password associated with the <see cref="UserAccount"/> attempting to log in. This value must not be empty.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record LoginUserRequest(
    string Username,
    string Password) : IRequest<LoginUserResponse>;
