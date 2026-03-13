using System.Reflection;

namespace HabitTracker.API.Domain.SeedWork;

public abstract class Enumeration : IComparable
{
    public string Name { get; }

    public int Id { get; }

    protected Enumeration(int id, string name) => (Id, Name) = (id, name);

    public override string ToString() => Name;

    public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
        typeof(T).GetFields(BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
                    .Select(f => f.GetValue(null))
                    .Cast<T>();

    public override bool Equals(object? obj)
    {
        if (obj is not Enumeration otherValue)
        {
            return false;
        }

        bool typeMatches = GetType().Equals(obj.GetType());
        bool valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
    {
        return Math.Abs(firstValue.Id - secondValue.Id);
    }

    public static T FromValue<T>(int value) where T : Enumeration
    {
        return Parse<T, int>(value, "value", item => item.Id == value);
    }

    public static T FromDisplayName<T>(string displayName) where T : Enumeration
    {
        return Parse<T, string>(displayName, "display name", item => item.Name == displayName);
    }

    private static T Parse<T, K>(K value, string description, Func<T, bool> predicate) where T : Enumeration
    {
        return GetAll<T>().FirstOrDefault(predicate)
            ?? throw new InvalidOperationException($"'{value}' is not a valid {description} in {typeof(T)}");
    }

    public int CompareTo(object? obj)
    {
        return obj is null ? throw new ArgumentNullException(nameof(obj)) : Id.CompareTo(((Enumeration)obj).Id);
    }

    public static bool operator ==(Enumeration firstValue, Enumeration secondValue)
    {
        if (Equals(firstValue, null))
        {
            return Equals(secondValue, null);
        }

        return firstValue.Equals(secondValue);
    }

    public static bool operator !=(Enumeration left, Enumeration right)
    {
        return !(left == right);
    }

    public static bool operator <(Enumeration firstValue, Enumeration secondValue)
    {
        ArgumentNullException.ThrowIfNull(firstValue, nameof(firstValue));

        return firstValue.CompareTo(secondValue) == -1;
    }

    public static bool operator <=(Enumeration firstValue, Enumeration secondValue)
    {
        ArgumentNullException.ThrowIfNull(firstValue, nameof(firstValue));

        return firstValue.CompareTo(secondValue) <= 0;
    }

    public static bool operator >=(Enumeration firstValue, Enumeration secondValue)
    {
        ArgumentNullException.ThrowIfNull(firstValue, nameof(firstValue));

        return firstValue.CompareTo(secondValue) >= 0;
    }

    public static bool operator >(Enumeration firstValue, Enumeration secondValue)
    {
        ArgumentNullException.ThrowIfNull(firstValue, nameof(firstValue));

        return firstValue.CompareTo(secondValue) == 1;
    }
}