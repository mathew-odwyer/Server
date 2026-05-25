namespace Winterhaven.API.Core.Application.Contexts.Users;

using Winterhaven.API.Core.Domain.Entities.Users;

public interface IActorContext
{
    Actor Actor { get; }
}