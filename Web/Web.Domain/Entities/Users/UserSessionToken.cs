// <copyright file="UserSessionToken.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Web.Domain.Entities.Users;

public sealed class UserSessionToken : AuditableEntityBase
{
    public DateTime ExpirationDate { get; set; }

    public required string HashedRefreshToken { get; init; }

    public byte[] RowVersion { get; private set; } = [];

    public required Guid SessionId { get; init; }

    public required string UserAccountId { get; init; }
}
