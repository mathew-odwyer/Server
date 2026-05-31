using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Winterhaven.Gateway.Core.Application.Services.Users;

namespace Winterhaven.Gateway.Infrastructure.Pipeline.Handlers;

internal sealed class AccessTokenHandler : DelegatingHandler
{
    private readonly IHttpContextAccessor httpContextAccessor;

    public AccessTokenHandler(IHttpContextAccessor httpContextAccessor) =>
        this.httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var sessionContext = httpContextAccessor.HttpContext?.RequestServices?.GetService<IUserSessionContext>();

        if (sessionContext?.UserSession != null && !string.IsNullOrWhiteSpace(sessionContext.UserSession.AccessToken))
        {
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", sessionContext.UserSession.AccessToken);
        }

        return base.SendAsync(request, cancellationToken);
    }
}
