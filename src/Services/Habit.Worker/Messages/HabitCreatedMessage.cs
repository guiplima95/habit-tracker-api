namespace HabitTracker.API.Worker.Messages;

public sealed record HabitCreatedMessage(
    Guid Id,
    string Name,
    int Frequency,
    DateTime CreatedAt);