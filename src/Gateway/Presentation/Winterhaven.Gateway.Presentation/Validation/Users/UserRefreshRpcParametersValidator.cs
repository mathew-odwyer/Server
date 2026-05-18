namespace Winterhaven.Gateway.Presentation.Validation.Users;

using FluentValidation;
using Winterhaven.Gateway.Presentation.Targets.Users;

public sealed class UserRefreshRpcParametersValidator : AbstractValidator<UserRefreshRpcParameters>
{
    public UserRefreshRpcParametersValidator()
    {
        this.RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token must not be empty.");
    }
}