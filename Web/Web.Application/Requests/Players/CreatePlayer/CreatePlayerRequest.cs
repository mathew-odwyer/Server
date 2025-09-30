// <copyright file="CreatePlayerRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.CreatePlayer;

using FluentResults;
using MediatR;

public sealed class CreatePlayerRequest : IRequest<Result>
{
    public required string UserAccountId { get; init; }

    public required string Name { get; init; }
}
