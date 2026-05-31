using System.Linq;
using FluentValidation;

namespace Winterhaven.API.Core.Application.Validators.Users;

internal sealed class PasswordValidator : AbstractValidator<string>
{
    internal PasswordValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage("Password cannot be empty.");
        RuleFor(x => x).MinimumLength(12).WithMessage("Password must be at least 12 characters.");
        RuleFor(x => x).Must(ContainsUppercaseCharacter).WithMessage("Password must contain at least one uppercase character.");
        RuleFor(x => x).Must(ContainsLowercaseCharacter).WithMessage("Password must contain at least one lowercase character.");
        RuleFor(x => x).Must(ContainsNumber).WithMessage("Password must contain at least one number.");
        RuleFor(x => x).Must(ContainsSpecialCharacter).WithMessage("Password must contain at least one special character.");
    }

    private static bool ContainsLowercaseCharacter(string password) => password.Any(char.IsLower);

    private static bool ContainsNumber(string password) => password.Any(char.IsDigit);

    private static bool ContainsSpecialCharacter(string password) => password.Any(x => !char.IsLetterOrDigit(x));

    private static bool ContainsUppercaseCharacter(string password) => password.Any(char.IsUpper);
}