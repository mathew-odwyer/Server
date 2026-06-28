namespace Winterhaven.Common.Events;

/// <summary>
/// Defines an interface that represents a request type that expects a single reply of type <typeparamref name="TReply"/> from a responder.
/// </summary>
/// <typeparam name="TReply">
/// The reply type expected in response to this request.
/// </typeparam>
public interface IRequest<TReply>
    where TReply : notnull
{
    /// <summary>
    /// Gets the subject that a request of this type should be sent to.
    /// </summary>
    /// <param name="options">
    /// Settings controlling how the request route is resolved.
    /// </param>
    /// <returns>
    /// The subject to publish the request on.
    /// </returns>
    public static abstract string GetRequestRoute(RequestOptions options);
}
