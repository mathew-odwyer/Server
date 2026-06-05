using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Security.Claims;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using NUnit.Framework;
using Testcontainers.Nats;
using Winterhaven.Brokering;
using Winterhaven.Gateway.Core.Application.Services.Users;
using Winterhaven.Gateway.Infrastructure.Options.Client;
using Winterhaven.Gateway.Infrastructure.Services.Users;
using Winterhaven.Gateway.Integrations.Services.Builders;
using Winterhaven.Gateway.Integrations.Services.Targets;
using Winterhaven.Gateway.Integrations.Services.Users;
using Winterhaven.Gateway.Presentation;
using Winterhaven.Gateway.Presentation.Targets;
using WireMock.Server;

namespace Winterhaven.Gateway.Integrations;

internal abstract class TestHostBase
{
    private NatsContainer natsContainer;

    protected WireMockServer Api { get; private set; }

    protected IHost Host { get; private set; }

    protected IMessageBus MessageBus => Host.Services.GetRequiredService<IMessageBus>();

    protected MockUserSessionManager UserSessionManager { get; } = new();

    [OneTimeSetUp]
    public virtual async Task OneTimeSetUp()
    {
        natsContainer = new NatsBuilder("nats:latest")
            .WithPortBinding(9222, true)
            .WithResourceMapping(
                Encoding.UTF8.GetBytes(await File.ReadAllTextAsync("nats-test.conf")),
                "/etc/nats/nats.conf")
            .WithCommand("-c", "/etc/nats/nats.conf")
            .Build();

        await natsContainer.StartAsync();

        Api = WireMockServer.Start();

        var builder = new HostBuilder();

        builder.ConfigureWebHost(x =>
        {
            x.UseStartup<Startup>();
            x.UseTestServer();
            x.ConfigureAppConfiguration(config =>
            {
                config.AddJsonFile("appsettings.Tests.json", optional: false);

                //// Override environment variables via in-memory config, this takes priority.
                //// Prefer this approach over Environment.SetEnvironmentVariable because some
                //// services require variables to be set during DI registration.
                config.AddInMemoryCollection(new Dictionary<string, string>
                {
                    ["NATS_URL"] = $"ws://localhost:{natsContainer.GetMappedPublicPort(9222)}"
                });
            });
            x.ConfigureTestServices(services =>
            {
                services.AddScoped<IRpcTarget, TestErrorRpcTarget>();
                services.AddScoped<IRpcTarget, TestAuthRpcTarget>();

                services.AddSingleton<IUserSessionContext>(UserSessionManager);
                services.AddSingleton<IUserSessionManager>(UserSessionManager);

                services.PostConfigure<ClientOptions>(x => x.BaseUrl = Api.Url);
            });
        });

        Host = builder.Build();
        await Host.StartAsync().ConfigureAwait(false);
    }

    [OneTimeTearDown]
    public virtual async Task OneTimeTearDown()
    {
        await Host.StopAsync().ConfigureAwait(false);

        Host.Dispose();

        Api.Stop();
        Api.Dispose();

        await natsContainer.StopAsync();
        await natsContainer.DisposeAsync();
    }

    protected string CreateAccessToken(Guid identifier, string username)
    {
        var configuration = Host.Services.GetRequiredService<IConfiguration>();

        string secret = configuration["JwtOptions:Secret"] ?? "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890";

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: configuration["JwtOptions:Issuer"] ?? "test-issuer",
            audience: configuration["JwtOptions:Audience"] ?? "test-audience",
            claims: [new Claim("identifier", identifier.ToString()), new Claim("username", username)],
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    protected async Task<WebSocketRpcConnection> CreateConnectionAsync(
        Action<WebSocketRpcConnectionBuilder> configure = null,
        string uri = "ws://localhost/ws",
        CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(uri);

        var builder = new WebSocketRpcConnectionBuilder();
        configure?.Invoke(builder);

        var client = Host.GetTestServer().CreateWebSocketClient();
        var webSocket = await client.ConnectAsync(new Uri(uri), cancellationToken).ConfigureAwait(false);

        return builder.Build(webSocket);
    }
}
