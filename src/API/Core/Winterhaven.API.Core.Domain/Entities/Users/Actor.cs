namespace Winterhaven.API.Core.Domain.Entities.Users;

using System;

public enum ActorType
{
    User,
    System,
}

public class Actor : EntityBase
{
    private static readonly Guid SystemUniqueIdentifier = Guid.Parse("AC892565-1CB6-4AE2-9616-2AA5E2385168");

    public static readonly Actor SystemActor = new()
    {
        Id = SystemUniqueIdentifier,
        Name = "SYSTEM",
        Type = ActorType.System,
    };

    public required string Name { get; init; }

    public ActorType Type { get; init; }
}