namespace Winterhaven.Brokering.NATS.Exceptions;

/// <summary>
///   Represents an exception that is thrown when an event type has not been mapped.
/// </summary>
/// <seealso cref="Exception"/>
public sealed class UnmappedEventException : Exception
{
    /// <summary>
    ///   Initializes a new instance of the <see cref="UnmappedEventException"/> class.
    /// </summary>
    public UnmappedEventException()
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="UnmappedEventException"/> class.
    /// </summary>
    /// <param name="eventType">
    ///   The type of the event that is unmapped.
    /// </param>
    /// <exception cref="ArgumentNullException">
    ///   The specified <paramref name="eventType"/> parameter cannot be <c>null</c>.
    /// </exception>
    public UnmappedEventException(Type eventType)
        : base($"Event type '{(eventType ?? throw new ArgumentNullException(nameof(eventType))).FullName}' is missing an [EventName] attribute.")
    {
        this.EventType = eventType;
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="UnmappedEventException"/> class.
    /// </summary>
    /// <param name="message">
    ///   The message that describes the error.
    /// </param>
    public UnmappedEventException(string message)
        : base(message)
    {
    }

    /// <summary>
    ///   Initializes a new instance of the <see cref="UnmappedEventException"/> class.
    /// </summary>
    /// <param name="message">
    ///   The error message that explains the reason for the exception.
    /// </param>
    /// <param name="innerException">
    ///   The exception that is the cause of the current exception, or a null reference ( <see langword="Nothing"/> in Visual Basic) if no inner exception is specified.
    /// </param>
    public UnmappedEventException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    /// <summary>
    ///   Gets the type of the event that is unmapped.
    /// </summary>
    /// <value>
    ///   The type of the event that is unmapped.
    /// </value>
    public Type? EventType { get; }
}