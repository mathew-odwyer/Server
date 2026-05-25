namespace Winterhaven.API.Presentation.Transformers;

using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;

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