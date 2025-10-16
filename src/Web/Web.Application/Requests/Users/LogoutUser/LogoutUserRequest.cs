// <copyright file="LogoutUserRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.LogoutUser;

using System.Diagnostics.CodeAnalysis;
using MediatR;
using Web.Application.Attributes;
using Web.Domain.Entities.Users;

/// <summary>
/// Represents a request used to logout an existing <see cref="UserAccount"/>.
/// </summary>
/// <seealso cref="IRequest{LoginUserResponse}" />
/// <seealso cref="IBaseRequest" />
[ExcludeFromCodeCoverage]
[Authorize]
public sealed record LogoutUserRequest()
    : IRequest;
