using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Winterhaven.Gateway.Core.Domain.Exceptions;

namespace Winterhaven.Gateway.Infrastructure.Pipeline.Factories;

internal class ApiExceptionFactory
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNameCaseInsensitive = true
    };

    private readonly ILogger<ApiExceptionFactory> logger;

    public ApiExceptionFactory(ILogger<ApiExceptionFactory> logger) =>
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<Exception?> CreateAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode)
        {
            return null;
        }

        var problem = await TryDeserializeProblemAsync(response).ConfigureAwait(false);
        string detail = problem?.Detail ?? "An unexpected error occurred. Please try again later.";

        return response.StatusCode switch
        {
            HttpStatusCode.BadRequest => BuildValidationException(problem, detail),
            HttpStatusCode.Unauthorized => new AuthorizationException(detail),
            HttpStatusCode.InternalServerError => new InvalidOperationException(detail),
            _ => new InvalidOperationException($"Unexpected status {(int)response.StatusCode}: {detail}")
        };
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

    private ValidationException BuildValidationException(ValidationProblemDetails? problem, string detail)
    {
        ArgumentNullException.ThrowIfNull(detail);

        if (problem == null)
        {
            logger.LogWarning("Received a 400 response but the problem body could not be deserialised. Falling back to a generic validation error.");
            return new ValidationException(detail);
        }

        return problem.Errors is { Count: > 0 }
            ? new ValidationException(problem.Errors as IReadOnlyDictionary<string, string[]>)
            : new ValidationException(problem.Detail);
    }
}
