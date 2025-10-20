// <copyright file="UpdatePlayerRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Core.Application.Requests.Players.UpdatePlayer;

using System.Diagnostics.CodeAnalysis;
using Mantanimus.Core.Application.Attributes;
using Mantanimus.Core.Domain.Entities.Players;
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
    int? X,
    int? Y)
    : IRequest;
