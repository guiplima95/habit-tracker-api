using MediatR;

namespace HabitTracker.API.Application.Users.Commands.Create;

public sealed record CreateUserCommand(
    string Name,
    string Email,
    string Password) : IRequest<CreateUserResponse>;