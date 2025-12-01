// <copyright file="RefreshTokenRequestDto.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.DTOs.Users;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents the data transfer object for a refresh token request.
/// </summary>
/// <param name="RefreshToken">
/// The refresh token used to generate a new JWT.
/// </param>
[ExcludeFromCodeCoverage]
public sealed record RefreshTokenRequestDto(
    string RefreshToken);
