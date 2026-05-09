namespace Winterhaven.API.Core.Application.Services.Maps;

using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

[ExcludeFromCodeCoverage]
public sealed record MapData(
    string Name,
    ReadOnlyMemory<byte> Data);

public interface IMapLocator
{
    Task<MapData> LocateMapDataAsync(string name, CancellationToken cancellationToken = default);
}