namespace HabitTracker.API.Domain.Entities.UserAggregate.Events;

public sealed record UserCreatedEvent(
    Guid Id,
    string UserName,
    string Email,
    DateTime CreatedAt);