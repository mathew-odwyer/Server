// <copyright file="LoginUserResponse.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.LoginUser;
public sealed record LoginUserResponse(string AccessToken, string RefreshToken);
