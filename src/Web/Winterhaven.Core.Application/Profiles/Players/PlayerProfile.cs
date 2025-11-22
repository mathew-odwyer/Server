// <copyright file="PlayerProfile.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Profiles.Players;

using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Winterhaven.Core.Application.DTOs.Players;
using Winterhaven.Core.Application.Requests.Players.UpdatePlayer;
using Winterhaven.Core.Domain.Entities.Players;

[ExcludeFromCodeCoverage]
internal sealed class PlayerProfile : Profile
{
    public PlayerProfile()
    {
        this.CreateMap<UpdatePlayerRequestDto, UpdatePlayerRequest>();
        this.CreateMap<Player, PlayerDto>();
    }
}
