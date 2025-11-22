// <copyright file="GetPlayerRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Requests.Players.GetPlayer;

using System.Diagnostics.CodeAnalysis;
using Winterhaven.Core.Application.Attributes;
using Winterhaven.Core.Domain.Entities.Players;
using MediatR;

/// <summary>
/// Represents a request used to fetch an existing <see cref="Player"/>
/// </summary>
/// <seealso cref="IRequest" />
/// <seealso cref="IBaseRequest" />
[ExcludeFromCodeCoverage]
[Authorize]
public sealed record GetPlayerRequest()
    : IRequest<GetPlayerResponse>;
