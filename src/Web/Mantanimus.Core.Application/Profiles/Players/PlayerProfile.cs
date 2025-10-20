// <copyright file="PlayerProfile.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Core.Application.Profiles.Players;

using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Mantanimus.Core.Application.DTOs.Players;
using Mantanimus.Core.Application.Requests.Players.UpdatePlayer;
using Mantanimus.Core.Domain.Entities.Players;

[ExcludeFromCodeCoverage]
internal sealed class PlayerProfile : Profile
{
    public PlayerProfile()
    {
        this.CreateMap<UpdatePlayerRequestDto, UpdatePlayerRequest>();
        this.CreateMap<Player, PlayerDto>();
    }
}
