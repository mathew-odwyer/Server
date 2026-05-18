namespace Winterhaven.API.Core.Application.Requests.Maps.GetMap;

using FluentValidation;

/// <summary>
///   Provides validation logic for a <see cref="GetMapRequest"/>.
/// </summary>
public sealed class GetMapRequestValidator : AbstractValidator<GetMapRequest>
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="GetMapRequestValidator"/> class.
    /// </summary>
    public GetMapRequestValidator()
    {
        this.RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("You must provide a map name.");
    }
}