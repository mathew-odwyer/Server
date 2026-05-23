namespace Winterhaven.Gateway.Presentation.Services.Dispatching;

internal interface IJsonRpcMethodResolver
{
    string ResolveMethodName<TNotification>(TNotification notification)
        where TNotification : class;
}