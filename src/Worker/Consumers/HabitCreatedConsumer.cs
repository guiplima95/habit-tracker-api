using Azure.Messaging.ServiceBus;
using HabitTracker.API.Worker.Messages;
using System.Text.Json;

namespace HabitTracker.Worker.Consumers;

public sealed class HabitCreatedConsumer(
    ILogger<HabitCreatedConsumer> logger)
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public async Task HandleAsync(ProcessMessageEventArgs args)
    {
        var body = args.Message.Body.ToString();

        var message = JsonSerializer.Deserialize<HabitCreatedMessage>(
            body,
            JsonOptions);

        if (message is null)
        {
            logger.LogWarning(
                "Could not deserialize HabitCreatedMessage. Body: {Body}",
                body);

            // Dead letter
            await args.DeadLetterMessageAsync(
                args.Message,
                deadLetterReason: "DeserializationFailed",
                deadLetterErrorDescription: "Could not deserialize to HabitCreatedMessage");

            return;
        }

        logger.LogInformation(
            "Processing HabitCreatedMessage — HabitId: {HabitId}, Name: {HabitName}",
            message.Id,
            message.Name);

        // Business logic

        // Complete — remove a mensagem da subscription
        await args.CompleteMessageAsync(args.Message);

        logger.LogInformation(
            "HabitCreatedMessage processed — HabitId: {HabitId}",
            message.Id);
    }

    // Delegate - ProcessErrorEventArgs
    public Task HandleErrorAsync(ProcessErrorEventArgs args)
    {
        logger.LogError(
            args.Exception,
            "Error processing message. Source: {Source}, Reason: {Reason}",
            args.ErrorSource,
            args.Exception.Message);

        return Task.CompletedTask;
    }
}