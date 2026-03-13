using HabitTracker.API.Domain.Notifications;

namespace HabitTracker.API.Domain.ValueObjects;

public sealed class HabitName : IEquatable<HabitName>
{
    public string Value { get; }

    public const int MaxLength = 100;
    public const int MinLength = 3;

    private HabitName(string value) => Value = value;

    public static HabitName Create(string value, Notification notification)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            notification.AddError("HabitName", "Name is required.");
        }
        if (value.Length < MinLength)
        {
            notification.AddError("HabitName", $"Name must be at least {MinLength} characters.");
        }
        if (value.Length > MaxLength)
        {
            notification.AddError("HabitName", $"Name must be at most {MaxLength} characters.");
        }

        return new HabitName(value);
    }

    public bool Equals(HabitName? other) => other?.Value == Value;

    public override bool Equals(object? obj) => obj is HabitName name && Equals(name);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}