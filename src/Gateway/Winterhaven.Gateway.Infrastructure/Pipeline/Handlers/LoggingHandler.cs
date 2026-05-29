using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Winterhaven.Gateway.Infrastructure.Pipeline.Handlers;

[ExcludeFromCodeCoverage(Justification = "Logging")]
internal sealed class LoggingHandler : DelegatingHandler
{
    private readonly ILogger<LoggingHandler> logger;

    public LoggingHandler(ILogger<LoggingHandler> logger) =>
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        logger.LogTrace("HTTP {Method} {Uri}", request.Method, request.RequestUri);
        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        logger.LogTrace("HTTP {StatusCode} from {Uri}", (int)response.StatusCode, request.RequestUri);

        return response;
    }
}
