namespace Winterhaven.API.Core.Application.Requests.Players.GetPlayer;

using System;

/// <summary>
///   Represents a response that contains the details of an existing player.
/// </summary>
/// <param name="Identifier">
///   The unique identifier of the player.
/// </param>
/// <param name="Name">
///   The unique name of the player.
/// </param>
/// <param name="X">
///   The current X-coordinate of the player.
/// </param>
/// <param name="Y">
///   The current Y-coordinate of the player.
/// </param>
public sealed record GetPlayerResponse(
    Guid Identifier,
    string Name,
    double X,
    double Y);