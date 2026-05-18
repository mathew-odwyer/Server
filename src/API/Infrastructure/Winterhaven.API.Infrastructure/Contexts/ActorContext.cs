namespace Winterhaven.API.Infrastructure.Contexts;

using Microsoft.AspNetCore.Http;
using System;
using System.Security.Claims;
using Winterhaven.API.Core.Application.Contexts.Users;
using Winterhaven.API.Core.Application.Work.Users;
using Winterhaven.API.Core.Domain.Entities.Users;
using Winterhaven.API.Core.Domain.Exceptions;

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
            var user = this.accessor?.HttpContext?.User;

            if (user != null && user.Identity?.IsAuthenticated == true)
            {
                string? identifier = user.FindFirstValue("identifier");

                if (!string.IsNullOrWhiteSpace(identifier) && Guid.TryParse(identifier, out var userAccountId))
                {
                    return this.actorRepository.GetById(userAccountId) ?? throw new ResourceNotFoundException(nameof(Core.Domain.Entities.Users.Actor), userAccountId);
                }
            }

            return Actor.GetSystemActor();
        }
    }
}