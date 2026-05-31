using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Winterhaven.API.Core.Application.Requests.Maps.GetMap;
using Winterhaven.Common.DTOs.Maps;

namespace Winterhaven.API.Presentation.Mappings.Maps;

[ExcludeFromCodeCoverage]
internal sealed class MapMapper : Profile
{
    public MapMapper() => CreateMap<GetMapRequestDto, GetMapRequest>();
}