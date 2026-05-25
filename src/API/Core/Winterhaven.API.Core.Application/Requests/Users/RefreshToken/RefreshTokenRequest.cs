namespace Winterhaven.API.Core.Application.Requests.Users.RefreshToken;

using MediatR;
using System.Diagnostics.CodeAnalysis;
using Winterhaven.API.Core.Domain.Attributes.Users;

/// <summary>
///   Represents a request used to refresh a JSON Web Token for an existing user account.
/// </summary>
/// <seealso cref="IRequest{LoginUserResponse}"/>
/// <seealso cref="IBaseRequest"/>
/// <param name="RefreshToken">
///   The refresh token used to refresh the JSON Web Token for the user account.
/// </param>
[Authorize]
[ExcludeFromCodeCoverage]
public sealed record RefreshTokenRequest(string RefreshToken)
    : IRequest<RefreshTokenResponse>;