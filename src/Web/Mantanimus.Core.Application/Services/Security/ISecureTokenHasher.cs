// <copyright file="ISecureTokenHasher.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Core.Application.Services.Security;

/// <summary>
/// Defines an interface that provides a method used to hash secure tokens.
/// </summary>
public interface ISecureTokenHasher
{
    /// <summary>
    /// Hashes the specified <paramref name="token"/> for secure storage and comparison.
    /// </summary>
    /// <returns>
    /// Returns a <see cref="string"/> representing the hashed token.
    /// </returns>
    string HashSecureToken(string token);
}
