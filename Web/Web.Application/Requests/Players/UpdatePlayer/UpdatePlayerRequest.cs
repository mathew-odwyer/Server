// <copyright file="UpdatePlayerRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.UpdatePlayer;

using System.Diagnostics.CodeAnalysis;
using MediatR;

[ExcludeFromCodeCoverage]
public sealed class UpdatePlayerRequest : IRequest
{
    public required string UserAccountId { get; init; }

    public required string Name { get; init; }

    public int? X { get; init; }

    public int? Y { get; init; }
}
