using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Winterhaven.Events.Extensions;
using Winterhaven.Gateway.Infrastructure.Extensions;
using Winterhaven.Gateway.Presentation.Extensions;
using Winterhaven.Gateway.Presentation.Middleware;
using Winterhaven.Gateway.Presentation.Services.Events;
using Winterhaven.Gateway.Presentation.Services.Events.Players;
using Winterhaven.Gateway.Presentation.Services.Sessions;
using Winterhaven.Gateway.Presentation.Services.Targets;
using Winterhaven.Gateway.Presentation.Targets.Chat;
using Winterhaven.Gateway.Presentation.Targets.Health;
using Winterhaven.Gateway.Presentation.Targets.Players;
using Winterhaven.Gateway.Presentation.Targets.Users;

namespace Winterhaven.Gateway.Presentation;

[ExcludeFromCodeCoverage]
internal sealed class Startup
{
    public Startup(IConfiguration configuration)
    {
        this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

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
            KeepAliveInterval = TimeSpan.FromSeconds(this.Configuration.GetValue("WebSocketOptions:KeepAliveInterval", 30.0)),
        });

        application.UseMiddleware<WebSocketMiddleware>();

        application.UseRouting();
        application.UseEndpoints(x => x.MapControllers());
    }

    public void ConfigureServices(IServiceCollection services)
    {
        ArgumentNullException.ThrowIfNull(services);

        services.AddSingleton(TimeProvider.System);

        services.AddRateLimiter();
        services.AddControllers();

        services.AddScoped<IJsonRpcTargetRegistrar, JsonRpcTargetRegistrar>();
        services.AddScoped<IEventForwarderCoordinator, EventForwarderCoordinator>();
        services.AddScoped<IWebSocketRpcSession, WebSocketRpcSession>();

        services.AddRpcSessionTarget<HealthRpcTarget>();
        services.AddRpcSessionTarget<UserRpcTarget>();
        services.AddRpcSessionTarget<PlayerRpcTarget>();
        services.AddRpcSessionTarget<ChatRpcTarget>();

        services.AddEventForwarder<PlayerEventForwarder>();

        services.AddEventServices(this.Configuration);
        services.AddGatewayInfrastructureServices(this.Configuration);

        services.AddHttpContextAccessor();
    }
}
