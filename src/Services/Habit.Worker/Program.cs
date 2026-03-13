using HabitTracker.Worker;
using HabitTracker.Worker.Extensions;
using Serilog;

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = Host.CreateApplicationBuilder(args);

    // Serilog
    builder.Services.AddSerilog((ctx, cfg) =>
        cfg.ReadFrom.Configuration(builder.Configuration)
           .Enrich.FromLogContext()
           .Enrich.WithProperty("Application", "HabitTracker.Worker")
           .WriteTo.Console()
           .WriteTo.Seq(builder.Configuration["Seq:ServerUrl"]
               ?? "http://localhost:5341"));

    // Worker services — Service Bus + Consumers
    builder.Services.AddWorkerServices(builder.Configuration);

    // Background service
    builder.Services.AddHostedService<Worker>();

    // Health checks
    builder.Services.AddHealthChecks()
        .AddAzureServiceBusTopic(
            connectionString: builder.Configuration.GetConnectionString("ServiceBus")!,
            topicName: "habit-events",
            name: "service-bus",
            tags: ["messaging", "azure"]);

    var host = builder.Build();
    await host.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Worker terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}