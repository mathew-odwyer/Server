// <copyright file="RefreshTokenRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Requests.Users.RefreshToken;

using System.Diagnostics.CodeAnalysis;
using Winterhaven.Core.Application.Attributes;
using Winterhaven.Core.Domain.Entities.Users;
using MediatR;

/// <summary>
/// Represents a request used to refresh a JSON Web Token for an existing <see cref="UserAccount"/>.
/// </summary>
/// <seealso cref="IRequest{LoginUserResponse}" />
/// <seealso cref="IBaseRequest" />
/// <param name="RefreshToken">
/// The refresh token used to refresh the JSON Web Token for the <see cref="UserAccount"/>.
/// </param>
[ExcludeFromCodeCoverage]
[Authorize]
public sealed record RefreshTokenRequest(string RefreshToken)
    : IRequest<RefreshTokenResponse>;
