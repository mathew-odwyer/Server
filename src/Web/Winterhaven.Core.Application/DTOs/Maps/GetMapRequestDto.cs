// <copyright file="GetMapRequestDto.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.DTOs.Maps;

/// <summary>
/// Represents the data transfer object used to fetch a map.
/// </summary>
/// <param name="Name">
/// The name of the map to fetch.
/// </param>
public sealed record GetMapRequestDto(string Name);
