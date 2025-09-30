// <copyright file="GetPlayersRequestValidator.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.GetPlayers;

using FluentValidation;

public sealed class GetPlayersRequestValidator : AbstractValidator<GetPlayersRequest>
{
    public GetPlayersRequestValidator()
    {
        this.RuleFor(x => x.UserAccountId)
            .NotEmpty()
            .WithMessage("User account ID must not be empty.");
    }
}
