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
    private readonly HttpClient client;

    public UserAccountClient(HttpClient client)
    {
        this.client = client ?? throw new ArgumentNullException(nameof(client));
    }

    public async Task<LoginUserResponseDto> LoginUserAsync(LoginUserRequestDto dto, CancellationToken cancellationToken)
    {
        var response = await this.client.PostAsJsonAsync("Login", dto, cancellationToken).ConfigureAwait(false);

        return await response.Content.ReadFromJsonAsync<LoginUserResponseDto>(cancellationToken).ConfigureAwait(false)
            ?? throw new InvalidOperationException("Failed to de-serialize login response.");
    }

    public async Task LogoutUserAsync(CancellationToken cancellationToken)
    {
        // FIXME: Resolve issue where when we logout we have o remove the X-API-KEY so that the JWT
        // has authority or at least be able to handle using both the API Key and JWT for authentication.

        await this.client.PostAsync("Logout", null, cancellationToken).ConfigureAwait(false);
    }

    public async Task RegisterUserAsync(RegisterUserRequestDto dto, CancellationToken cancellationToken)
    {
        await this.client.PostAsJsonAsync("Register", dto, cancellationToken).ConfigureAwait(false);
    }
}