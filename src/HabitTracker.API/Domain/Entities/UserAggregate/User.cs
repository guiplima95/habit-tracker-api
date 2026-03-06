using HabitTracker.API.Domain.Entities.UserAggregate.Events;
using HabitTracker.API.Domain.Notifications;
using HabitTracker.API.Domain.SeedWork;
using HabitTracker.API.Domain.ValueObjects;

namespace HabitTracker.API.Domain.Entities.UserAggregate;

public sealed class User : Entity
{
    public UserName Name { get; private set; } = null!;
    public Email Email { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<object> _domainEvents = [];

    private User()
    { } // EF Core

    public static User Create(
        string name,
        string email,
        Notification notification)
    {
        var userName = UserName.Create(name, notification);
        var userEmail = Email.Create(email, notification);

        var user = new User
        {
            Name = userName,
            Email = userEmail,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        user._domainEvents.Add(new UserCreatedEvent(
            user.Id,
            user.Name.Value,
            user.Email.Value,
            user.CreatedAt));

        return user;
    }

    public void UpdateName(string name, Notification notification)
    {
        var userName = UserName.Create(name, notification);

        if (notification.IsInvalid)
            return;

        Name = userName;
    }

    public void Deactivate(Notification notification)
    {
        if (!IsActive)
        {
            notification.AddError("User", "User is already inactive.");
            return;
        }

        IsActive = false;

        _domainEvents.Add(new UserDeactivatedEvent(
            Id,
            DateTime.UtcNow));
    }
}