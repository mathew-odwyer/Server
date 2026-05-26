using System.Diagnostics.CodeAnalysis;
using Microsoft.AspNetCore.Builder;
using Winterhaven.API.Presentation.Middleware.Users;

namespace Winterhaven.API.Presentation.Extensions;

[ExcludeFromCodeCoverage]
internal static class ApplicationBuilderExtensions
{
    internal static IApplicationBuilder UseUserSessions(this IApplicationBuilder builder)
    {
        builder.UseMiddleware<UserSessionValidationMiddleware>();
        return builder;
    }
}