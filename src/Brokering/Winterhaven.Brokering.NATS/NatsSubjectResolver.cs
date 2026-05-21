namespace Winterhaven.Brokering.NATS;

using System.Reflection;
using Winterhaven.Brokering.Attributes;
using Winterhaven.Brokering.NATS.Exceptions;

internal static class NatsSubjectResolver
{
    public static string ResolveSubject<TEvent>(TEvent e)
        where TEvent : class
    {
        var type = e.GetType();
        var attribute = type.GetCustomAttribute<EventNameAttribute>()
            ?? throw new UnmappedEventException(type);

        return attribute.Name;
    }
}