// <copyright file="DeletePlayerRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.DeletePlayer;

using System.Diagnostics.CodeAnalysis;
using MediatR;

[ExcludeFromCodeCoverage]
public sealed class DeletePlayerRequest : IRequest
{
    public required string UserAccountId { get; init; }

    public required string Name { get; init; }
}
