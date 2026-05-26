using Winterhaven.API.Core.Domain.ValueObjects.Users;

namespace Winterhaven.API.Core.Application.Services.Security;

/// <summary>
///   Defines an interface that provides functionality to generate secure tokens (such as JSON Web Tokens).
/// </summary>
public interface ISecureTokenFactory
{
    /// <summary>
    ///   Generates a new secure token.
    /// </summary>
    /// <returns>
    ///   Returns a <see cref="string"/> representing the generated secure token.
    /// </returns>
    public string GenerateSecureToken();

    /// <summary>
    ///   Generates a new <see cref="UserToken"/> using the specified <paramref name="parameters"/>.
    /// </summary>
    /// <param name="parameters">
    ///   Specifies a <see cref="UserTokenParameters"/> instance containing the user information required to generate the token.
    /// </param>
    /// <returns>
    ///   Returns a <see cref="UserToken"/> containing the generated access and refresh tokens.
    /// </returns>
    public UserToken GenerateUserToken(UserTokenParameters parameters);
}