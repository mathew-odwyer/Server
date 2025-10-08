// <copyright file="LogoutUserRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.LogoutUser;

using System.Diagnostics.CodeAnalysis;
using MediatR;
using Web.Domain.Entities.Users;

/// <summary>
/// Represents a request used to logout an existing <see cref="UserAccount"/>.
/// </summary>
/// <seealso cref="IRequest{LoginUserResponse}" />
/// <seealso cref="IBaseRequest" />
/// <param name="UserAccountId">
/// The user account identifier that refers to the <see cref="UserAccount"/> that wishes to logout.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record LogoutUserRequest(
    string UserAccountId) : IRequest;
