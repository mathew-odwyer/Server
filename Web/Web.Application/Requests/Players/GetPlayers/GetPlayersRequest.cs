// <copyright file="GetPlayersRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.GetPlayers;

using System.Diagnostics.CodeAnalysis;
using MediatR;

[ExcludeFromCodeCoverage]
public sealed record GetPlayersRequest(string UserAccountId) : IRequest<GetPlayersResponse>;
