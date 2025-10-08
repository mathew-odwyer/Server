// <copyright file="LogoutUserRequestValidator.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.LogoutUser;

using FluentValidation;

/// <summary>
/// Provides validation for a <see cref="LogoutUserRequest"/>.
/// </summary>
public sealed class LogoutUserRequestValidator : AbstractValidator<LogoutUserRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="LogoutUserRequestValidator"/> class.
    /// </summary>
    public LogoutUserRequestValidator()
    {
        this.RuleFor(x => x.UserAccountId)
            .NotEmpty()
            .WithMessage("User account ID must not be empty.");
    }
}
