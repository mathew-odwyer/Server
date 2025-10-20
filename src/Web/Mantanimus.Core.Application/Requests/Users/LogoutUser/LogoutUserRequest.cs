// <copyright file="LogoutUserRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Core.Application.Requests.Users.LogoutUser;

using System.Diagnostics.CodeAnalysis;
using Mantanimus.Core.Application.Attributes;
using Mantanimus.Core.Domain.Entities.Users;
using MediatR;

/// <summary>
/// Represents a request used to logout an existing <see cref="UserAccount"/>.
/// </summary>
/// <seealso cref="IRequest{LoginUserResponse}" />
/// <seealso cref="IBaseRequest" />
[ExcludeFromCodeCoverage]
[Authorize]
public sealed record LogoutUserRequest()
    : IRequest;
