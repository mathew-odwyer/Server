// <copyright file="ApiOptions.cs" company="Software Antics">
//   Copyright (c) Software Antics. All rights reserved.
// </copyright>

namespace Winterhaven.Core.Application.Options.Security;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Provides options used to configuration the server-to-API settings.
/// </summary>
public sealed class ApiOptions
{
    /// <summary>
    /// Gets the API key used to access endpoints for the server.
    /// </summary>
    /// <value>
    /// The API key used to access endpoints for the server.
    /// </value>
    [Required]
    public required string Key { get; init; }
}
