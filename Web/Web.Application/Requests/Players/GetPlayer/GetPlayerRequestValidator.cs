// <copyright file="GetPlayerRequestValidator.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Players.GetPlayer;

using FluentValidation;

public sealed class GetPlayerRequestValidator : AbstractValidator<GetPlayerRequest>
{
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
