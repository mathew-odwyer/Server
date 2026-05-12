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
using Winterhaven.Gateway.Presentation.Targets;
using Winterhaven.Gateway.Presentation.Targets.Health;
using Winterhaven.Gateway.Presentation.Targets.Services;
using Winterhaven.Gateway.Presentation.Targets.Users;

/*
    TODO: Fix issue where the server doesn't shut down once a client has connected.
        TODO: Figure out whether I should ConfigureAwait(true) when a connection needs to be disconnected or something?
    TOOD: Register things via DI container.
    TODO: General cleanup, documentation, PUSH AND MERGE changes as the "Gateway Server" is ready.
    TODO: Then, WRITE ISSUE FOR Gateway Registration Service (Requires research on HttpClient/Factory, IUserAccountClient, using MediatR, FluentValidation, setting up infra, etc)
        - This also includes ValidationException, AuthorizationException, etc
        - As well as mapping the HTTP response that are not in the 200 range to their correct exceptions, which will bubble up to the presentation layer and be converted into JSON-RPC 2.0 Error Details
*/

[ExcludeFromCodeCoverage]
internal sealed class Startup
{
    public Startup(IConfiguration configuration)
    {
        this.Configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public IConfiguration Configuration { get; }

    public static void Configure(WebApplication application, IWebHostEnvironment environment)
    {
        ArgumentNullException.ThrowIfNull(application);
        ArgumentNullException.ThrowIfNull(environment);

        // Forward headers to maintain origin of client.
        application.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
        });

        if (!environment.IsDevelopment())
        {
            application.UseHsts();
            application.UseRateLimiter();
        }

        application.UseWebSockets(new WebSocketOptions
        {
            KeepAliveInterval = TimeSpan.FromSeconds(30),
        });

        application.UseMiddleware<WebSocketMiddleware>();

        application.UseRouting();
        application.UseEndpoints(x => x.MapControllers());
    }

    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddRateLimiter();

        services.AddScoped<JsonRpcRegistrar>();
        services.AddScoped<RpcTargetBase, HealthRpcTarget>();
        services.AddScoped<RpcTargetBase, UserRpcTarget>();
    }
}