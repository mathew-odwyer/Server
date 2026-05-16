namespace Winterhaven.Gateway.Infrastructure.Clients.Users;

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.Common.DTOs.Users;
using Winterhaven.Gateway.Core.Application.Clients.Users;

internal sealed class UserAccountClient : IUserAccountClient
{
    private static readonly Uri LoginUri = new("Login", UriKind.Relative);

    private static readonly Uri LogoutUri = new("Logout", UriKind.Relative);

    private static readonly Uri RegisterUri = new("Register", UriKind.Relative);

    private static readonly Uri RefreshTokenUri = new("RefreshToken", UriKind.Relative);

    private readonly HttpClient client;

    public UserAccountClient(HttpClient client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<LoginUserResponseDto> LoginUserAsync(LoginUserRequestDto dto, CancellationToken cancellationToken)
    {
        using var response = await this.client.PostAsJsonAsync(LoginUri, dto, cancellationToken).ConfigureAwait(false);

        return await response.Content.ReadFromJsonAsync<LoginUserResponseDto>(cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Failed to de-serialize login response.");
    }

    public async Task LogoutUserAsync(CancellationToken cancellationToken)
    {
        (await this.client.PostAsync(LogoutUri, null, cancellationToken).ConfigureAwait(false)).Dispose();
    }

    public async Task<RefreshTokenResponseDto> RefreshTokenAsync(RefreshTokenRequestDto dto, CancellationToken cancellationToken)
    {
        using var response = await this.client.PostAsJsonAsync(RefreshTokenUri, dto, cancellationToken).ConfigureAwait(false);

        return await response.Content.ReadFromJsonAsync<RefreshTokenResponseDto>(cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Failed to de-serialize refresh token response.");
    }

    public async Task RegisterUserAsync(RegisterUserRequestDto dto, CancellationToken cancellationToken)
    {
        (await this.client.PostAsJsonAsync(RegisterUri, dto, cancellationToken).ConfigureAwait(false)).Dispose();
    }
}