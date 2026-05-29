using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Core.Domain.ValueObjects.Maps;

namespace Winterhaven.API.Core.Application.Services.Maps;

/// <summary>
/// </summary>
public interface IMapLocator
{
    /// <summary>
    /// </summary>
    /// <param name="name">
    /// </param>
    /// <param name="cancellationToken">
    /// </param>
    /// <returns>
    /// </returns>
    public Task<MapData> LocateMapDataAsync(string name, CancellationToken cancellationToken = default);
}