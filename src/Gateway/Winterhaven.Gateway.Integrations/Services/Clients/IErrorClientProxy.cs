using System.Threading;
using System.Threading.Tasks;
using PolyType;
using StreamJsonRpc;

namespace Winterhaven.Gateway.Integrations.Services.Clients;

[JsonRpcContract]
[GenerateShape(IncludeMethods = MethodShapeFlags.PublicInstance)]
internal partial interface IErrorClientProxy
{
    [JsonRpcMethod("error.chat", UseSingleObjectParameterDeserialization = true)]
    public Task GenerateChatError(CancellationToken cancellationToken = default);

    [JsonRpcMethod("error.unauthorized", UseSingleObjectParameterDeserialization = true)]
    public Task GenerateUnauthorizedError(CancellationToken cancellationToken = default);

    [JsonRpcMethod("error.unhandled", UseSingleObjectParameterDeserialization = true)]
    public Task GenerateUnhandledError(CancellationToken cancellationToken = default);

    [JsonRpcMethod("error.validation", UseSingleObjectParameterDeserialization = true)]
    public Task GenerateValidationError(CancellationToken cancellationToken = default);

    [JsonRpcMethod("error.validation.null", UseSingleObjectParameterDeserialization = true)]
    public Task GenerateValidationErrorWithNullErrors(CancellationToken cancellationToken = default);
}
