using System;
using System.IdentityModel.Tokens.Jwt;
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
    protected WireMockServer Api { get; private set; }

    protected IHost Host { get; private set; }

    protected MockUserSessionManager UserSessionManager { get; } = new();

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

    protected async Task SetUpTestHost()
    {
        Api = WireMockServer.Start();

        var builder = new HostBuilder();

        builder.ConfigureWebHost(x =>
        {
            x.UseStartup<Startup>();
            x.UseTestServer();
            x.ConfigureAppConfiguration(x => x.AddJsonFile("appsettings.Tests.json", optional: false));
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

    protected async Task TearDownTestHost()
    {
        await Host.StopAsync().ConfigureAwait(false);

        Host.Dispose();

        Api.Stop();
        Api.Dispose();
    }
}
