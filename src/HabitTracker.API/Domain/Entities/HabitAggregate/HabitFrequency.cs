using HabitTracker.API.Domain.SeedWork;

namespace HabitTracker.API.Domain.Entities.HabitAggregate;

public sealed class HabitFrequency : Enumeration
{
    public static readonly HabitFrequency Daily = new(1, "Daily");
    public static readonly HabitFrequency Weekly = new(2, "Weekly");
    public static readonly HabitFrequency Monthly = new(3, "Monthly");

    private HabitFrequency(int id, string name) : base(id, name)
    {
    }
}