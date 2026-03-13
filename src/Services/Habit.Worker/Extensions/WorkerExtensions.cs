using Azure.Messaging.ServiceBus;
using HabitTracker.Worker.Consumers;

namespace HabitTracker.Worker.Extensions;

public static class WorkerExtensions
{
    public static IServiceCollection AddWorkerServices(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Azure Service Bus — singleton client
        services.AddSingleton(_ => new ServiceBusClient(
            configuration.GetConnectionString("ServiceBus")!,
            new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets,
                RetryOptions = new ServiceBusRetryOptions
                {
                    Mode = ServiceBusRetryMode.Exponential,
                    MaxRetries = 3,
                    Delay = TimeSpan.FromSeconds(1),
                    MaxDelay = TimeSpan.FromSeconds(30)
                }
            }));

        // Consumers — scoped per message
        services.AddSingleton<HabitCreatedConsumer>();

        return services;
    }
}