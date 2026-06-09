namespace Winterhaven.Common.Events;

/// <summary>
/// </summary>
public interface IEvent
{
    /// <summary>
    /// </summary>
    /// <param name="options"></param>
    public static abstract string GetPublishEventRoute(PublishOptions options);

    /// <summary>
    /// </summary>
    /// <param name="options"></param>
    public static abstract string GetSubscribeEventRoute(SubscribeOptions options);
}
