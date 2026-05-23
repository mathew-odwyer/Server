namespace Winterhaven.Gateway.Presentation.Exceptions;

using System;
using System.Diagnostics.CodeAnalysis;

[ExcludeFromCodeCoverage]
internal sealed class UnmappedNotificationException : Exception
{
    public UnmappedNotificationException()
    {
    }

    public UnmappedNotificationException(Type notificationType)
        : base($"Notification type '{(notificationType ?? throw new ArgumentNullException(nameof(notificationType))).FullName}' is missing a [NotificationName] attribute.")
    {
        this.NotificationType = notificationType;
    }

    public UnmappedNotificationException(string message)
        : base(message)
    {
    }

    public UnmappedNotificationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    public Type? NotificationType { get; }
}