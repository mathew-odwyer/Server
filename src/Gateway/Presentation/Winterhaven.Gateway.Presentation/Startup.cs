namespace Winterhaven.Gateway.Presentation;

using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Diagnostics.CodeAnalysis;
using Winterhaven.Gateway.Infrastructure.Extensions;
using Winterhaven.Gateway.Presentation.Middleware;
using Winterhaven.Gateway.Presentation.Services;
using Winterhaven.Gateway.Presentation.Targets;
using Winterhaven.Gateway.Presentation.Targets.Health;
using Winterhaven.Gateway.Presentation.Targets.Users;
using Winterhaven.Gateway.Presentation.Validation;
using Winterhaven.Gateway.Presentation.Validation.Users;
using IValidatorFactory = Validation.IValidatorFactory;

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

        // Read X-Forwarded-For and X-Forwarded-Proto headers set by the reverse proxy so the app sees the real client IP and protocol instead of the proxy's.
        var options = new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
        };

        // Clear the default known network/ proxy restrictions since our reverse proxy runs on Docker's internal bridge network, which isn't trusted by default.
        options.KnownNetworks.Clear();
        options.KnownProxies.Clear();

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

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers();
        services.AddRateLimiter();

        services.AddScoped<WebSocketRpcSession>();
        services.AddScoped<JsonRpcRegistrar>();

        services.AddScoped<RpcTargetBase, HealthRpcTarget>();
        services.AddScoped<RpcTargetBase, UserRpcTarget>();

        services.AddGatewayInfrastructureServices(this.Configuration);

        services.AddValidatorsFromAssembly(typeof(UserLoginRpcParametersValidator).Assembly);
        services.AddScoped<IValidatorFactory, ValidatorFactory>();

        services.AddHttpContextAccessor();
    }
}