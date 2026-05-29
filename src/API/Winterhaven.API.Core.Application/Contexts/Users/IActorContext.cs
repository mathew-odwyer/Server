using Winterhaven.API.Core.Domain.Entities.Users;

namespace Winterhaven.API.Core.Application.Contexts.Users;

/// <summary>
/// </summary>
public interface IActorContext
{
    /// <summary>
    /// </summary>
    public Actor Actor { get; }
}