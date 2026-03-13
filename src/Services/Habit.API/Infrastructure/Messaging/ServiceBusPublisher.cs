using Azure.Messaging.ServiceBus;
using HabitTracker.API.Application.Common.Interfaces;
using System.Diagnostics;
using System.Text.Json;

namespace HabitTracker.API.Infrastructure.Messaging;

public sealed class ServiceBusPublisher(
    ServiceBusClient client,
    ILogger<ServiceBusPublisher> logger) : IServiceBusPublisher
{
    private const string TopicName = "habit-events";

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    public async Task PublishAsync<T>(
        T message,
        CancellationToken cancellationToken = default)
        where T : class
    {
        try
        {
            await using var sender = client.CreateSender(TopicName);

            var payload = JsonSerializer.Serialize(message, JsonOptions);
            var busMessage = BuildMessage<T>(payload);

            await sender.SendMessageAsync(busMessage, cancellationToken);

            logger.LogInformation(
                "Event {EventType} published to topic '{Topic}' with MessageId '{MessageId}'",
                typeof(T).Name,
                TopicName,
                busMessage.MessageId);
        }
        catch (ServiceBusException ex) when (ex.IsTransient)
        {
            logger.LogWarning(
                ex,
                "Transient error publishing {EventType} to '{Topic}'. Retrying...",
                typeof(T).Name,
                TopicName);

            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Failed to publish {EventType} to topic '{Topic}'",
                typeof(T).Name,
                TopicName);

            throw;
        }
    }

    private static ServiceBusMessage BuildMessage<T>(string payload)
        where T : class
        => new(payload)
        {
            MessageId = Guid.NewGuid().ToString(),
            ContentType = "application/json",
            Subject = typeof(T).Name,

            // Links message to the original HTTP trace
            CorrelationId = Activity.Current?.TraceId.ToString()
                            ?? Guid.NewGuid().ToString(),

            // Subscribers filter by EventType property
            ApplicationProperties =
            {
                ["EventType"] = typeof(T).Name,
            }
        };
}