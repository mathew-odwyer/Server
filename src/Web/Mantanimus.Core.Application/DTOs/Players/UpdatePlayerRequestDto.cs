// <copyright file="UpdatePlayerRequestDto.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Core.Application.DTOs.Players;

using System.Diagnostics.CodeAnalysis;
using Mantanimus.Core.Domain.Entities.Players;

/// <summary>
/// Represents the data transfer object used to update an existing <see cref="Player"/>.
/// </summary>
/// <param name="X">
/// The current X-coordinate of the <see cref="Player"/>.
/// </param>
/// <param name="Y">
/// The current Y-coordinate of the <see cref="Player"/>.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record UpdatePlayerRequestDto(
    int? X,
    int? Y);
