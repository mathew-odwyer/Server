using System;
using MediatR;
using Winterhaven.API.Core.Domain.Attributes.Users;

namespace Winterhaven.API.Core.Application.Requests.Players.UpdatePlayer;
/// <summary>
///   Represents a request used to update an existing player.
/// </summary>
/// <seealso cref="IRequest"/>
/// <seealso cref="IBaseRequest"/>
/// <param name="PlayerId">
///   The identifier of the player to update.
/// </param>
/// <param name="X">
///   The optional X-coordinate of the player.
/// </param>
/// <param name="Y">
///   The optional Y-coordinate of the player.
/// </param>
/// <param name="RoomId">
///   The identifier of the room the player is in.
/// </param>
[Authorize]
public sealed record UpdatePlayerRequest(
    Guid PlayerId,
    double? X,
    double? Y,
    Guid RoomId)
    : IRequest;
