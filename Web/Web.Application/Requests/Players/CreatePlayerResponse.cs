// <copyright file="CreatePlayerResponse.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players;

public sealed class CreatePlayerResponse
{
    public required string Username { get; init; }
}
