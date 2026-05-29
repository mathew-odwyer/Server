using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Refit;
using Winterhaven.Gateway.Core.Domain.Exceptions;

namespace Winterhaven.Gateway.Infrastructure.Pipeline.Handlers;

internal sealed class ApiExceptionHandler : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        try
        {
            return await base.SendAsync(request, cancellationToken).ConfigureAwait(false);
        }
        catch (ApiException ex)
        {
            var problem = await ex.GetContentAsAsync<ValidationProblemDetails>().ConfigureAwait(false);
            string detail = problem?.Detail ?? "An unexpected error occurred. Please try again later.";

            throw ex.StatusCode switch
            {
                HttpStatusCode.BadRequest => BuildValidationException(problem!),
                HttpStatusCode.Unauthorized => new AuthorizationException(detail),
                HttpStatusCode.InternalServerError => new InvalidOperationException(detail),
                _ => new InvalidOperationException($"Unexpected status {(int)ex.StatusCode}: {detail}")
            };
        }
    }

    private static ValidationException BuildValidationException(ValidationProblemDetails problem)
    {
        ArgumentNullException.ThrowIfNull(problem);
        return problem.Errors is { Count: > 0 }
            ? new ValidationException(problem.Errors as IReadOnlyDictionary<string, string[]>)
            : new ValidationException(problem.Detail);
    }
}
