namespace Winterhaven.API.Presentation.Extensions;

using Microsoft.AspNetCore.Builder;
using System.Diagnostics.CodeAnalysis;
using Winterhaven.API.Presentation.Middleware.Users;

[ExcludeFromCodeCoverage]
internal static class ApplicationBuilderExtensions
{
    internal static IApplicationBuilder UseUserSessions(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<UserSessionValidationMiddleware>();
        return builder;
    }
}