using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Winterhaven.Gateway.Presentation.Middleware;

namespace Winterhaven.Gateway.Presentation;

internal sealed class Startup
{
    public Startup(IConfiguration configuration) => Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

    public IConfiguration Configuration { get; }

    public void Configure(IApplicationBuilder application, IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(application);
        ArgumentNullException.ThrowIfNull(environment);

        // Tell ASP.NET Core to trust reverse proxy headers.
        var options = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
        };

        //// Clear default network/proxy restrictions since Caddy runs on Docker's
        //// internal bridge network, which ASP.NET Core doesn't trust by default.
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();

        // Must be first so all subsequent middleware sees the real client IP/protocol.
        application.UseForwardedHeaders(options);

        if (!environment.IsDevelopment())
        {
            // FIXME: See #112.
            application.UseRateLimiter();
        }

        application.UseWebSockets(new WebSocketOptions
        {
            //// How often the server automatically sends small "ping" messages to the other side to verify the connection is still active and functioning.
            //// This is by default set to 30 as we also have the HealthRpcTarget that supports ping and heartbeat requests anyways.
            KeepAliveInterval = TimeSpan.FromSeconds(Configuration.GetValue("WebSocketOptions:KeepAliveInterval", 30.0)),
        });

        application.UseMiddleware<WebSocketMiddleware>();

        application.UseRouting();
        application.UseEndpoints(x => x.MapControllers());
    }

    public void ConfigureServices(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddRateLimiter();
        services.AddControllers();
    }
}
