namespace Winterhaven.Brokering.Attributes;

using System;

/// <summary>
///   Represents an attribute that is used to provide an event type with a name.
/// </summary>
/// <seealso cref="Attribute"/>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public sealed class EventNameAttribute : Attribute
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="EventNameAttribute"/> class.
    /// </summary>
    /// <param name="name">
    ///   The name of the event.
    /// </param>
    public EventNameAttribute(string name)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(name);
        this.Name = name;
    }

    /// <summary>
    ///   Gets the name of the event.
    /// </summary>
    /// <value>
    ///   The name of the event.
    /// </value>
    public string Name { get; }
}