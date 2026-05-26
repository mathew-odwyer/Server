using FluentValidation;

namespace Winterhaven.API.Core.Application.Requests.Users.LoginUser;

/// <summary>
///   Provides validation for a <see cref="LoginUserRequest"/>.
/// </summary>
public sealed class LoginUserRequestValidator : AbstractValidator<LoginUserRequest>
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="LoginUserRequestValidator"/> class.
    /// </summary>
    public LoginUserRequestValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username must not be empty.")
            .OverridePropertyName("Username");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password must not be empty.")
            .OverridePropertyName("Password");
    }
}