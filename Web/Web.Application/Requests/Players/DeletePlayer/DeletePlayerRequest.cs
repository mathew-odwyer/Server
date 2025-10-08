// <copyright file="DeletePlayerRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.DeletePlayer;

using System.Diagnostics.CodeAnalysis;
using MediatR;
using Web.Domain.Entities.Players;
using Web.Domain.Entities.Users;

/// <summary>
/// Represents a request used to delete an existing <see cref="Player"/>
/// </summary>
/// <seealso cref="IRequest" />
/// <seealso cref="IBaseRequest" />
/// <param name="UserAccountId">
/// The user account identifier that matches the <see cref="UserAccount"/> that wishes to delete an existing <see cref="Player"/>.
/// </param>
/// <param name="Name">
/// The name of the existing <see cref="Player"/> to delete.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record DeletePlayerRequest(
    string UserAccountId,
    string Name) : IRequest;
