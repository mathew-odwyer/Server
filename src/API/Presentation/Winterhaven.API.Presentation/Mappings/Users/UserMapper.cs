using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Winterhaven.API.Core.Application.Requests.Users.LoginUser;
using Winterhaven.API.Core.Application.Requests.Users.RefreshToken;
using Winterhaven.API.Core.Application.Requests.Users.RegisterUser;
using Winterhaven.Common.DTOs.Users;

namespace Winterhaven.API.Presentation.Mappings.Users;

[ExcludeFromCodeCoverage]
internal sealed class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<RegisterUserRequestDto, RegisterUserRequest>();
        CreateMap<LoginUserRequestDto, LoginUserRequest>();
        CreateMap<RefreshTokenRequestDto, RefreshTokenRequest>();
        CreateMap<LoginUserResponse, LoginUserResponseDto>();
        CreateMap<RefreshTokenResponse, RefreshTokenResponseDto>();
    }
}