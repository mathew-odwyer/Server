namespace Winterhaven.API.Core.Domain.ValueObjects.Maps;

using System;

public sealed record MapData(
    string Name,
    ReadOnlyMemory<byte> Data);