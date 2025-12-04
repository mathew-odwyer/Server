namespace Winterhaven.Core.Application.Options.Security;

using System.ComponentModel.DataAnnotations;

public sealed class ApiOptions
{
    [Required]
    public string Key { get; init; }
}
