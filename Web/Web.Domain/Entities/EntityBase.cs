// <copyright file="EntityBase.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Domain.Entities;

using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents the base class for entities that require a unique identifier.
/// </summary>
[ExcludeFromCodeCoverage]
public abstract class EntityBase
{
    /// <summary>
    /// Gets a <see cref="Guid"/> that represents the unique identifier of the entity.
    /// </summary>
    public Guid Id { get; init; }
}
