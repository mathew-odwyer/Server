// <copyright file="CreatePlayerRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.CreatePlayer;

using System.Diagnostics.CodeAnalysis;
using MediatR;
using Web.Domain.Entities.Players;
using Web.Domain.Entities.Users;

/// <summary>
/// Represents a request used to create a new <see cref="Player"/>
/// </summary>
/// <seealso cref="IRequest" />
/// <seealso cref="IBaseRequest" />
/// <param name="UserAccountId">
/// The user account identifier that matches the <see cref="UserAccount"/> that wishes to create a new <see cref="Player"/>.
/// </param>
/// <param name="Name">
/// The name of the new <see cref="Player"/>.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record CreatePlayerRequest(
    string UserAccountId,
    string Name)
    : IRequest;
