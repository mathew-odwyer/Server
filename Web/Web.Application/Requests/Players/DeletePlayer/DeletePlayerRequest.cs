// <copyright file="DeletePlayerRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.DeletePlayer;

using System.Diagnostics.CodeAnalysis;
using MediatR;

[ExcludeFromCodeCoverage]
public sealed record DeletePlayerRequest(
    string UserAccountId,
    string Name) : IRequest;
