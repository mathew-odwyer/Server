namespace Winterhaven.Gateway.Presentation.Mappings.Users;

using AutoMapper;
using Winterhaven.Gateway.Core.Application.Requests.Users.UserLogin;
using Winterhaven.Gateway.Core.Application.Requests.Users.UserRegister;
using Winterhaven.Gateway.Presentation.DTOs.User;

public sealed class UserMapper : Profile
{
    public UserMapper()
    {
        this.CreateMap<UserRegisterRequestDto, UserRegisterRequest>();
        this.CreateMap<UserRegisterResponse, UserRegisterResponseDto>();

        this.CreateMap<UserLoginRequestDto, UserLoginRequest>();
        this.CreateMap<UserLoginResponse, UserLoginResponseDto>();
    }
}
