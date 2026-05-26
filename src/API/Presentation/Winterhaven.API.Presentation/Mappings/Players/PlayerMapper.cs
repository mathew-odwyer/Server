using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Winterhaven.API.Core.Application.Requests.Players.GetPlayer;
using Winterhaven.API.Core.Application.Requests.Players.UpdatePlayer;
using Winterhaven.Common.DTOs.Players;

namespace Winterhaven.API.Presentation.Mappings.Players;

[ExcludeFromCodeCoverage]
internal sealed class PlayerMapper : Profile
{
    public PlayerMapper()
    {
        CreateMap<UpdatePlayerRequestDto, UpdatePlayerRequest>();
        CreateMap<GetPlayerResponse, GetPlayerResponseDto>();
    }
}