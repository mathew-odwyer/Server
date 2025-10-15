// <copyright file="UserClientToken.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Domain.Entities.Users;

public sealed class UserClientToken : AuditableEntityBase
{
    public required DateTime ExpirationDate { get; init; }

    public required string HashedToken { get; init; }

    public bool IsRevoked { get; set; }

    public required string UserAccountId { get; init; }
}
