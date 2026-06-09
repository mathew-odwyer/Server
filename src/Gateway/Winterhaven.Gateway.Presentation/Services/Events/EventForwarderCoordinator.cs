using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using StreamJsonRpc;

namespace Winterhaven.Gateway.Presentation.Services.Events;

internal sealed class EventForwarderCoordinator : IEventForwarderCoordinator
{
    private readonly IEnumerable<EventForwarderBase> eventForwarders;

    private readonly ILogger<EventForwarderCoordinator> logger;

    public EventForwarderCoordinator(ILogger<EventForwarderCoordinator> logger, IEnumerable<EventForwarderBase> eventForwarders)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
        this.eventForwarders = eventForwarders ?? throw new ArgumentNullException(nameof(eventForwarders));
    }

    public async Task StartAllForwardersAsync(JsonRpc rpc, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(rpc);

        if (rpc.IsDisposed)
        {
            return;
        }

        try
        {
            foreach (var eventForwarder in this.eventForwarders)
            {
                await eventForwarder.StartAsync(rpc, cancellationToken).ConfigureAwait(false);
            }
        }
        catch (Exception ex)
        {
            // Ensure exceptions from the fire-and-forget task are observed and logged.
            this.logger.LogError(ex, "Failed to start one or more event forwarders after session establishment.");
        }
    }
}
