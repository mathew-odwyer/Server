// <copyright file="AuditableEntityBase.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Core.Domain.Entities;

using System;
using System.Diagnostics.CodeAnalysis;

/// <summary>
/// Represents an abstract base class for entities that need auditing information, including tracking creation and modification details.
/// </summary>
[ExcludeFromCodeCoverage]
public abstract class AuditableEntityBase : EntityBase
{
    /// <summary>
    /// Gets or sets a <see cref="Guid"/> representing the identifier of the user who created the entity.
    /// </summary>
    /// <value>
    /// The <see cref="Guid"/> representing identifier of the user who created the entity.
    /// </value>
    public Guid CreatedBy { get; set; }

    /// <summary>
    /// Gets or sets a <see cref="DateTime"/> representing when the entity was created.
    /// </summary>
    /// <value>
    /// The <see cref="DateTime"/> representing when the entity was created.
    /// </value>
    public DateTime CreatedOn { get; set; }

    /// <summary>
    /// Gets or sets a <see cref="Guid"/> representing the identifier of the user who last modified the entity.
    /// </summary>
    /// <value>
    /// The <see cref="Guid"/> representing the identifier of the user who last modified the entity.
    /// </value>
    public Guid ModifiedBy { get; set; }

    /// <summary>
    /// Gets or sets a <see cref="DateTime"/> representing when the entity was last modified.
    /// </summary>
    /// <value>
    /// The <see cref="DateTime"/> representing when the entity was last modified.
    /// </value>
    public DateTime ModifiedOn { get; set; }
}
