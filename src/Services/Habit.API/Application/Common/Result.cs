using HabitTracker.API.Domain.Notifications;

namespace HabitTracker.API.Application.Common;

public sealed class Result<T>
{
    public T? Value { get; }
    public IReadOnlyCollection<NotificationError> Errors { get; }
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;

    private Result(T value)
    {
        Value = value;
        IsSuccess = true;
        Errors = [];
    }

    private Result(IEnumerable<NotificationError> errors)
    {
        Value = default;
        IsSuccess = false;
        Errors = [.. errors];
    }

    // Factory methods
    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(IEnumerable<NotificationError> errors) => new(errors);

    public static Result<T> Failure(string key, string message) =>
        new([new NotificationError(key, message)]);
}