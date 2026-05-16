namespace Winterhaven.API.Core.Application.Services.Maps;

using System;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Core.Domain.ValueObjects.Maps;

public interface IMapLocator
{
    Task<MapData> LocateMapDataAsync(string name, CancellationToken cancellationToken = default);
}