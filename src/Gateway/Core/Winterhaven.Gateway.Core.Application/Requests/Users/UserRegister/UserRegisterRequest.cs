namespace Winterhaven.Gateway.Core.Application.Requests.Users.UserRegister;

using MediatR;

/// <summary>
/// Represents a request to register a new user.
/// </summary>
/// <param name="EmailAddress">
/// Specifies a <see cref="string"/> that represents the email address of the user to be registered.
/// </param>
/// <param name="Username">
/// Specifies a <see cref="string"/> that represents the username of the user to be registered.
/// </param>
/// <param name="Password">
/// Specifies a <see cref="string"/> that represents the password of the user to be registered.
/// </param>
public sealed record UserRegisterRequest(
    string EmailAddress,
    string Username,
    string Password) : IRequest<UserRegisterResponse>;