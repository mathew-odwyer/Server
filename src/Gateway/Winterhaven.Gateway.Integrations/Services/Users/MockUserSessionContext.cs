using System;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Core.Domain.ValueObjects.Users;

namespace Winterhaven.Gateway.Integrations.Services.Users;

internal sealed class MockUserSessionContext : IUserSessionContext
{
    public event EventHandler<EventArgs> Established;

    public event EventHandler<EventArgs> Invalidated;

    public event EventHandler<EventArgs> Refreshed;

    public bool IsAuthenticated { get; set; }

    public UserSession UserSession { get; set; }

    public void Dispose()
    {
    }
}
