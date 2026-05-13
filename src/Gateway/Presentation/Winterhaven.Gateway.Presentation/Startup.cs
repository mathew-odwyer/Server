namespace Winterhaven.Gateway.Presentation;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics.CodeAnalysis;
using Winterhaven.Gateway.Presentation.Middleware;
using Winterhaven.Gateway.Presentation.Services;
using Winterhaven.Gateway.Presentation.Targets;
using Winterhaven.Gateway.Presentation.Targets.Health;
using Winterhaven.Gateway.Presentation.Targets.Users;

/*
    TODO: Documentation
    TODO: REVIEW, PUSH AND MERGE PR! GATEWAY IS AS READY AS IT WILL EVER BE IN TERMS OF PRESENTATION
*/

[ExcludeFromCodeCoverage]
internal sealed class Startup
{
    public Startup(IConfiguration configuration)
    {
        this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public IConfiguration Configuration { get; }

    public void Configure(WebApplication application, IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(application);
        ArgumentNullException.ThrowIfNull(environment);

        var options = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
        };

        // Clear the default restrictions so the docker internal bridge network is trusted.
        options.KnownIPNetworks.Clear();
        options.KnownProxies.Clear();

        // Forward headers to maintain origin of client.
        application.UseForwardedHeaders(options);

        if (!environment.IsDevelopment())
        {
            application.UseHsts();
            application.UseRateLimiter();
        }

        application.UseWebSockets(new WebSocketOptions
        {
            KeepAliveInterval = TimeSpan.FromSeconds(this.Configuration.GetValue("WebSocketOptions:KeepAliveInterval", 30.0)),
        });

        application.UseMiddleware<WebSocketMiddleware>();

        application.UseRouting();
        application.UseEndpoints(x => x.MapControllers());
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddRateLimiter();

        services.AddScoped<WebSocketRpcSession>();
        services.AddScoped<JsonRpcRegistrar>();

        services.AddScoped<RpcTargetBase, HealthRpcTarget>();
        services.AddScoped<RpcTargetBase, UserRpcTarget>();
    }
}