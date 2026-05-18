namespace Winterhaven.API.Core.Application.Requests.Users.LoginUser;

using FluentValidation;

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
        this.RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username must not be empty.");

        this.RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password must not be empty.");
    }
}