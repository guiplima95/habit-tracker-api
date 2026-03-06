namespace HabitTracker.API.Domain.Notifications;

public sealed class Notification
{
    private readonly List<NotificationError> _errors = [];

    public IReadOnlyCollection<NotificationError> Errors => _errors;
    public bool IsValid => _errors.Count == 0;
    public bool IsInvalid => !IsValid;

    public void AddError(string key, string message) =>
        _errors.Add(new NotificationError(key, message));

    public void AddError(NotificationError error) =>
        _errors.Add(error);

    public void AddErrors(IEnumerable<NotificationError> errors) =>
        _errors.AddRange(errors);
}