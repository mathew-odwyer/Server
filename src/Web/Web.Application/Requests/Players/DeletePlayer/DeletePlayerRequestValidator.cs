// <copyright file="DeletePlayerRequestValidator.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.DeletePlayer;

using FluentValidation;

/// <summary>
/// Provides validation for a <see cref="DeletePlayerRequest"/>.
/// </summary>
public sealed class DeletePlayerRequestValidator : AbstractValidator<DeletePlayerRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeletePlayerRequestValidator"/> class.
    /// </summary>
    public DeletePlayerRequestValidator()
    {
        this.RuleFor(x => x.UserAccountId)
            .NotEmpty()
            .WithMessage("User account ID must not be empty.");

        this.RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name must not be empty.");
    }
}
