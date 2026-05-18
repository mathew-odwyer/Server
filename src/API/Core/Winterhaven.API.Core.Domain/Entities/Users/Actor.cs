namespace Winterhaven.API.Core.Domain.Entities.Users;

using System;

public enum ActorType
{
    User,

    System,
}

public class Actor : EntityBase
{
    private static Actor? systemActor;

    public required string Name { get; init; }

    public ActorType Type { get; init; }

    public static Actor GetSystemActor()
    {
        const string SystemUid = "AC892565-1CB6-4AE2-9616-2AA5E2385168";

        return systemActor ??= new()
        {
            Id = Guid.Parse(SystemUid),
            Name = "SYSTEM",
            Type = ActorType.System,
        };
    }
}