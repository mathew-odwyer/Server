// <copyright file="UsernameValidator.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Validators.Users;

using FluentValidation;

internal sealed class UsernameValidator : AbstractValidator<string>
{
    internal UsernameValidator()
    {
        this.RuleFor(x => x)
            .NotEmpty().WithMessage("Name cannot be empty.")
            .MinimumLength(3).WithMessage("Name must be at least 3 characters.")
            .MaximumLength(12).WithMessage("Name must be no more than 12 characters.")
            .Must(ContainsLegalCharacters).WithMessage("Name can only contain alphanumerical characters (and '-' or '_')");
    }

    private static bool ContainsLegalCharacters(string username)
    {
        return username.All(x => char.IsLetterOrDigit(x) || x == '-' || x == '_');
    }
}
