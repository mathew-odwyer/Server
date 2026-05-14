namespace Winterhaven.Gateway.Core.Application.Requests.Users.UserLogin;

using FluentValidation;

public sealed class UserLoginRequestValidator : AbstractValidator<UserLoginRequest>
{
    public UserLoginRequestValidator()
    {
        this.RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(100);

        this.RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(1);
    }
}
