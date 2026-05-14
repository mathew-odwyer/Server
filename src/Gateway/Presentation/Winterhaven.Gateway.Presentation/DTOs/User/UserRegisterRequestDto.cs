namespace Winterhaven.Gateway.Presentation.DTOs.User;

using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
public sealed record UserRegisterRequestDto(
    string Username,
    string Password,
    string EmailAddress);