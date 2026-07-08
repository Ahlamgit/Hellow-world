using Khadamati.Application.Common;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Khadamati.Infrastructure.Services.Identity;

public class PasswordPolicyService : IPasswordPolicyService
{
    private readonly IConfiguration _configuration;
    private readonly IIdentityRepository _identityRepository;
    private readonly IPasswordHasher _passwordHasher;

    public PasswordPolicyService(IConfiguration configuration, IIdentityRepository identityRepository, IPasswordHasher passwordHasher)
    {
        _configuration = configuration;
        _identityRepository = identityRepository;
        _passwordHasher = passwordHasher;
    }

    public void ValidatePassword(string password)
    {
        var minLength = _configuration.GetValue("Auth:PasswordPolicy:MinLength", 8);
        var requireUpper = _configuration.GetValue("Auth:PasswordPolicy:RequireUppercase", true);
        var requireLower = _configuration.GetValue("Auth:PasswordPolicy:RequireLowercase", true);
        var requireDigit = _configuration.GetValue("Auth:PasswordPolicy:RequireDigit", true);
        var requireSpecial = _configuration.GetValue("Auth:PasswordPolicy:RequireSpecialChar", true);

        var errors = new List<string>();
        if (string.IsNullOrWhiteSpace(password) || password.Length < minLength)
            errors.Add($"Password must be at least {minLength} characters.");
        if (requireUpper && !password.Any(char.IsUpper))
            errors.Add("Password must contain an uppercase letter.");
        if (requireLower && !password.Any(char.IsLower))
            errors.Add("Password must contain a lowercase letter.");
        if (requireDigit && !password.Any(char.IsDigit))
            errors.Add("Password must contain a digit.");
        if (requireSpecial && !password.Any(c => !char.IsLetterOrDigit(c)))
            errors.Add("Password must contain a special character.");

        if (errors.Count > 0)
            throw new ValidationException(errors);
    }

    public async Task ValidatePasswordNotReusedAsync(Guid userId, string newPassword, CancellationToken cancellationToken = default)
    {
        var historyCount = _configuration.GetValue("Auth:PasswordPolicy:HistoryCount", 5);
        var history = await _identityRepository.GetRecentPasswordHistoryAsync(userId, historyCount, cancellationToken);
        if (history.Any(h => _passwordHasher.Verify(newPassword, h.PasswordHash)))
            throw new ValidationException(["Password was used recently. Choose a different password."]);
    }
}
