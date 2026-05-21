namespace Winterhaven.Brokering.NATS.Resolving;

internal interface INatsSubjectResolver
{
    string ResolveSubject<TEvent>(TEvent e)
        where TEvent : class;
}