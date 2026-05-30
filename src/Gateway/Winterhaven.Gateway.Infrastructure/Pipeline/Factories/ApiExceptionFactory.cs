using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Winterhaven.Gateway.Core.Domain.Exceptions;

namespace Winterhaven.Gateway.Infrastructure.Pipeline.Factories;

internal static class ApiExceptionFactory
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public static async Task<Exception?> CreateAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return null;

        var problem = await TryDeserializeProblemAsync(response).ConfigureAwait(false);
        string detail = problem?.Detail ?? "An unexpected error occurred. Please try again later.";

        return response.StatusCode switch
        {
            HttpStatusCode.BadRequest => BuildValidationException(problem!),
            HttpStatusCode.Unauthorized => new AuthorizationException(detail),
            HttpStatusCode.InternalServerError => new InvalidOperationException(detail),
            _ => new InvalidOperationException($"Unexpected status {(int)response.StatusCode}: {detail}")
        };
    }

    private static ValidationException BuildValidationException(ValidationProblemDetails problem)
    {
        ArgumentNullException.ThrowIfNull(problem);
        return problem.Errors is { Count: > 0 }
            ? new ValidationException(problem.Errors as IReadOnlyDictionary<string, string[]>)
            : new ValidationException(problem.Detail);
    }

    private static async Task<ValidationProblemDetails?> TryDeserializeProblemAsync(HttpResponseMessage response)
    {
        try
        {
            string json = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
            return JsonSerializer.Deserialize<ValidationProblemDetails>(json, Options);
        }
        catch (JsonException)
        {
            return null;
        }
    }
}
