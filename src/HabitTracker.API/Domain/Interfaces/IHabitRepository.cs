using HabitTracker.API.Domain.Entities.HabitAggregate;

namespace HabitTracker.API.Domain.Interfaces;

public interface IHabitRepository
{
    Task<Habit?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IEnumerable<Habit>> GetAllAsync(CancellationToken cancellationToken = default);

    Task AddAsync(Habit habit, CancellationToken cancellationToken = default);

    Task UpdateAsync(Habit habit, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}