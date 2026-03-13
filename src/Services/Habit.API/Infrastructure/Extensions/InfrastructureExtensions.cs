using Azure.Messaging.ServiceBus;
using HabitTracker.API.Application.Common.Interfaces;
using HabitTracker.API.Domain.Interfaces;
using HabitTracker.API.Infrastructure.Data;
using HabitTracker.API.Infrastructure.Messaging;
using HabitTracker.API.Infrastructure.Persistence;
using HabitTracker.API.Infrastructure.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.API.Infrastructure.Extensions;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddDatabase(configuration)
            .AddRepositories()
            .AddMessaging(configuration)
            .AddSecurity()
            .AddHealthChecks(configuration);

        return services;
    }

    private static IServiceCollection AddDatabase(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // EF Core
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(
                configuration.GetConnectionString("SqlServer"),
                sql =>
                {
                    sql.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                    sql.CommandTimeout(30);
                })
            .UseQueryTrackingBehavior(
                QueryTrackingBehavior.NoTrackingWithIdentityResolution));

        services.AddScoped<IUnitOfWork>(sp =>
            sp.GetRequiredService<AppDbContext>());

        // Dapper — lightweight reads
        services.AddScoped<IDapperContext, DapperContext>();

        return services;
    }

    // Repositories
    private static IServiceCollection AddRepositories(
        this IServiceCollection services)
    {
        services.AddScoped<IHabitRepository, HabitRepository>();
        services.AddScoped<IUserRepository, UserRepository>();

        return services;
    }

    // Azure Service Bus
    private static IServiceCollection AddMessaging(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ServiceBus")!;

        services.AddSingleton(_ => new ServiceBusClient(
            connectionString,
            new ServiceBusClientOptions
            {
                TransportType = ServiceBusTransportType.AmqpWebSockets, // firewall friendly
                RetryOptions = new ServiceBusRetryOptions
                {
                    Mode = ServiceBusRetryMode.Exponential,
                    MaxRetries = 3,
                    Delay = TimeSpan.FromSeconds(1),
                    MaxDelay = TimeSpan.FromSeconds(30)
                }
            }));

        services.AddScoped<IServiceBusPublisher, ServiceBusPublisher>();

        return services;
    }

    // Security
    private static IServiceCollection AddSecurity(
        this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher<object>, PasswordHasher>();

        return services;
    }

    // Health Checks
    private static IServiceCollection AddHealthChecks(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services
            .AddHealthChecks()
            .AddSqlServer(
                connectionString: configuration.GetConnectionString("SqlServer")!,
                name: "sql-server",
                tags: ["db", "sql", "azure"])
            .AddAzureServiceBusTopic(
                connectionString: configuration.GetConnectionString("ServiceBus")!,
                topicName: "habit-events",
                name: "service-bus",
                tags: ["messaging", "azure"]);

        return services;
    }
}