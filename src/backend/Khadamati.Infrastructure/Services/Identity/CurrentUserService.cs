using System.Security.Claims;
using Khadamati.Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Khadamati.Infrastructure.Services.Identity;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor) => _httpContextAccessor = httpContextAccessor;

  private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var id = User?.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(id, out var guid) ? guid : null;
        }
    }

    public string? Email => User?.FindFirstValue(ClaimTypes.Email);
    public string? PrimaryRole => User?.FindFirstValue(ClaimTypes.Role);
    public string? Role => PrimaryRole;
    public string? IpAddress => _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();

    public Guid? SessionId
    {
        get
        {
            var sid = _httpContextAccessor.HttpContext?.Request.Headers["X-Session-Id"].FirstOrDefault();
            return Guid.TryParse(sid, out var guid) ? guid : null;
        }
    }

    public IReadOnlyList<string> Permissions =>
        User?.FindAll("permission").Select(c => c.Value).ToList() ?? [];

    public bool EmailVerified =>
        bool.TryParse(User?.FindFirstValue("email_verified"), out var v) && v;
}
