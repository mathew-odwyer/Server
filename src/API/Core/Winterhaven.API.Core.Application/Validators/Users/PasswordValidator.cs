namespace Winterhaven.API.Core.Application.Validators.Users;

using FluentValidation;
using System.Linq;

internal sealed class PasswordValidator : AbstractValidator<string>
{
    internal PasswordValidator()
    {
        this.RuleFor(x => x).NotEmpty().WithMessage("Password cannot be empty.");
        this.RuleFor(x => x).MinimumLength(12).WithMessage("Password must be at least 12 characters.");
        this.RuleFor(x => x).Must(ContainsUppercaseCharacter).WithMessage("Password must contain at least one uppercase character.");
        this.RuleFor(x => x).Must(ContainsLowercaseCharacter).WithMessage("Password must contain at least one lowercase character.");
        this.RuleFor(x => x).Must(ContainsNumber).WithMessage("Password must contain at least one number.");
        this.RuleFor(x => x).Must(ContainsSpecialCharacter).WithMessage("Password must contain at least one special character.");
    }

    private static bool ContainsLowercaseCharacter(string password)
    {
        return password.Any(char.IsLower);
    }

    private static bool ContainsNumber(string password)
    {
        return password.Any(char.IsDigit);
    }

    private static bool ContainsSpecialCharacter(string password)
    {
        return password.Any(x => !char.IsLetterOrDigit(x));
    }

    private static bool ContainsUppercaseCharacter(string password)
    {
        return password.Any(char.IsUpper);
    }
}