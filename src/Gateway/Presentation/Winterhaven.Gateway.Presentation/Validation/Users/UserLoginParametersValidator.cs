namespace Winterhaven.Gateway.Presentation.Validation.Users;

using FluentValidation;
using Winterhaven.Gateway.Presentation.Targets.Users;

internal sealed class UserLoginParametersValidator : AbstractValidator<UserLoginParameters>
{
    public UserLoginParametersValidator()
    {
        this.RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username must not be empty.");

        this.RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password must not be empty.");
    }
}