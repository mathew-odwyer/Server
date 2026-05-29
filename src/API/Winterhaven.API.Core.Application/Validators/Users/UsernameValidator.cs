using System.Linq;
using FluentValidation;

namespace Winterhaven.API.Core.Application.Validators.Users;

internal sealed class UsernameValidator : AbstractValidator<string>
{
    internal UsernameValidator()
    {
        RuleFor(x => x).NotEmpty().WithMessage("Username cannot be empty.");
        RuleFor(x => x).MinimumLength(3).WithMessage("Username must be at least 3 characters.");
        RuleFor(x => x).MaximumLength(12).WithMessage("Username must be no more than 12 characters.");
        RuleFor(x => x).Must(ContainsLegalCharacters).WithMessage("Username can only contain alphanumerical characters (and '-' or '_')");
    }

    private static bool ContainsLegalCharacters(string username) => username.All(x => char.IsLetterOrDigit(x) || x == '-' || x == '_');
}