namespace Winterhaven.Gateway.Infrastructure.HTTP.Handlers;

using Microsoft.Extensions.Logging;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

internal sealed class LoggingHandler : DelegatingHandler
{
    private readonly ILogger<LoggingHandler> logger;

    public LoggingHandler(ILogger<LoggingHandler> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        logger.LogDebug("HTTP {Method} {Uri}", request.Method, request.RequestUri);
        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        logger.LogDebug("HTTP {StatusCode} from {Uri}", (int)response.StatusCode, request.RequestUri);

        return response;
    }
}
