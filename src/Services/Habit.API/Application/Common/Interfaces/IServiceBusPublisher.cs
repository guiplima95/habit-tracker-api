namespace HabitTracker.API.Application.Common.Interfaces;

public interface IServiceBusPublisher
{
    Task PublishAsync<T>(T message, CancellationToken cancellationToken = default)
        where T : class;
}