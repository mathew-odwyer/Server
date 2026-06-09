using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace Winterhaven.API.Presentation.Options.Security;

/// <summary>
///   Provides options used to configuration the server-to-API settings.
/// </summary>
[ExcludeFromCodeCoverage]
internal sealed class ApiOptions
{
    /// <summary>
    ///   Gets the API key used to access endpoints for the server.
    /// </summary>
    /// <value>
    ///   The API key used to access endpoints for the server.
    /// </value>
    [Required]
    public required string Key { get; init; }
}
