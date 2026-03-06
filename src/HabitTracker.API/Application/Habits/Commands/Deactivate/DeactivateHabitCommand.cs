using HabitTracker.API.Application.Common;
using MediatR;

namespace HabitTracker.API.Application.Habits.Commands.Deactivate;

public sealed record DeactivateHabitCommand(Guid Id) : IRequest<Result<bool>>;