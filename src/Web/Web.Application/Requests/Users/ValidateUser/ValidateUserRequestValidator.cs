// <copyright file="ValidateUserRequestValidator.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.ValidateUser;

using FluentValidation;

public sealed class ValidateUserRequestValidator : AbstractValidator<ValidateUserRequest>
{
    public ValidateUserRequestValidator()
    {
        this.RuleFor(x => x.ClientToken)
            .NotEmpty()
            .WithMessage("Client token must not be empty.");
    }
}
