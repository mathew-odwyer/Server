// <copyright file="GetPlayerRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.GetPlayer;

using System.Diagnostics.CodeAnalysis;
using MediatR;
using Web.Domain.Entities.Players;
using Web.Domain.Entities.Users;

/// <summary>
/// Represents a request used to fetch an existing <see cref="Player"/>
/// </summary>
/// <seealso cref="IRequest" />
/// <seealso cref="IBaseRequest" />
/// <param name="UserAccountId">
/// The user account identifier that matches the <see cref="UserAccount"/> that wishes to fetch an existing <see cref="Player"/>.
/// </param>
/// <param name="Name">
/// The name of the existing <see cref="Player"/> to fetch.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record GetPlayerRequest(
    string UserAccountId,
    string Name) : IRequest<GetPlayerResponse>;
