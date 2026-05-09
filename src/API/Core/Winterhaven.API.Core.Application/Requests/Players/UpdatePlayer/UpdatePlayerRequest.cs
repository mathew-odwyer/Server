namespace Winterhaven.API.Core.Application.Requests.Players.UpdatePlayer;

using MediatR;
using Winterhaven.API.Core.Domain.Attributes.Users;
using Winterhaven.API.Core.Domain.Entities.Players;

/// <summary>
/// Represents a request used to update an existing <see cref="Player"/>.
/// </summary>
/// <seealso cref="IRequest"/>
/// <seealso cref="IBaseRequest"/>
/// <param name="X">The optional X-coordinate of the <see cref="Player"/>.</param>
/// <param name="Y">The optional Y-coordinate of the <see cref="Player"/>.</param>
[Authorize]
public sealed record UpdatePlayerRequest(
    double? X,
    double? Y)
    : IRequest;