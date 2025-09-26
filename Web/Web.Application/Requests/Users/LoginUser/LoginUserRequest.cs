// <copyright file="LoginUserRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.LoginUser;

using FluentResults;
using MediatR;

public sealed class LoginUserRequest : IRequest<Result<LoginUserResponse>>
{
    public required string Username { get; init; }

    public required string Password { get; init; }
}
