// <copyright file="GetPlayersRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.GetPlayers;

using System.Diagnostics.CodeAnalysis;
using MediatR;
using Web.Domain.Entities.Users;

/// <summary>
/// Represents a request used to fetch all players for the current <see cref="UserAccount"/>.
/// </summary>
/// <seealso cref="IRequest" />
/// <seealso cref="IBaseRequest" />
/// <param name="UserAccountId">
/// The user account identifier that matches the <see cref="UserAccount"/> that wishes to fetch all players.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record GetPlayersRequest(string UserAccountId) : IRequest<GetPlayersResponse>;
