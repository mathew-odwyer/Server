namespace Winterhaven.Gateway.Presentation.DTOs.User;

public sealed record UserLoginRequestDto(
    string Username,
    string Password);
