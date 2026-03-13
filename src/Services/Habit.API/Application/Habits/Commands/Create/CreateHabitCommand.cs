using HabitTracker.API.Application.Common;
using MediatR;

namespace HabitTracker.API.Application.Habits.Commands.Create;

public sealed record CreateHabitCommand(
    string Name,
    int FrequencyDays,
    Guid UserId) : IRequest<Result<CreateHabitResponse>>;