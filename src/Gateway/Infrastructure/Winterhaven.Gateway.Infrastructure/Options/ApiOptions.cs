namespace Winterhaven.Gateway.Infrastructure.Options;

using System.ComponentModel.DataAnnotations;

internal sealed class ApiOptions
{
    [Required]
    public required string BaseUrl { get; init; }

    [Required]
    public required string ApiKey { get; init; }
}
