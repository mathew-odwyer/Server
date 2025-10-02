// <copyright file="PasswordValidator.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Validators.Users;

using System.Linq;
using FluentValidation;

//// TODO: Unit tests

/// <summary>
/// Provides an implementation of <see cref="AbstractValidator{T}"/> that validates password strength.
/// </summary>
internal sealed class PasswordValidator : AbstractValidator<string>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PasswordValidator"/> class.
    /// </summary>
    internal PasswordValidator()
    {
        this.RuleFor(x => x)
            .NotEmpty().WithMessage("Password cannot be empty.")
            .MinimumLength(12).WithMessage("Password must be at least 12 characters.")
            .Must(ContainsUppercaseCharacter).WithMessage("Password must contain at least one uppercase character.")
            .Must(ContainsLowercaseCharacter).WithMessage("Password must contain at least one lowercase character.")
            .Must(ContainsNumber).WithMessage("Password must contain at least one number.")
            .Must(ContainsSpecialCharacter).WithMessage("Password must contain at least one special character.");
    }

    /// <summary>
    /// Determines whether the password contains at least one lowercase character.
    /// </summary>
    /// <param name="password">
    /// Specifies a <see cref="string"/> representing the password to validate.
    /// </param>
    /// <returns>
    /// Returns <see cref="bool"/> indicating whether the password contains at least one lowercase character.
    /// </returns>
    private static bool ContainsLowercaseCharacter(string password)
    {
        return password.Any(char.IsLower);
    }

    /// <summary>
    /// Determines whether the password contains at least one number.
    /// </summary>
    /// <param name="password">
    /// Specifies a <see cref="string"/> representing the password to validate.
    /// </param>
    /// <returns>
    /// Returns <see cref="bool"/> indicating whether the password contains at least one number.
    /// </returns>
    private static bool ContainsNumber(string password)
    {
        return password.Any(char.IsDigit);
    }

    /// <summary>
    /// Determines whether the password contains at least one special character.
    /// </summary>
    /// <param name="password">
    /// Specifies a <see cref="string"/> representing the password to validate.
    /// </param>
    /// <returns>
    /// Returns <see cref="bool"/> indicating whether the password contains at least one special character.
    /// </returns>
    private static bool ContainsSpecialCharacter(string password)
    {
        return password.Any(x => !char.IsLetterOrDigit(x));
    }

    /// <summary>
    /// Determines whether the password contains at least one uppercase character.
    /// </summary>
    /// <param name="password">
    /// Specifies a <see cref="string"/> representing the password to validate.
    /// </param>
    /// <returns>
    /// Returns <see cref="bool"/> indicating whether the password contains at least one uppercase character.
    /// </returns>
    private static bool ContainsUppercaseCharacter(string password)
    {
        return password.Any(char.IsUpper);
    }
}
