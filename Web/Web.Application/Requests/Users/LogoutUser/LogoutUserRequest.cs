// <copyright file="LogoutUserRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.LogoutUser;

using System.Diagnostics.CodeAnalysis;
using FluentResults;
using MediatR;

[ExcludeFromCodeCoverage]
public sealed class LogoutUserRequest : IRequest<Result>
{
    public required string UserAccountId { get; init; }
}
