namespace Winterhaven.API.Core.Application.Requests.Users.RefreshToken;

using MediatR;
using Winterhaven.API.Core.Domain.Attributes.Users;
using Winterhaven.API.Core.Domain.Entities.Users;

/// <summary>
/// Represents a request used to refresh a JSON Web Token for an existing <see cref="UserAccount"/>.
/// </summary>
/// <seealso cref="IRequest{LoginUserResponse}"/>
/// <seealso cref="IBaseRequest"/>
/// <param name="RefreshToken">
/// The refresh token used to refresh the JSON Web Token for the <see cref="UserAccount"/>.
/// </param>
[Authorize]
public sealed record RefreshTokenRequest(string RefreshToken)
    : IRequest<RefreshTokenResponse>;