namespace Winterhaven.Common.Events;

/// <summary>
/// </summary>
public interface IEvent
{
    /// <summary>
    /// </summary>
    public static abstract string GetPublishEventRoute(PublishOptions options);

    /// <summary>
    /// </summary>
    public static abstract string GetSubscribeEventRoute(SubscribeOptions options);
}
