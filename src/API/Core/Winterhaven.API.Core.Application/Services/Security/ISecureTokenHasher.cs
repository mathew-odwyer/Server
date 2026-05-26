namespace Winterhaven.API.Core.Application.Services.Security;

/// <summary>
///   Defines an interface that provides a method used to hash secure tokens.
/// </summary>
public interface ISecureTokenHasher
{
    /// <summary>
    ///   Hashes the specified <paramref name="token"/> for secure storage and comparison.
    /// </summary>
    /// <param name="token">
    /// </param>
    /// <returns>
    ///   Returns a <see cref="string"/> representing the hashed token.
    /// </returns>
    public string HashSecureToken(string token);
}