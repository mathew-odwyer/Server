// <copyright file="RefreshTokenRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.RefreshToken;

using System.Diagnostics.CodeAnalysis;
using MediatR;

[ExcludeFromCodeCoverage]
public sealed record RefreshTokenRequest(
    string UserAccountId,
    string RefreshToken) : IRequest<RefreshTokenResponse>;
