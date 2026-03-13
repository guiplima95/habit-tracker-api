using HabitTracker.API.Domain.Entities.HabitAggregate;

namespace HabitTracker.API.Application.Habits.Queries;

public sealed record HabitResponse(
    Guid Id,
    string Name,
    int FrequencyDays,
    string FrequencyType,
    bool IsActive,
    DateTime CreatedAt,
    IEnumerable<HabitLogResponse> Logs)
{
    // Implicit operator — Habit → HabitResponse
    public static implicit operator HabitResponse(Habit habit) =>
        new(
            habit.Id,
            habit.Name.Value,
            habit.Frequency.Days,
            habit.Frequency.Type.Name,
            habit.IsActive,
            habit.CreatedAt,
            habit.Logs.Select(log => (HabitLogResponse)log)); // chains implicit operator
}