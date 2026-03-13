using HabitTracker.API.Domain.Entities.UserAggregate;

namespace HabitTracker.API.Application.Users.Commands.Create;

public sealed record CreateUserResponse(
    Guid Id,
    string Name,
    string Email,
    bool IsActive,
    DateTime CreatedAt)
{
    // Implicit operator — User → CreateUserResponse
    public static implicit operator CreateUserResponse(User user) =>
        new(
            user.Id,
            user.Name.Value,
            user.Email.Value,
            user.IsActive,
            user.CreatedAt);
}