namespace Winterhaven.Gateway.Presentation.Validation.Users;

using FluentValidation;
using Winterhaven.Gateway.Presentation.Targets.Users;

internal sealed class UserLoginRpcParametersValidator : AbstractValidator<UserLoginRpcParameters>
{
    public UserLoginRpcParametersValidator()
    {
        this.RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username must not be empty.");

        this.RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password must not be empty.");
    }
}