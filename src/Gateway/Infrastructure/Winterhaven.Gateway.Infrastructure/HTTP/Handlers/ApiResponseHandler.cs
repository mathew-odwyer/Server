namespace Winterhaven.Gateway.Infrastructure.HTTP.Handlers;

using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Winterhaven.Gateway.Core.Domain.Exceptions;

internal sealed class ApiResponseHandler : DelegatingHandler
{
    private static readonly JsonSerializerOptions options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

        if (response.IsSuccessStatusCode)
        {
            return response;
        }

        string content = await response.Content.ReadAsStringAsync(cancellationToken).ConfigureAwait(false);
        var problem = TryDeserialize<ProblemDetails>(content);
        string detail = problem?.Detail ?? "An unexpected error occurred. Please try again later.";

        throw response.StatusCode switch
        {
            HttpStatusCode.BadRequest => BuildValidationException(content),
            HttpStatusCode.Unauthorized => new AuthorizationException(detail),
            HttpStatusCode.InternalServerError => new InvalidOperationException(detail),
            _ => new InvalidOperationException($"Unexpected status {(int)response.StatusCode}: {detail}")
        };
    }

    private static ValidationException BuildValidationException(string content)
    {
        var problem = TryDeserialize<ValidationProblemDetails>(content);

        return problem?.Errors is { Count: > 0 }
            ? new ValidationException(problem.Errors as IReadOnlyDictionary<string, string[]>)
            : new ValidationException(problem?.Detail);
    }

    private static T? TryDeserialize<T>(string content)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(content, options);
        }
        catch
        (JsonException)
        {
            return default;
        }
    }
}