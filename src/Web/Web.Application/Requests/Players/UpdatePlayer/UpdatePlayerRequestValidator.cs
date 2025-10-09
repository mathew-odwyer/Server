// <copyright file="UpdatePlayerRequestValidator.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.UpdatePlayer;

using FluentValidation;

/// <summary>
/// Provides validation for a <see cref="UpdatePlayerRequest"/>.
/// </summary>
public sealed class UpdatePlayerRequestValidator : AbstractValidator<UpdatePlayerRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UpdatePlayerRequestValidator"/> class.
    /// </summary>
    public UpdatePlayerRequestValidator()
    {
        this.RuleFor(x => x.UserAccountId)
            .NotEmpty()
            .WithMessage("User account ID must not be empty.");

        this.RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name must not be empty.");
    }
}
