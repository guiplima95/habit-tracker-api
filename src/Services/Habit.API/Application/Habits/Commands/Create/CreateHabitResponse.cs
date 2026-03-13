using HabitTracker.API.Domain.Entities.HabitAggregate;

namespace HabitTracker.API.Application.Habits.Commands.Create;

public sealed record CreateHabitResponse(
    Guid Id,
    string Name,
    int Frequency,
    string FrequencyType,
    bool IsActive,
    DateTime CreatedAt)
{
    // Implicit operator — Habit → CreateHabitResponse
    public static implicit operator CreateHabitResponse(Habit habit) =>
        new(
            habit.Id,
            habit.Name.Value,
            habit.Frequency.Days,
            habit.Frequency.Type.Name,
            habit.IsActive,
            habit.CreatedAt);
}