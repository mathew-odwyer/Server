// <copyright file="LogoutUserRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.LogoutUser;

using FluentResults;
using MediatR;

public sealed class LogoutUserRequest : IRequest<Result>
{
    public required string UserAccountId { get; init; }
}
