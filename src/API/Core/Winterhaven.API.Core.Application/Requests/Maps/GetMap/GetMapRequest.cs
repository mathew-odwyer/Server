namespace Winterhaven.API.Core.Application.Requests.Maps.GetMap;

using MediatR;
using System.Diagnostics.CodeAnalysis;

/// <summary>
///   Represents a request used to fetch an existing map.
/// </summary>
/// <param name="Name">
///   The name of the map to fetch.
/// </param>
/// <seealso cref="IBaseRequest"/>
[ExcludeFromCodeCoverage]
public sealed record GetMapRequest(string Name)
    : IRequest<GetMapResponse>;