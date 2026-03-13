namespace HabitTracker.API.Domain.Entities.HabitAggregate.Events;

public sealed record HabitDeactivatedEvent(
    Guid Id,
    DateTime DeactivatedAt);