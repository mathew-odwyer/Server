namespace Winterhaven.Gateway.Presentation.Services;

using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;

internal sealed class JsonRpcUserSessionManager
{
    private readonly ILogger<JsonRpcUserSessionManager> logger;

    private readonly ConcurrentDictionary<Guid, string> userAccountIdToConnectionIdMap;

    public JsonRpcUserSessionManager(ILogger<JsonRpcUserSessionManager> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.userAccountIdToConnectionIdMap = [];
    }

    public void AddUserSession(Guid userAccountId, string connectionId)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(connectionId);

        this.logger.LogDebug("Adding user session for user with ID: '{UserAccountId}'...", userAccountId);

        if (!this.userAccountIdToConnectionIdMap.TryAdd(userAccountId, connectionId))
        {
            this.logger.LogWarning("Failed to add user session for user with ID: '{UserAccountId}', user already added.", userAccountId);
        }

        this.logger.LogDebug("User session added for user with ID: '{UserAccountId}'", userAccountId);
    }

    public void RemoveUserSession(Guid userAccountId)
    {
        this.logger.LogDebug("Removing user session for user with ID: '{UserAccountId}'...", userAccountId);

        if (!this.userAccountIdToConnectionIdMap.TryRemove(userAccountId, out _))
        {
            this.logger.LogWarning("Failed to remove user session for user with ID: '{UserAccountId}', user has not been added.", userAccountId);
        }

        this.logger.LogDebug("User session removed for user with ID: '{UserAccountId}'", userAccountId);
    }
}