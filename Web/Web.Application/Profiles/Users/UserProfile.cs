// <copyright file="UserProfile.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Profiles.Users;

using System.Diagnostics.CodeAnalysis;
using AutoMapper;
using Web.Application.DTOs.Users;
using Web.Application.Requests.Users.LoginUser;
using Web.Application.Requests.Users.RegisterUser;

/// <summary>
/// Provides mapping configuration for user-related objects.
/// </summary>
/// <seealso cref="Profile" />
[ExcludeFromCodeCoverage]
public sealed class UserProfile : Profile
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UserProfile"/> class.
    /// </summary>
    public UserProfile()
    {
        this.CreateMap<RegisterUserRequestDto, RegisterUserRequest>();
        this.CreateMap<LoginUserRequestDto, LoginUserRequest>();
    }
}
