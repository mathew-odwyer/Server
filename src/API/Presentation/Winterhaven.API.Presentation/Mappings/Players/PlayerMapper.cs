namespace Winterhaven.API.Presentation.Mappings.Players;

using AutoMapper;
using System.Diagnostics.CodeAnalysis;
using Winterhaven.API.Core.Application.Requests.Players.GetPlayer;
using Winterhaven.API.Core.Application.Requests.Players.UpdatePlayer;
using Winterhaven.Common.DTOs.Players;

[ExcludeFromCodeCoverage]
internal sealed class PlayerMapper : Profile
{
    public PlayerMapper()
    {
        this.CreateMap<UpdatePlayerRequestDto, UpdatePlayerRequest>();
        this.CreateMap<GetPlayerResponse, GetPlayerResponseDto>();
    }
}