using MediatR;

namespace HabitTracker.API.Application.Users.Commands.Deactivate;

public sealed record DeactivateUserCommand(Guid UserId) : IRequest<bool>;