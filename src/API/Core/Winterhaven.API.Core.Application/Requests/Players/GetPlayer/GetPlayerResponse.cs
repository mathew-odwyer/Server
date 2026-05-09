namespace Winterhaven.API.Core.Application.Requests.Players.GetPlayer;

using Winterhaven.API.Core.Domain.Entities.Players;

/// <summary>
/// Represents a response that contains the details of an existing <see cref="Player"/>.
/// </summary>
/// <param name="Name">The unique name of the <see cref="Player"/>.</param>
/// <param name="X">The current X-coordinate of the <see cref="Player"/>.</param>
/// <param name="Y">The current Y-coordinate of the <see cref="Player"/>.</param>
public sealed record GetPlayerResponse(
    string Name,
    double X,
    double Y);