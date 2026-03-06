using HabitTracker.API.Domain.Notifications;
using HabitTracker.API.Domain.SeedWork;
using HabitTracker.API.Domain.ValueObjects;

namespace HabitTracker.API.Domain.Entities.HabitAggregate;

public sealed class Habit : Entity
{
    public HabitName Name { get; private set; } = null!;
    public Frequency Frequency { get; private set; } = null!;
    public bool IsActive { get; private set; }
    public DateTime CreatedAt { get; private set; }

    private readonly List<object> _domainEvents = [];

    private readonly List<HabitLog> _logs = [];
    public IReadOnlyCollection<HabitLog> Logs => _logs;

    private Habit()
    { } // EF Core

    public static Habit Create(
        string name,
        int frequencyDays,
        Notification notification)
    {
        var habitName = HabitName.Create(name, notification);
        var frequency = Frequency.Create(frequencyDays, notification);

        return new Habit
        {
            Name = habitName,
            Frequency = frequency,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };
    }

    public void Complete(Notification notification)
    {
        if (!IsActive)
        {
            notification.AddError("Habit", "Cannot complete an inactive habit.");
            return;
        }

        _logs.Add(HabitLog.Create(Id));
    }

    public void Deactivate() => IsActive = false;
}