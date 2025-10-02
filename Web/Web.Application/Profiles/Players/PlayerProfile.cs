// <copyright file="PlayerProfile.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Profiles.Players;

using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Web.Application.DTOs.Players;
using Web.Domain.Entities.Players;

[ExcludeFromCodeCoverage]
public sealed class PlayerProfile : Profile
{
    public PlayerProfile()
    {
        this.CreateMap<Player, PlayerDto>();
    }
}
