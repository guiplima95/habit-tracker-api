using HabitTracker.API.Application.Common;
using HabitTracker.API.Application.Habits.Commands.Create;
using HabitTracker.API.Application.Habits.Commands.Deactivate;
using HabitTracker.API.Application.Habits.Queries;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HabitTracker.API.Apis.Endpoints;

public static class HabitApi
{
    public static void MapEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/habits")
            .WithTags("Habits")
            .WithOpenApi();

        group.MapGet("/", GetAllHabits)
            .WithName("GetAllHabits")
            .WithSummary("Gets all habits paginated")
            .Produces<PagedResponse<HabitSummaryResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{id:guid}", GetHabitById)
            .WithName("GetHabitById")
            .WithSummary("Gets a habit by ID")
            .Produces<HabitResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateHabit)
            .WithName("CreateHabit")
            .WithSummary("Creates a new habit")
            .Produces<CreateHabitResponse>(StatusCodes.Status201Created)
            .Produces<ValidationProblemDetails>(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status422UnprocessableEntity);

        group.MapDelete("/{id:guid}", DeactivateHabit)
            .WithName("DeactivateHabit")
            .WithSummary("Deactivates a habit")
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> GetAllHabits(
       ISender sender,
       CancellationToken cancellationToken,
       int pageNumber = 1,
       int pageSize = 10,
       bool? isActive = null,
       string? searchTerm = null)
    {
        var result = await sender.Send(
            new GetAllHabitsQuery(pageNumber, pageSize, isActive, searchTerm),
            cancellationToken);

        return result.IsFailure
            ? Results.UnprocessableEntity(result.Errors)
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> GetHabitById(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new GetHabitByIdQuery(id),
            cancellationToken);

        return result.IsFailure
            ? Results.NotFound(result.Errors)
            : Results.Ok(result.Value);
    }

    private static async Task<IResult> CreateHabit(
        CreateHabitCommand command,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(command, cancellationToken);

        return result.IsFailure
            ? Results.UnprocessableEntity(result.Errors)
            : Results.Created($"/api/habits/{result.Value!.Id}", result.Value);
    }

    private static async Task<IResult> DeactivateHabit(
        Guid id,
        ISender sender,
        CancellationToken cancellationToken)
    {
        var result = await sender.Send(
            new DeactivateHabitCommand(id),
            cancellationToken);

        return result.IsFailure
            ? Results.UnprocessableEntity(result.Errors)
            : Results.NoContent();
    }
}