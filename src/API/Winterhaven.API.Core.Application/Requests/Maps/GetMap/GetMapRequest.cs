using MediatR;

namespace Winterhaven.API.Core.Application.Requests.Maps.GetMap;

/// <summary>
///   Represents a request used to fetch an existing map.
/// </summary>
/// <param name="Name">
///   The name of the map to fetch.
/// </param>
/// <seealso cref="IBaseRequest"/>
public sealed record GetMapRequest(string Name)
    : IRequest<GetMapResponse>;