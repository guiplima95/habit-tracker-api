using HabitTracker.API.Domain.SeedWork;

namespace HabitTracker.API.Domain.Entities.HabitAggregate;

public sealed class HabitLog : Entity
{
    public Guid HabitId { get; private set; }
    public DateTime CompletedAt { get; private set; }
    public string? Notes { get; private set; }

    private HabitLog()
    { } // EF Core

    public static HabitLog Create(Guid habitId, string? notes = null)
    {
        ArgumentException.ThrowIfNullOrEmpty(nameof(habitId));

        return new HabitLog
        {
            HabitId = habitId,
            CompletedAt = DateTime.UtcNow,
            Notes = notes
        };
    }
}