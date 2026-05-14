namespace Winterhaven.API.Presentation.Mappings.Users;

using AutoMapper;
using System.Diagnostics.CodeAnalysis;
using Winterhaven.API.Core.Application.Requests.Users.LoginUser;
using Winterhaven.API.Core.Application.Requests.Users.RefreshToken;
using Winterhaven.API.Core.Application.Requests.Users.RegisterUser;
using Winterhaven.API.Common.DTOs.Users;

[ExcludeFromCodeCoverage]
internal sealed class UserMapper : Profile
{
    public UserMapper()
    {
        this.CreateMap<RegisterUserRequestDto, RegisterUserRequest>();
        this.CreateMap<LoginUserRequestDto, LoginUserRequest>();
        this.CreateMap<RefreshTokenRequestDto, RefreshTokenRequest>();
    }
}