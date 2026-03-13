using HabitTracker.API.Domain.Entities.UserAggregate;
using HabitTracker.API.Domain.Interfaces;
using HabitTracker.API.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HabitTracker.API.Infrastructure.Persistence;

public sealed class UserRepository(AppDbContext context) : IUserRepository
{
    // With tracking — needed for updates
    public async Task<User?> GetByIdAsync(
        Guid id,
        CancellationToken cancellationToken = default)
    {
        return await context.Users
                .FirstOrDefaultAsync(
                    u => u.Id == id,
                    cancellationToken);
    }

    // AsNoTracking — read-only query
    public async Task<User?> GetByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                u => u.Email.Value.Equals(email, StringComparison.InvariantCultureIgnoreCase),
                cancellationToken);
    }

    // Lightweight existence check — no entity loaded
    public async Task<bool> ExistsAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        return await context.Users
                .AnyAsync(
                    u => u.Email.Value.Equals(email, StringComparison.InvariantCultureIgnoreCase),
                    cancellationToken);
    }

    public async Task AddAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        await context.Users.AddAsync(user, cancellationToken);
    }

    public Task UpdateAsync(
        User user,
        CancellationToken cancellationToken = default)
    {
        context.Users.Update(user);
        return Task.CompletedTask;
    }
}