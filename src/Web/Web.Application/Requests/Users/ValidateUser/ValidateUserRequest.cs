// <copyright file="ValidateUserRequest.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Application.Requests.Users.ValidateUser;

using MediatR;

public sealed class ValidateUserRequest : IRequest<ValidateUserResponse>
{
    public required string ClientToken { get; init; }
}
