namespace Winterhaven.API.Core.Application.Services.Security;

/// <summary>
/// Defines an interface that provides a method used to hash secure tokens.
/// </summary>
public interface ISecureTokenHasher
{
    /// <summary>
    /// Hashes the specified <paramref name="token"/> for secure storage and comparison.
    /// </summary>
    /// <returns>Returns a <see cref="string"/> representing the hashed token.</returns>
    string HashSecureToken(string token);
}