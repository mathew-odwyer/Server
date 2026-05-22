namespace Winterhaven.Brokering.NATS.Serialization;

using global::NATS.Client.Core;
using global::NATS.Client.Serializers.Json;
using System.Text.Json;

internal sealed class NatsSnakeCaseSerializerRegistry : INatsSerializerRegistry
{
    public static readonly NatsSnakeCaseSerializerRegistry Default = new();

    private static readonly JsonSerializerOptions SnakeCaseOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.SnakeCaseLower,
    };

    public INatsDeserialize<T> GetDeserializer<T>()
    {
        return new NatsJsonSerializer<T>(SnakeCaseOptions);
    }

    public INatsSerialize<T> GetSerializer<T>()
    {
        return new NatsJsonSerializer<T>(SnakeCaseOptions);
    }
}