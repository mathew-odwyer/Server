namespace Winterhaven.API.Presentation.Mappings.Players;

using AutoMapper;
using System.Diagnostics.CodeAnalysis;
using Winterhaven.API.Core.Application.Requests.Players.UpdatePlayer;
using Winterhaven.API.Core.Domain.Entities.Players;
using Winterhaven.API.Common.DTOs.Players;

[ExcludeFromCodeCoverage]
internal sealed class PlayerMapper : Profile
{
    public PlayerMapper()
    {
        this.CreateMap<UpdatePlayerRequestDto, UpdatePlayerRequest>();
        this.CreateMap<Player, GetPlayerRequestDto>();
    }
}