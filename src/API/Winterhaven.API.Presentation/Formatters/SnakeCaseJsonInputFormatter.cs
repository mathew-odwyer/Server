using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.Net.Http.Headers;

namespace Winterhaven.API.Presentation.Formatters;

internal sealed class SnakeCaseJsonInputFormatter : TextInputFormatter
{
    private static readonly JsonSerializerOptions SnakeCaseOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    public SnakeCaseJsonInputFormatter()
    {
        SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/json"));
        SupportedEncodings.Add(Encoding.UTF8);
        SupportedEncodings.Add(Encoding.Unicode);
    }

    public override bool CanRead(InputFormatterContext context)
    {
        var request = context.HttpContext.Request;
        string? contentCase = request.Headers["Content-Case"].FirstOrDefault();

        // Only take over if the client explicitly signals snake_case.
        return contentCase is not null and "snake_case";
    }

    public override async Task<InputFormatterResult> ReadRequestBodyAsync(InputFormatterContext context, Encoding encoding)
    {
        var request = context.HttpContext.Request;

        using var reader = new StreamReader(request.Body, encoding);
        string body = await reader.ReadToEndAsync().ConfigureAwait(false);

        try
        {
            object? result = JsonSerializer.Deserialize(body, context.ModelType, SnakeCaseOptions);
            return await InputFormatterResult.SuccessAsync(result).ConfigureAwait(false);
        }
        catch (JsonException ex)
        {
            context.ModelState.AddModelError(context.ModelName, $"Failed to deserialise snake_case body: {ex.Message}");
            return await InputFormatterResult.FailureAsync().ConfigureAwait(false);
        }
    }
}
