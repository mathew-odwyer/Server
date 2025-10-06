// <copyright file="PlayerProfile.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Profiles.Players;

using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Web.Application.DTOs.Players;
using Web.Domain.Entities.Players;

/// <summary>
/// Provides mapping configuration for player-related objects.
/// </summary>
/// <seealso cref="Profile" />
[ExcludeFromCodeCoverage]
public sealed class PlayerProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PlayerProfile"/> class.
    /// </summary>
    public PlayerProfile()
    {
        this.CreateMap<Player, PlayerDto>();
    }
}
