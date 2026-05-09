namespace Winterhaven.API.Presentation.Transformers.Security;

using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.API.Presentation.Authentication;

[ExcludeFromCodeCoverage]
internal sealed class ApiKeySecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        document.Components ??= new OpenApiComponents();

        document.AddComponent(WinterhavenBearerDefaults.ServerAuthenticationScheme, new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            Name = "X-API-KEY",
            In = ParameterLocation.Header,
            Description = "API Key for server endpoints."
        });

        var requirement = new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(WinterhavenBearerDefaults.ServerAuthenticationScheme, document)] = []
        };

        foreach (var operation in document.Paths.Values.SelectMany(path => (IEnumerable<OpenApiOperation>?)path.Operations?.Values ?? []))
        {
            operation.Security ??= [];
            operation.Security.Add(requirement);
        }

        return Task.CompletedTask;
    }
}