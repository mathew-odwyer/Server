namespace Winterhaven.Gateway.Presentation.Targets;

using AutoMapper;
using MediatR;

internal abstract class RpcTargetBase
{
    protected RpcTargetBase(IMediator mediator, IMapper mapper)
    {
        this.Mediator = mediator ?? throw new System.ArgumentNullException(nameof(mediator));
        this.Mapper = mapper ?? throw new System.ArgumentNullException(nameof(mapper));
    }

    protected IMediator Mediator { get; init; }

    protected IMapper Mapper { get; init; }
}