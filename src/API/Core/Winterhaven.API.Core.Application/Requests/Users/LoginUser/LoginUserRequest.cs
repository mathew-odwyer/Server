namespace Winterhaven.API.Core.Application.Requests.Users.LoginUser;

using MediatR;
using Winterhaven.API.Core.Domain.Entities.Users;

/// <summary>
/// Represents a request used to authenticate an existing user account.
/// </summary>
/// <seealso cref="IRequest{LoginUserResponse}"/>
/// <seealso cref="IBaseRequest"/>
/// <param name="Username">
/// The username associated with the user account attempting to log in. This value must
/// not be empty.
/// </param>
/// <param name="Password">
/// The password associated with the user account attempting to log in. This value must
/// not be empty.
/// </param>
public sealed record LoginUserRequest(
    string Username,
    string Password
    ) : IRequest<LoginUserResponse>;