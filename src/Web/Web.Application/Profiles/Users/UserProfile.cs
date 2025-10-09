// <copyright file="UserProfile.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Profiles.Users;

using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Web.Application.DTOs.Users;
using Web.Application.Requests.Users.LoginUser;
using Web.Application.Requests.Users.RegisterUser;

[ExcludeFromCodeCoverage]
internal sealed class UserProfile : Profile
{
    public UserProfile()
    {
        this.CreateMap<RegisterUserRequestDto, RegisterUserRequest>();
        this.CreateMap<LoginUserRequestDto, LoginUserRequest>();
    }
}
