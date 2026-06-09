using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Winterhaven.Gateway.Infrastructure.Options.Client;

[ExcludeFromCodeCoverage]
internal sealed class ClientOptions
{
    [Required]
    public required string ApiKey { get; set; }

    [Required]
    public required string BaseUrl { get; set; }
}
