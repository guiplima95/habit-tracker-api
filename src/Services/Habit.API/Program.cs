using HabitTracker.API.Apis.Endpoints;
using HabitTracker.API.Apis.Middleware;
using HabitTracker.API.Application.Extensions;
using HabitTracker.API.Infrastructure.Extensions;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using OpenTelemetry.Trace;
using Serilog;
using System.Text.Json;

// Bootstrap logger — captures startup errors before DI is built
Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateBootstrapLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // Serilog — structured logging
    builder.Host.UseSerilog((ctx, cfg) =>
        cfg.ReadFrom.Configuration(ctx.Configuration)
           .Enrich.FromLogContext()
           .Enrich.WithProperty("Application", "HabitTracker.API")
           .WriteTo.Console()
           .WriteTo.Seq(ctx.Configuration["Seq:ServerUrl"] ?? "http://localhost:5341"));

    // OpenTelemetry — distributed tracing
    builder.Services.AddOpenTelemetry()
        .WithTracing(tracing =>
            tracing
                .AddAspNetCoreInstrumentation()
                .AddConsoleExporter());

    // Application layer — MediatR, FluentValidation, Behaviors
    builder.Services.AddApplication();

    // Infrastructure layer — EF Core, Repositories, Service Bus, Health Checks
    builder.Services.AddInfrastructure(builder.Configuration);

    // Swagger / OpenAPI
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new()
        {
            Title = "HabitTracker API",
            Version = "v1",
            Description = "Scalable Habit Tracker REST API — .NET 9"
        });
    });

    // CORS
    builder.Services.AddCors(options =>
        options.AddDefaultPolicy(policy =>
            policy.AllowAnyOrigin()
                  .AllowAnyMethod()
                  .AllowAnyHeader()));

    // JSON — camelCase globally
    builder.Services.ConfigureHttpJsonOptions(options =>
        options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase);

    var app = builder.Build();

    // MUST be first — wraps entire pipeline
    app.UseMiddleware<ExceptionHandlingMiddleware>();

    // Serilog request logging
    app.UseSerilogRequestLogging(options =>
        options.MessageTemplate =
            "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms");

    app.UseCors();
    app.UseHttpsRedirection();

    // Swagger — development only
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "HabitTracker API v1");
            options.RoutePrefix = string.Empty; // Swagger at root "/"
        });
    }

    // Endpoints
    app.MapEndpoints();
    //app.MapUserEndpoints();

    // Health checks — detailed JSON response
    app.MapHealthChecks("/health", new HealthCheckOptions
    {
        ResponseWriter = async (context, report) =>
        {
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new
            {
                status = report.Status.ToString(),
                duration = report.TotalDuration.TotalMilliseconds,
                checks = report.Entries.Select(e => new
                {
                    name = e.Key,
                    status = e.Value.Status.ToString(),
                    duration = e.Value.Duration.TotalMilliseconds,
                    tags = e.Value.Tags
                })
            });
        }
    });

    await app.RunAsync();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    await Log.CloseAndFlushAsync();
}