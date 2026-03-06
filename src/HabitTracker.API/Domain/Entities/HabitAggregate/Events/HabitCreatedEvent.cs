namespace HabitTracker.API.Domain.Entities.HabitAggregate.Events;

public sealed record HabitCreatedEvent(
    Guid Id,
    string Name,
    int Frequency,
    DateTime CreatedAt);