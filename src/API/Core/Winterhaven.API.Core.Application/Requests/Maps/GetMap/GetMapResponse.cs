namespace Winterhaven.API.Core.Application.Requests.Maps.GetMap;

using System;

/// <summary>
/// Represents a response object that contains map data.
/// </summary>
/// <param name="Name">The name of the map.</param>
/// <param name="Data">The map data.</param>
public sealed record GetMapResponse(string Name, ReadOnlyMemory<byte> Data);