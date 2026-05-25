namespace Winterhaven.API.Core.Application.Requests.Users.LogoutUser;

using MediatR;
using System.Diagnostics.CodeAnalysis;
using Winterhaven.API.Core.Domain.Attributes.Users;

/// <summary>
///   Represents a request used to logout an existing user account.
/// </summary>
/// <seealso cref="IRequest{LoginUserResponse}"/>
/// <seealso cref="IBaseRequest"/>
[Authorize]
[ExcludeFromCodeCoverage]
public sealed record LogoutUserRequest
    : IRequest;