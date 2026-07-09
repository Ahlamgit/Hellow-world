using FluentAssertions;
using Khadamati.Application.DTOs.Support;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Infrastructure.Data;
using Khadamati.Infrastructure.Repositories;
using Khadamati.Application.Interfaces;
using Khadamati.Infrastructure.Services;
using Khadamati.Infrastructure.Services.Integrations;
using Microsoft.EntityFrameworkCore;

namespace Khadamati.Tests.Services;

public class AdminOperationsServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly AdminService _adminService;

    public AdminOperationsServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        _adminService = new AdminService(
            _context,
            new AdminExportService(),
            new UnitOfWork(_context),
            new NoOpPermissionService(),
            new IntegrationReadinessService(new Microsoft.Extensions.Configuration.ConfigurationBuilder().Build()));

        var user = new User
        {
            Email = "user@test.com",
            Phone = "+966500000099",
            PasswordHash = "hash",
            Role = UserRole.Customer,
            Status = UserStatus.Active,
            VerificationStatus = VerificationStatus.Verified,
            SubscriptionStatus = SubscriptionStatus.Active,
            Profile = new UserProfile { FirstName = "Test", LastName = "User", PreferredLanguage = "en" },
        };
        _context.Users.Add(user);
        _context.Complaints.Add(new Complaint
        {
            ComplainantUserId = user.Id,
            Complainant = user,
            Subject = "Late service",
            Description = "Craftsman arrived two hours late.",
            Status = "Open",
            Priority = "High",
        });
        _context.SaveChanges();
    }

    [Fact]
    public async Task ResolveComplaintAsync_SetsResolvedStatus()
    {
        var complaint = await _context.Complaints.FirstAsync();
        var result = await _adminService.ResolveComplaintAsync(
            complaint.Id,
            new ResolveComplaintDto { Resolution = "Issued apology and partial refund." },
            "admin-id");

        result.Status.Should().Be("Resolved");
        result.Resolution.Should().Contain("refund");
        result.ResolvedAt.Should().NotBeNull();
    }

    public void Dispose() => _context.Dispose();

    private sealed class NoOpPermissionService : IPermissionService
    {
        public Task<IReadOnlyList<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
        public Task<bool> UserHasPermissionAsync(Guid userId, string permissionCode, CancellationToken cancellationToken = default) =>
            Task.FromResult(true);
        public void InvalidateCache(Guid userId) { }
        public Task InvalidateCacheForRoleAsync(Guid roleId, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
