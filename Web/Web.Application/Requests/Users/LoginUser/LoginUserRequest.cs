// <copyright file="LoginUserRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.LoginUser;

using System.Diagnostics.CodeAnalysis;
using MediatR;

[ExcludeFromCodeCoverage]
public sealed record LoginUserRequest(
    string Username,
    string Password) : IRequest<LoginUserResponse>;
