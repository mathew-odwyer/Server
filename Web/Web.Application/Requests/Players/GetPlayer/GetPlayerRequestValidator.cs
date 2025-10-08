// <copyright file="GetPlayerRequestValidator.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.GetPlayer;

using FluentValidation;

/// <summary>
/// Provides validation for a <see cref="GetPlayerRequest"/>.
/// </summary>
public sealed class GetPlayerRequestValidator : AbstractValidator<GetPlayerRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetPlayerRequestValidator"/> class.
    /// </summary>
    public GetPlayerRequestValidator()
    {
        this.RuleFor(x => x.UserAccountId)
            .NotEmpty()
            .WithMessage("User account ID must not be empty.");

        this.RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Name must not be empty.");
    }
}
