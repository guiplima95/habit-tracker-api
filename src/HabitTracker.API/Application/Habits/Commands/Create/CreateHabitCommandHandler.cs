using HabitTracker.API.Application.Common;
using HabitTracker.API.Application.Common.Interfaces;
using HabitTracker.API.Domain.Entities.HabitAggregate;
using HabitTracker.API.Domain.Interfaces;
using HabitTracker.API.Domain.Notifications;
using MediatR;

namespace HabitTracker.API.Application.Habits.Commands.Create;

public sealed class CreateHabitCommandHandler(
    IHabitRepository habitRepository,
    IServiceBusPublisher publisher,
    IUnitOfWork unitOfWork) : IRequestHandler<CreateHabitCommand, Result<CreateHabitResponse>>
{
    public async Task<Result<CreateHabitResponse>> Handle(
        CreateHabitCommand request,
        CancellationToken cancellationToken)
    {
        var notification = new Notification();

        var habit = Habit.Create(
            request.Name,
            request.FrequencyDays,
            notification);

        // Return failure — no exceptions!
        if (notification.IsInvalid)
        {
            return Result<CreateHabitResponse>.Failure(notification.Errors);
        }

        await habitRepository.AddAsync(habit, cancellationToken);
        await unitOfWork.CommitAsync(cancellationToken);

        foreach (var domainEvent in habit.DomainEvents)
            await publisher.PublishAsync(domainEvent, cancellationToken);

        habit.ClearDomainEvents();

        return Result<CreateHabitResponse>.Success(habit); // implicit operator
    }
}