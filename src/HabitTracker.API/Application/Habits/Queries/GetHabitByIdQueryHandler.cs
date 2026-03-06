using HabitTracker.API.Application.Common;
using HabitTracker.API.Domain.Interfaces;
using MediatR;

namespace HabitTracker.API.Application.Habits.Queries;

public sealed class GetHabitByIdQueryHandler(
    IHabitRepository habitRepository) : IRequestHandler<GetHabitByIdQuery, Result<HabitResponse>>
{
    public async Task<Result<HabitResponse>> Handle(
        GetHabitByIdQuery request,
        CancellationToken cancellationToken)
    {
        var habit = await habitRepository.GetByIdAsync(
            request.Id,
            cancellationToken);

        if (habit is null)
            return Result<HabitResponse>.Failure(
                "Habit",
                $"Habit with id '{request.Id}' was not found.");

        return Result<HabitResponse>.Success(habit); // implicit operator
    }
}