namespace Winterhaven.Brokering.NATS.Resolving;

internal interface INatsSubjectResolver
{
    string ResolveSubject<TEvent>()
        where TEvent : class;
}