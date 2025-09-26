// <copyright file="RefreshTokenRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.RefreshToken;

using FluentResults;
using MediatR;

public sealed class RefreshTokenRequest : IRequest<Result<RefreshTokenResponse>>
{
    public required string UserAccountId { get; init; }

    public required string RefreshToken { get; init; }
}
