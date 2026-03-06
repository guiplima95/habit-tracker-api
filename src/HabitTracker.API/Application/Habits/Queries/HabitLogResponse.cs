using HabitTracker.API.Domain.Entities.HabitAggregate;

namespace HabitTracker.API.Application.Habits.Queries;

public sealed record HabitLogResponse(
    Guid Id,
    DateTime CompletedAt,
    string? Notes)
{
    // Implicit operator — HabitLog → HabitLogResponse
    public static implicit operator HabitLogResponse(HabitLog log) =>
        new(log.Id, log.CompletedAt, log.Notes);
}