namespace Winterhaven.Brokering.NATS.Resolving;

using System.Reflection;
using Winterhaven.Brokering.Attributes;
using Winterhaven.Brokering.NATS.Exceptions;

internal sealed class NatsSubjectResolver : INatsSubjectResolver
{
    public string ResolveSubject<TEvent>(TEvent e)
        where TEvent : class
    {
        var type = e.GetType();
        var attribute = type.GetCustomAttribute<EventNameAttribute>()
            ?? throw new UnmappedEventException(type);

        return attribute.Name;
    }
}