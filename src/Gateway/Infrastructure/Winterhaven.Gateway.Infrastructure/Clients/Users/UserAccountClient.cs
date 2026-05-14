namespace Winterhaven.Gateway.Infrastructure.Clients.Users;

using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Common.DTOs.Users;
using Winterhaven.Gateway.Core.Application.Clients.Users;

internal sealed class UserAccountClient : IUserAccountClient
{
    private readonly HttpClient client;

    public UserAccountClient(HttpClient client)
    {
        this.client = client ?? throw new System.ArgumentNullException(nameof(client));
    }

    public async Task RegisterUserAsync(RegisterUserRequestDto dto, CancellationToken cancellationToken)
    {
        await this.client.PostAsJsonAsync("Register", dto, cancellationToken).ConfigureAwait(false);
    }
}
