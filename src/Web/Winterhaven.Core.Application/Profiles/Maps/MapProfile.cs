// <copyright file="MapProfile.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Profiles.Maps;

using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Winterhaven.Core.Application.DTOs.Maps;
using Winterhaven.Core.Application.Requests.Maps.GetMap;

[ExcludeFromCodeCoverage]
internal sealed class MapProfile : Profile
{
    public MapProfile()
    {
        this.CreateMap<GetMapRequestDto, GetMapRequest>();
    }
}
