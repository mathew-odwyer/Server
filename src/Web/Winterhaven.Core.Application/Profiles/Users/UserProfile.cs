// <copyright file="UserProfile.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Profiles.Users;

using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Winterhaven.Core.Application.DTOs.Users;
using Winterhaven.Core.Application.Requests.Users.LoginUser;
using Winterhaven.Core.Application.Requests.Users.RefreshToken;
using Winterhaven.Core.Application.Requests.Users.RegisterUser;

[ExcludeFromCodeCoverage]
internal sealed class UserProfile : Profile
{
    public UserProfile()
    {
        this.CreateMap<RegisterUserRequestDto, RegisterUserRequest>();
        this.CreateMap<LoginUserRequestDto, LoginUserRequest>();
        this.CreateMap<RefreshTokenRequestDto, RefreshTokenRequest>();
    }
}
