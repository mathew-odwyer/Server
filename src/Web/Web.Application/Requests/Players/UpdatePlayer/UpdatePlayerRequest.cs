// <copyright file="UpdatePlayerRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.UpdatePlayer;

using System.Diagnostics.CodeAnalysis;
using MediatR;
using Web.Application.Attributes;
using Web.Domain.Entities.Players;

/// <summary>
/// Represents a request used to update an existing <see cref="Player"/>.
/// </summary>
/// <seealso cref="IRequest" />
/// <seealso cref="IBaseRequest" />
/// <param name="X">
/// The optional X-coordinate of the <see cref="Player"/>.
/// </param>
/// <param name="Y">
/// The optional Y-coordinate of the <see cref="Player"/>.
/// </param>
[ExcludeFromCodeCoverage]
[Authorize]
public sealed record UpdatePlayerRequest(
    int? X,
    int? Y)
    : IRequest;
