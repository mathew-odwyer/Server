using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Winterhaven.API.Presentation.Authentication;

namespace Winterhaven.API.Presentation.Transformers.Security;

[ExcludeFromCodeCoverage]
internal sealed class ApiKeySecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();

        document.Components.SecuritySchemes ??= new Dictionary<string, OpenApiSecurityScheme>();

        document.Components.SecuritySchemes[WinterhavenBearerDefaults.ServerAuthenticationScheme] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            Name = "X-API-KEY",
            In = ParameterLocation.Header,
            Description = "API Key for server endpoints."
        };

        var scheme = new OpenApiSecurityScheme
        {
            Reference = new OpenApiReference
            {
                Type = ReferenceType.SecurityScheme,
                Id = WinterhavenBearerDefaults.ServerAuthenticationScheme
            },
        };

        var requirement = new OpenApiSecurityRequirement
        {
            [scheme] = [],
        };

        foreach (var operation in document.Paths.Values.SelectMany(path => (IEnumerable<OpenApiOperation>?)path.Operations?.Values ?? []))
        {
            operation.Security ??= [];
            operation.Security.Add(requirement);
        }

        return Task.CompletedTask;
    }
}