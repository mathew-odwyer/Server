namespace Winterhaven.Gateway.Infrastructure.Options;

using System.ComponentModel.DataAnnotations;

internal sealed class ClientOptions
{
    [Required]
    public required string ApiKey { get; init; }

    [Required]
    public required string BaseUrl { get; init; }
}