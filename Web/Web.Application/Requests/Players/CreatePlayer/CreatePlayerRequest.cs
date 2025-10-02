// <copyright file="CreatePlayerRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.CreatePlayer;

using System.Diagnostics.CodeAnalysis;
using FluentResults;
using MediatR;

[ExcludeFromCodeCoverage]
public sealed record CreatePlayerRequest(
    string UserAccountId,
    string Name)
    : IRequest<Result>;
