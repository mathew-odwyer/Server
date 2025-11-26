// <copyright file="RefreshTokenRequestValidator.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Requests.Users.RefreshToken;

using FluentValidation;

/// <summary>
/// Provides validation for a <see cref="RefreshTokenRequest"/>.
/// </summary>
public sealed class RefreshTokenRequestValidator : AbstractValidator<RefreshTokenRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RefreshTokenRequestValidator"/> class.
    /// </summary>
    public RefreshTokenRequestValidator()
    {
        this.RuleFor(request => request.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token must not be empty.");
    }
}
