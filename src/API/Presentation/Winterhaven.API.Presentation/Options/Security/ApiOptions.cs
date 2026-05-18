namespace Winterhaven.API.Presentation.Options.Security;

using System.ComponentModel.DataAnnotations;

/// <summary>
///   Provides options used to configuration the server-to-API settings.
/// </summary>
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