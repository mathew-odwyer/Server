namespace Winterhaven.Gateway.Infrastructure.HTTP.Handlers;

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.Gateway.Core.Application.Services.Sessions;

internal sealed class AccessTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public AccessTokenHandler(IHttpContextAccessor httpContextAccessor)
    {
        this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var sessionContext = this.httpContextAccessor.HttpContext?.RequestServices.GetRequiredService<ISessionContext>();
        
        if (sessionContext?.Session is not null && !string.IsNullOrEmpty(sessionContext.Session.AccessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sessionContext.Session.AccessToken);
        }

        return base.SendAsync(request, cancellationToken);
    }
}