// <copyright file="UpdatePlayerRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.UpdatePlayer;

using System.Diagnostics.CodeAnalysis;
using MediatR;
using Web.Domain.Entities.Players;
using Web.Domain.Entities.Users;

/// <summary>
/// Represents a request used to update an existing <see cref="Player"/>.
/// </summary>
/// <seealso cref="IRequest" />
/// <seealso cref="IBaseRequest" />
/// <param name="UserAccountId">
/// The user account identifier that matches the <see cref="UserAccount"/> who owns the <see cref="Player"/> being updated.
/// </param>
/// <param name="Name">
/// The updated name of the <see cref="Player"/>.
/// </param>
/// <param name="X">
/// The optional X-coordinate of the <see cref="Player"/>.
/// </param>
/// <param name="Y">
/// The optional Y-coordinate of the <see cref="Player"/>.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record UpdatePlayerRequest(
    string UserAccountId,
    string Name,
    int? X,
    int? Y)
    : IRequest;
