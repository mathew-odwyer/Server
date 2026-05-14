namespace Winterhaven.Gateway.Core.Application.Requests.Users.UserLogin;

using MediatR;

public sealed record UserLoginRequest(
    string Username,
    string Password) : IRequest<UserLoginResponse>;
