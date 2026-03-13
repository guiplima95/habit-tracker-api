using Azure.Messaging.ServiceBus;
using HabitTracker.Worker.Consumers;

namespace HabitTracker.Worker;

public sealed class Worker(
    ServiceBusClient client,
    HabitCreatedConsumer habitCreatedConsumer,
    ILogger<Worker> logger) : BackgroundService
{
    // Subscription names
    private const string TopicName = "habit-events";

    private const string HabitCreatedSub = "HabitCreated";

    private ServiceBusProcessor? _habitCreatedProcessor;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        logger.LogInformation("Worker starting...");

        // ✅ Create processors — one per subscription
        _habitCreatedProcessor = client.CreateProcessor(
            TopicName,
            HabitCreatedSub,
            new ServiceBusProcessorOptions
            {
                MaxConcurrentCalls = 5,   // parallel processing
                AutoCompleteMessages = false // manual complete — we control ack
            });

        // Wire up handlers
        _habitCreatedProcessor.ProcessMessageAsync += habitCreatedConsumer.HandleAsync;
        _habitCreatedProcessor.ProcessErrorAsync += habitCreatedConsumer.HandleErrorAsync;

        // Start all processors
        await _habitCreatedProcessor.StartProcessingAsync(stoppingToken);

        logger.LogInformation("Worker started — listening on {Topic}", TopicName);

        // Keep alive until cancellation
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        logger.LogInformation("Worker stopping...");

        // Graceful shutdown — stop processors before disposing
        if (_habitCreatedProcessor is not null)
        {
            await _habitCreatedProcessor.StopProcessingAsync(cancellationToken);
        }

        await base.StopAsync(cancellationToken);

        logger.LogInformation("Worker stopped gracefully.");
    }

    public async ValueTask DisposeAsync()
    {
        if (_habitCreatedProcessor is not null)
        {
            await _habitCreatedProcessor.DisposeAsync();
            _habitCreatedProcessor = null;
        }
    }
}