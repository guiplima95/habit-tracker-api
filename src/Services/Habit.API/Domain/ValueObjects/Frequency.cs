using HabitTracker.API.Domain.Entities.HabitAggregate;
using HabitTracker.API.Domain.Notifications;

namespace HabitTracker.API.Domain.ValueObjects;

public sealed class Frequency : IEquatable<Frequency>
{
    public int Days { get; }
    public HabitFrequency Type { get; }

    public const int MinDays = 1;
    public const int MaxDays = 365;

    private Frequency(int days, HabitFrequency type)
    {
        Days = days;
        Type = type;
    }

    public static Frequency Create(int days, Notification notification)
    {
        if (days < MinDays || days > MaxDays)
        {
            notification.AddError(
                "Frequency",
                $"Frequency must be between {MinDays} and {MaxDays} days.");

            return new Frequency(0, HabitFrequency.Daily);
        }

        var type = days switch
        {
            1 => HabitFrequency.Daily,
            <= 7 => HabitFrequency.Weekly,
            _ => HabitFrequency.Monthly
        };

        return new Frequency(days, type);
    }

    public bool Equals(Frequency? other) => other?.Days == Days && other.Type == Type;

    public override bool Equals(object? obj) => obj is Frequency f && Equals(f);

    public override int GetHashCode() => HashCode.Combine(Days, Type);

    public override string ToString() => $"Every {Days} day(s) ({Type})";
}