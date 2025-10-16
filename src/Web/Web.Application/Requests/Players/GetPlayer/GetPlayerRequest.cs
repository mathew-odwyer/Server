// <copyright file="GetPlayerRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.GetPlayer;

using System.Diagnostics.CodeAnalysis;
using MediatR;
using Web.Application.Attributes;
using Web.Domain.Entities.Players;

/// <summary>
/// Represents a request used to fetch an existing <see cref="Player"/>
/// </summary>
/// <seealso cref="IRequest" />
/// <seealso cref="IBaseRequest" />
[ExcludeFromCodeCoverage]
[Authorize]
public sealed record GetPlayerRequest()
    : IRequest<GetPlayerResponse>;
