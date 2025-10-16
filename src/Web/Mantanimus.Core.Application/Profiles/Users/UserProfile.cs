// <copyright file="UserProfile.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Mantanimus.Core.Application.Profiles.Users;

using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Mantanimus.Core.Application.DTOs.Users;
using Mantanimus.Core.Application.Requests.Users.RegisterUser;

[ExcludeFromCodeCoverage]
internal sealed class UserProfile : Profile
{
    public UserProfile()
    {
        this.CreateMap<RegisterUserRequestDto, RegisterUserRequest>();
    }
}
