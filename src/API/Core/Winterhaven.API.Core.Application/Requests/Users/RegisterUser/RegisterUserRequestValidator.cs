namespace Winterhaven.API.Core.Application.Requests.Users.RegisterUser;

using FluentValidation;
using FluentValidation.Validators;
using Winterhaven.API.Core.Application.Validators.Users;

/// <summary>
///   Provides validation for the <see cref="RegisterUserRequest"/> class.
/// </summary>
public sealed class RegisterUserRequestValidator : AbstractValidator<RegisterUserRequest>
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="RegisterUserRequestValidator"/> class.
    /// </summary>
    public RegisterUserRequestValidator()
    {
        this.RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .WithMessage("Email address must not be empty.")
            .EmailAddress(EmailValidationMode.AspNetCoreCompatible)
            .WithMessage("Email address must be a valid format.");

        this.RuleFor(x => x.Username)
            .NotNull()
            .WithMessage("Username must not be null.")
            .SetValidator(new UsernameValidator());

        this.RuleFor(x => x.Password)
            .NotNull()
            .WithMessage("Password must not be null.")
            .SetValidator(new PasswordValidator());
    }
}