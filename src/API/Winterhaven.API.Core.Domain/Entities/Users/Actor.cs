using System;
using System.Diagnostics.CodeAnalysis;

namespace Winterhaven.API.Core.Domain.Entities.Users;

/// <summary>
///   Enumerates the available actor types.
/// </summary>
public enum ActorType
{
    /// <summary>
    ///   The actor is an authenticated user.
    /// </summary>
    User = 0,

    /// <summary>
    ///   The actor is the system.
    /// </summary>
    System = 1,
}

/// <summary>
///   Represents an actor (a user, the system, etc).
/// </summary>
[ExcludeFromCodeCoverage]
public class Actor : EntityBase
{
    private static Actor? systemActor;

    /// <summary>
    ///   Gets the name of the actor.
    /// </summary>
    /// <value>
    ///   The name of the actor.
    /// </value>
    public required string Name { get; init; }

    /// <summary>
    ///   Gets the type of the actor.
    /// </summary>
    /// <value>
    ///   The type of the actor.
    /// </value>
    public ActorType Type { get; init; }

    /// <summary>
    ///   Gets the system actor.
    /// </summary>
    /// <returns>
    ///   Returns an <see cref="Actor"/> that represents the system actor.
    /// </returns>
    public static Actor GetSystemActor()
    {
        const string systemUid = "AC892565-1CB6-4AE2-9616-2AA5E2385168";

        return systemActor ??= new()
        {
            Id = Guid.Parse(systemUid),
            Name = "SYSTEM",
            Type = ActorType.System,
        };
    }
}
