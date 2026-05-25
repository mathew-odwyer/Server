namespace Winterhaven.API.Core.Application.Requests.Players.GetPlayer;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
///   Represents a response that contains the details of an existing player.
/// </summary>
/// <param name="Id">
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
[ExcludeFromCodeCoverage]
public sealed record GetPlayerResponse(
    Guid Id,
    string Name,
    double X,
    double Y);