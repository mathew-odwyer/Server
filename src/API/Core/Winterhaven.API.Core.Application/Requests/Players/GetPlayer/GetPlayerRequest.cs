using MediatR;
using Winterhaven.API.Core.Domain.Attributes.Users;

namespace Winterhaven.API.Core.Application.Requests.Players.GetPlayer;
/// <summary>
///   Represents a request used to fetch an existing player
/// </summary>
/// <seealso cref="IRequest"/>
/// <seealso cref="IBaseRequest"/>
[Authorize]
public sealed record GetPlayerRequest
    : IRequest<GetPlayerResponse>;