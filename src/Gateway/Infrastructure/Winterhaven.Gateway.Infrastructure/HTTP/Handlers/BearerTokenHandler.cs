namespace Winterhaven.Gateway.Infrastructure.HTTP.Handlers;

using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.Gateway.Core.Application.Services.Sessions;

internal sealed class BearerTokenHandler : DelegatingHandler
{
    private readonly ISessionContext sessionContext;

    public BearerTokenHandler(ISessionContext sessionContext)
    {
        this.sessionContext = sessionContext ?? throw new ArgumentNullException(nameof(sessionContext));
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        if (this.sessionContext.AccessToken is { } token)
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }

        return base.SendAsync(request, cancellationToken);
    }
}