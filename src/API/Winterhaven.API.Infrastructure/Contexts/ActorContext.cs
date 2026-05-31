using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Winterhaven.API.Core.Application.Contexts.Users;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Users;
using Winterhaven.API.Core.Domain.Exceptions;

namespace Winterhaven.API.Infrastructure.Contexts;

internal sealed class ActorContext : IActorContext
{
    private readonly IHttpContextAccessor? accessor;

    private readonly IActorRepository actorRepository;

    public ActorContext(IActorRepository actorRepository, IHttpContextAccessor? accessor = null)
    {
        this.actorRepository = actorRepository ?? throw new ArgumentNullException(nameof(actorRepository));
        this.accessor = accessor;
    }

    public Actor Actor
    {
        get
        {
            var user = accessor?.HttpContext?.User;

            if (user != null && user.Identity?.IsAuthenticated == true)
            {
                string? identifier = user.FindFirstValue("identifier");

                if (!string.IsNullOrWhiteSpace(identifier) && Guid.TryParse(identifier, out var actorId))
                {
                    return actorRepository.GetById(actorId) ?? throw new ResourceNotFoundException(nameof(Core.Domain.Entities.Users.Actor), actorId);
                }
            }

            return Actor.GetSystemActor();
        }
    }
}