using HabitTracker.API.Domain.Notifications;
using System.Text.RegularExpressions;

namespace HabitTracker.API.Domain.ValueObjects;

public sealed class Email : IEquatable<Email>
{
    public string Value { get; }

    public const int MaxLength = 254;

    private static readonly Regex EmailRegex = new(
        @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$",
        RegexOptions.Compiled | RegexOptions.IgnoreCase);

    private Email(string value) => Value = value;

    public static Email Create(string value, Notification notification)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            notification.AddError("Email", "Email is required.");
            return new Email(string.Empty);
        }

        if (value.Length > MaxLength)
        {
            notification.AddError("Email", $"Email must be at most {MaxLength} characters.");
            return new Email(string.Empty);
        }

        if (!EmailRegex.IsMatch(value))
        {
            notification.AddError("Email", "Email format is invalid.");
            return new Email(string.Empty);
        }

        return new Email(value.Trim().ToLowerInvariant());
    }

    public bool Equals(Email? other) => other?.Value == Value;

    public override bool Equals(object? obj) => obj is Email email && Equals(email);

    public override int GetHashCode() => Value.GetHashCode();

    public override string ToString() => Value;
}