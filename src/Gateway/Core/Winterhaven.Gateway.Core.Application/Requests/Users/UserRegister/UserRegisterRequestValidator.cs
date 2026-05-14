namespace Winterhaven.Gateway.Core.Application.Requests.Users.UserRegister;

using FluentValidation;

public class UserRegisterRequestValidator : AbstractValidator<UserRegisterRequest>
{
    public UserRegisterRequestValidator()
    {
        this.RuleFor(x => x.Username)
            .NotEmpty()
            .MaximumLength(100);

        this.RuleFor(x => x.EmailAddress)
            .NotEmpty()
            .EmailAddress();

        this.RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(1);
    }
}
