// <copyright file="IUserAccountContext.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Core.Application.Contexts.Users;

using Mantanimus.Core.Domain.Entities.Users;

/// <summary>
/// Defines an interface that provides a context for the current user account.
/// </summary>
public interface IUserAccountContext
{
    /// <summary>
    /// Gets a <see cref="UserAccount"/> that represents the current user.
    /// </summary>
    /// <value>
    /// The <see cref="UserAccount"/> that represents the current user.
    /// </value>
    UserAccount User { get; }
}
