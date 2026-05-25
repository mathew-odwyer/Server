namespace Winterhaven.API.Core.Application.Requests.Players.GetPlayer;

using MediatR;
using System.Diagnostics.CodeAnalysis;
using Winterhaven.API.Core.Domain.Attributes.Users;

/// <summary>
///   Represents a request used to fetch an existing player
/// </summary>
/// <seealso cref="IRequest"/>
/// <seealso cref="IBaseRequest"/>
[Authorize]
[ExcludeFromCodeCoverage]
public sealed record GetPlayerRequest
    : IRequest<GetPlayerResponse>;