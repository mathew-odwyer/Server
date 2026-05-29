using System.ComponentModel.DataAnnotations;

namespace Winterhaven.Gateway.Infrastructure.Options.Client;

internal sealed class ClientOptions
{
    [Required]
    public required string ApiKey { get; init; }

    [Required]
    public required string BaseUrl { get; init; }
}
