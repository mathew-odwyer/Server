namespace Winterhaven.Gateway.Presentation.Validation.Health;

using FluentValidation;
using Winterhaven.Gateway.Presentation.Targets.Health;

public sealed class HealthPingRpcParametersValidator : AbstractValidator<HealthPingRpcParameters>
{
    public HealthPingRpcParametersValidator()
    {
        this.RuleFor(x => x.TimeStamp)
            .NotNull()
            .GreaterThan(0);
    }
}