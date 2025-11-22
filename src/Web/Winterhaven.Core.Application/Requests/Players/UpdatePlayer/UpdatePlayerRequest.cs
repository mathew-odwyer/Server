// <copyright file="UpdatePlayerRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Requests.Players.UpdatePlayer;

using System.Diagnostics.CodeAnalysis;
using Winterhaven.Core.Application.Attributes;
using Winterhaven.Core.Domain.Entities.Players;
using MediatR;

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
    double? X,
    double? Y)
    : IRequest;
