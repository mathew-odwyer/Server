// <copyright file="GetMapRequestValidator.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Requests.Maps.GetMap;

using FluentValidation;

public sealed class GetMapRequestValidator : AbstractValidator<GetMapRequest>
{
    public GetMapRequestValidator()
    {
        this.RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("You must provide a map name.");
    }
}
