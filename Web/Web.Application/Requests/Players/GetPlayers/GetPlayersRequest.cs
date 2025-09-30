// <copyright file="GetPlayersRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.GetPlayers;

using MediatR;

public sealed class GetPlayersRequest : IRequest<GetPlayersResponse>
{
    public required string UserAccountId { get; init; }
}
