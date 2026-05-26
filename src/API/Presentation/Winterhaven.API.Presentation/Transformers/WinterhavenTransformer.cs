using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;

namespace Winterhaven.API.Presentation.Transformers;

[ExcludeFromCodeCoverage]
internal sealed class WinterhavenTransformer : IOpenApiDocumentTransformer
{
    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(document);

        document.Info.Title = "Winterhaven API";
        document.Info.Version = "v0.3.0";

        return Task.CompletedTask;
    }
}