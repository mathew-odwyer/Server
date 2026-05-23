namespace Winterhaven.Brokering.NATS.Resolving;

using System.Reflection;
using Winterhaven.Brokering.Attributes;
using Winterhaven.Brokering.NATS.Exceptions;

internal sealed class NatsSubjectResolver : INatsSubjectResolver
{
    public string ResolveSubject<TEvent>()
        where TEvent : class
    {
        var type = typeof(TEvent);
        var attribute = type.GetCustomAttribute<EventNameAttribute>()
            ?? throw new UnmappedEventException(type);

        return attribute.Name;
    }
}