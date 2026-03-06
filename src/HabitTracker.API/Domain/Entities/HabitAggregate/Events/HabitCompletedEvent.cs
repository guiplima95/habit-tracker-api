namespace HabitTracker.API.Domain.Entities.HabitAggregate.Events;

public sealed record HabitCompletedEvent(
    Guid Id,
    Guid HabitLogId,
    DateTime CompletedAt);