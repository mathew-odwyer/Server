// <copyright file="GetPlayersRequestValidator.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.GetPlayers;

using FluentValidation;

/// <summary>
/// Provides validation for a <see cref="GetPlayersRequest"/>.
/// </summary>
public sealed class GetPlayersRequestValidator : AbstractValidator<GetPlayersRequest>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="GetPlayersRequestValidator"/> class.
    /// </summary>
    public GetPlayersRequestValidator()
    {
        this.RuleFor(x => x.UserAccountId)
            .NotEmpty()
            .WithMessage("User account ID must not be empty.");
    }
}
