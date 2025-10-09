// <copyright file="RegisterUserRequestValidator.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.RegisterUser;

using FluentValidation;
using FluentValidation.Validators;
using Web.Application.Validators.Users;

/// <summary>
/// Provides a validator for the <see cref="RegisterUserRequest"/> class.
/// </summary>
public sealed class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RegisterUserRequestValidator"/> class.
    /// </summary>
    public RegisterUserRequestValidator()
    {
        this.RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .WithMessage("Email address must not be empty.")
            .EmailAddress(EmailValidationMode.AspNetCoreCompatible)
            .WithMessage("Email address must be a valid format.");

        this.RuleFor(x => x.Username)
            .SetValidator(new UsernameValidator());

        this.RuleFor(x => x.Password)
            .SetValidator(new PasswordValidator());
    }
}
