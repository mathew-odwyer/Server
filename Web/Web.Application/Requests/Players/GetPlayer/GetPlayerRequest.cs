// <copyright file="GetPlayerRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.GetPlayer;

using System.Diagnostics.CodeAnalysis;
using MediatR;

[ExcludeFromCodeCoverage]
public sealed class GetPlayerRequest : IRequest<GetPlayerResponse>
{
    public required string Name { get; init; }

    public required string UserAccountId { get; init; }
}
