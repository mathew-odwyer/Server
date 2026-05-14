namespace Winterhaven.Gateway.Infrastructure.Clients.Users;

using System;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Common.DTOs.Users;
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
            ?? throw new InvalidOperationException("Failed to deserialize login response.");
    }

    public async Task RegisterUserAsync(RegisterUserRequestDto dto, CancellationToken cancellationToken)
    {
        await this.client.PostAsJsonAsync("Register", dto, cancellationToken).ConfigureAwait(false);
    }
}
