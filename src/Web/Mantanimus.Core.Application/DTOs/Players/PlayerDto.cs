// <copyright file="PlayerDto.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Core.Application.DTOs.Players;

using System.Diagnostics.CodeAnalysis;
using Mantanimus.Core.Domain.Entities.Players;

/// <summary>
/// Represents a player within the system.
/// </summary>
/// <param name="Name">
/// The unique name of the <see cref="Player"/>.
/// </param>
/// <param name="X">
/// The current X-coordinate of the <see cref="Player"/>.
/// </param>
/// <param name="Y">
/// The current Y-coordinate of the <see cref="Player"/>.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record PlayerDto(
    string Name,
    int X,
    int Y);
