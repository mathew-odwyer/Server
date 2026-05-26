using MediatR;
using Winterhaven.API.Core.Domain.Attributes.Users;

namespace Winterhaven.API.Core.Application.Requests.Users.LogoutUser;
/// <summary>
///   Represents a request used to logout an existing user account.
/// </summary>
/// <seealso cref="IRequest{LoginUserResponse}"/>
/// <seealso cref="IBaseRequest"/>
[Authorize]
public sealed record LogoutUserRequest
    : IRequest;