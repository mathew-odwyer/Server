// <copyright file="LogoutUserRequestValidator.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.LogoutUser;

using FluentValidation;

public sealed class LogoutUserRequestValidator : AbstractValidator<LogoutUserRequest>
{
    public LogoutUserRequestValidator()
    {
        this.RuleFor(x => x.UserAccountId)
            .NotEmpty()
            .WithMessage("User account ID must not be empty.");
    }
}
