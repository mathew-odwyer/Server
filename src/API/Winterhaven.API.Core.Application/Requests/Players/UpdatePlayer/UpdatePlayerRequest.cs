using MediatR;
using Winterhaven.API.Core.Domain.Attributes.Users;

namespace Winterhaven.API.Core.Application.Requests.Players.UpdatePlayer;
/// <summary>
///   Represents a request used to update an existing player.
/// </summary>
/// <seealso cref="IRequest"/>
/// <seealso cref="IBaseRequest"/>
/// <param name="X">
///   The optional X-coordinate of the player.
/// </param>
/// <param name="Y">
///   The optional Y-coordinate of the player.
/// </param>
[Authorize]
public sealed record UpdatePlayerRequest(
    double? X,
    double? Y)
    : IRequest;