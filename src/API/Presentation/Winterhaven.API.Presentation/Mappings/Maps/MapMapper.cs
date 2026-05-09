namespace Winterhaven.API.Presentation.Mappings.Maps;

using AutoMapper;
using System.Diagnostics.CodeAnalysis;
using Winterhaven.API.Core.Application.Requests.Maps.GetMap;
using Winterhaven.API.Presentation.DTOs.Maps;

[ExcludeFromCodeCoverage]
internal sealed class MapMapper : Profile
{
    public MapMapper()
    {
        this.CreateMap<GetMapRequestDto, GetMapRequest>();
    }
}