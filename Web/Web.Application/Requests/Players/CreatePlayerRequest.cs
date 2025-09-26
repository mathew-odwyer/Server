// <copyright file="CreatePlayerRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players;

using FluentResults;
using MediatR;

public sealed class CreatePlayerRequest : IRequest<Result<CreatePlayerResponse>>
{
    public required string UserAccountId { get; init; }

    public required string Name { get; init; }
}
