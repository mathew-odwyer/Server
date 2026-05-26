using System;

namespace Winterhaven.API.Core.Domain.Entities;

/// <summary>
///   Represents the base class for entities that require a unique identifier.
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    ///   Gets a <see cref="Guid"/> that represents the unique identifier of the entity.
    /// </summary>
    public Guid Id { get; init; }
}