namespace Winterhaven.API.Core.Application.Validators.Users;

using FluentValidation;
using System.Linq;

internal sealed class UsernameValidator : AbstractValidator<string>
{
    internal UsernameValidator()
    {
        this.RuleFor(x => x).NotEmpty().WithMessage("Username cannot be empty.");
        this.RuleFor(x => x).MinimumLength(3).WithMessage("Username must be at least 3 characters.");
        this.RuleFor(x => x).MaximumLength(12).WithMessage("Username must be no more than 12 characters.");
        this.RuleFor(x => x).Must(ContainsLegalCharacters).WithMessage("Username can only contain alphanumerical characters (and '-' or '_')");
    }

    private static bool ContainsLegalCharacters(string username)
    {
        return username.All(x => char.IsLetterOrDigit(x) || x == '-' || x == '_');
    }
}