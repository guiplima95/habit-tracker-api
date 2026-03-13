using HabitTracker.API.Domain.Entities.HabitAggregate;
using HabitTracker.API.Domain.Interfaces;
using HabitTracker.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.API.Infrastructure.Persistence;

public sealed class HabitRepository(AppDbContext context) : IHabitRepository
{
    public async Task<Habit?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await context.Habits
                .Include(h => h.Logs)
                .FirstOrDefaultAsync(h => h.Id == id, cancellationToken);
    }

    // AsNoTracking — read-only, better performance
    public async Task<IEnumerable<Habit>> GetAllAsync(
        CancellationToken cancellationToken = default)
    {
        return await context.Habits
            .Include(h => h.Logs)
            .AsNoTracking()
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(
        Habit habit,
        CancellationToken cancellationToken = default)
    {
        await context.Habits.AddAsync(habit, cancellationToken);
    }

    public Task UpdateAsync(
        Habit habit,
        CancellationToken cancellationToken = default)
    {
        context.Habits.Update(habit);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        var habit = await GetByIdAsync(id, cancellationToken);

        if (habit is not null)
        {
            context.Habits.Remove(habit);
        }
    }
}