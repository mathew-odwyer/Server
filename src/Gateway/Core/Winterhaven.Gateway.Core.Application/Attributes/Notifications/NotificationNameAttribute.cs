namespace Winterhaven.Brokering.Attributes;

using System;

/// <summary>
///   Represents an attribute that is used to provide a notification type with a name.
/// </summary>
/// <seealso cref="Attribute"/>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class NotificationNameAttribute : Attribute
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="NotificationNameAttribute"/> class.
    /// </summary>
    /// <param name="name">
    ///   The name of the notification.
    /// </param>
    public NotificationNameAttribute(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        this.Name = name;
    }

    /// <summary>
    ///   Gets the name of the notification.
    /// </summary>
    /// <value>
    ///   The name of the notification.
    /// </value>
    public string Name { get; }
}