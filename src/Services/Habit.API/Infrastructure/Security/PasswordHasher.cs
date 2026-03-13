using Microsoft.AspNetCore.Identity;
using PasswordVerificationResult = Microsoft.AspNetCore.Identity.PasswordVerificationResult;

namespace HabitTracker.API.Infrastructure.Security;

public sealed class PasswordHasher : IPasswordHasher<object>
{
    private readonly PasswordHasher<object> _hasher = new();

    public string HashPassword(object user, string password) =>
        _hasher.HashPassword(user, password);

    public PasswordVerificationResult VerifyHashedPassword(
        object user,
        string hashedPassword,
        string providedPassword) =>
            _hasher.VerifyHashedPassword(user, hashedPassword, providedPassword);
}