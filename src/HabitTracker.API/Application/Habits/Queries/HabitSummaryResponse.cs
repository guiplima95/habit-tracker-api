namespace HabitTracker.API.Application.Habits.Queries;

public sealed record HabitSummaryResponse(
    Guid Id,
    string Name,
    int Frequency,
    string FrequencyType,
    bool IsActive,
    DateTime CreatedAt,
    int TotalCompletions);