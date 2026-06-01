using System;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;

namespace Winterhaven.Gateway.Integrations.Services.Users;

internal sealed class UserSessionFactory
{
    static UserSessionFactory() => DummySession = new UserSession(
            UserAccountId: Guid.NewGuid(),
            Username: "Dummy Dumb Dumb",
            AccessToken: "accessToken",
            ExpiresAt: DateTimeOffset.UtcNow.AddMinutes(15));

    public static UserSession DummySession { get; }
}
