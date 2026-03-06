namespace HabitTracker.API.Domain.Entities.UserAggregate.Events;

public sealed record UserDeactivatedEvent(
    Guid Id,
    DateTime DeactivatedAt);