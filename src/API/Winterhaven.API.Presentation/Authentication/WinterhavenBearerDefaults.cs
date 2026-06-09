using System.Diagnostics.CodeAnalysis;

namespace Winterhaven.API.Presentation.Authentication;

[ExcludeFromCodeCoverage]
internal static class WinterhavenBearerDefaults
{
    public const string Name = "WinterhavenAuthentication";

    public const string ServerAuthenticationScheme = "ApiKey";
}
