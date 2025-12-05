// <copyright file="GetMapResponse.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Requests.Maps.GetMap;

/// <summary>
/// Represents a response object that contains map data.
/// </summary>
/// <param name="Data">
/// The map data.
/// </param>
public sealed record GetMapResponse(IEnumerable<byte> Data);
