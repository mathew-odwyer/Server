// <copyright file="CreatePlayerRequestValidator.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.CreatePlayer;

using FluentValidation;
using Web.Application.Validators.Users;

/// <summary>
/// Provides validation for a <see cref="CreatePlayerRequest"/>.
/// </summary>
public sealed class CreatePlayerRequestValidator : AbstractValidator<CreatePlayerRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="CreatePlayerRequestValidator"/> class.
    /// </summary>
    public CreatePlayerRequestValidator()
    {
        this.RuleFor(x => x.UserAccountId)
            .NotEmpty()
            .WithMessage("User account ID must not be empty.");

        this.RuleFor(x => x.Name)
            .SetValidator(new UsernameValidator());
    }
}
