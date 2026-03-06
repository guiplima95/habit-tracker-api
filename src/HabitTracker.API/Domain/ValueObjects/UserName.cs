using HabitTracker.API.Domain.Notifications;

namespace HabitTracker.API.Domain.ValueObjects;

public sealed class UserName : IEquatable<UserName>
{
    public string Value { get; }

    public const int MinLength = 2;
    public const int MaxLength = 100;

    private UserName(string value) => Value = value;

    public static UserName Create(string value, Notification notification)
    {
        if (string.IsNullOrWhiteSpace(value))
            notification.AddError("UserName", "Name is required.");
        else if (value.Length < MinLength)
            notification.AddError("UserName", $"Name must be at least {MinLength} characters.");
        else if (value.Length > MaxLength)
            notification.AddError("UserName", $"Name must be at most {MaxLength} characters.");

        return new UserName(value?.Trim() ?? string.Empty);
    }

    public bool Equals(UserName? other) => other?.Value == Value;

    public override bool Equals(object? obj) => obj is UserName name && Equals(name);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}