using FluentAssertions;
using Khadamati.Application.DTOs.Admin;
using Khadamati.Application.Interfaces;
using Khadamati.Application.Validators;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Enums;
using Khadamati.Infrastructure.Data;
using Khadamati.Infrastructure.Repositories;
using Khadamati.Infrastructure.Services;
using Khadamati.Infrastructure.Services.Integrations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Khadamati.Tests.Services;

public class AdminServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly AdminService _adminService;

    public AdminServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);
        var export = new AdminExportService();
        var unitOfWork = new UnitOfWork(_context);
        var readiness = new IntegrationReadinessService(new ConfigurationBuilder().Build());
        _adminService = new AdminService(_context, export, unitOfWork, new NoOpPermissionService(), readiness);

        SeedData();
    }

    private void SeedData()
    {
        var admin = new User
        {
            Email = "admin@test.com",
            Phone = "+966500000001",
            PasswordHash = "hash",
            Role = UserRole.Administrator,
            Status = UserStatus.Active,
            VerificationStatus = VerificationStatus.Verified,
            SubscriptionStatus = SubscriptionStatus.Active,
            Profile = new UserProfile { FirstName = "Admin", LastName = "User", PreferredLanguage = "en" },
        };
        var customer = new User
        {
            Email = "customer@test.com",
            Phone = "+966500000002",
            PasswordHash = "hash",
            Role = UserRole.Customer,
            Status = UserStatus.Active,
            VerificationStatus = VerificationStatus.Verified,
            SubscriptionStatus = SubscriptionStatus.Active,
            Profile = new UserProfile { FirstName = "Test", LastName = "Customer", PreferredLanguage = "en" },
        };
        _context.Users.AddRange(admin, customer);
        _context.Regions.Add(new Region { NameEn = "Riyadh", NameAr = "الرياض", Code = "RYD" });
        _context.SaveChanges();
    }

    [Fact]
    public async Task GetDashboardAsync_ReturnsCounts()
    {
        var result = await _adminService.GetDashboardAsync();
        result.TotalUsers.Should().Be(2);
        result.TotalCustomers.Should().Be(1);
    }

    [Fact]
    public async Task ListModuleAsync_Users_ReturnsPaginatedResults()
    {
        var result = await _adminService.ListModuleAsync("users", new AdminListQueryDto { Page = 1, PageSize = 10 });
        result.Items.Should().HaveCount(2);
        result.TotalCount.Should().Be(2);
    }

    [Fact]
    public async Task ListModuleAsync_Customers_FiltersByRole()
    {
        var result = await _adminService.ListModuleAsync("customers", new AdminListQueryDto { Page = 1, PageSize = 10 });
        result.Items.Should().HaveCount(1);
        result.Items[0].Columns["email"].Should().Be("customer@test.com");
    }

    [Fact]
    public async Task ListModuleAsync_Regions_ReturnsSeededData()
    {
        var result = await _adminService.ListModuleAsync("regions", new AdminListQueryDto { Page = 1, PageSize = 10 });
        result.Items.Should().HaveCount(1);
        result.Items[0].Columns["nameEn"].Should().Be("Riyadh");
    }

    [Fact]
    public async Task BulkActionAsync_ActivateUser_UpdatesStatus()
    {
        var customer = await _context.Users.FirstAsync(u => u.Role == UserRole.Customer);
        customer.Status = UserStatus.Suspended;
        await _context.SaveChangesAsync();

        var result = await _adminService.BulkActionAsync("customers", new AdminBulkActionDto
        {
            Action = "activate",
            Ids = [customer.Id.ToString()],
        }, null);

        result.AffectedCount.Should().Be(1);
        (await _context.Users.FindAsync(customer.Id))!.Status.Should().Be(UserStatus.Active);
    }

    [Fact]
    public async Task ExportAsync_Users_ReturnsExcelBytes()
    {
        var bytes = await _adminService.ExportAsync("users", "xlsx", new AdminListQueryDto { Page = 1, PageSize = 10 });
        bytes.Should().NotBeEmpty();
        bytes[0].Should().Be(0x50); // PK zip header for xlsx
    }

    [Fact]
    public async Task ExportAsync_Users_ReturnsPdfBytes()
    {
        var bytes = await _adminService.ExportAsync("users", "pdf", new AdminListQueryDto { Page = 1, PageSize = 10 });
        bytes.Should().NotBeEmpty();
        System.Text.Encoding.ASCII.GetString(bytes.Take(4).ToArray()).Should().Be("%PDF");
    }

    [Fact]
    public async Task GetSystemHealthAsync_ReturnsHealthyStatus()
    {
        var health = await _adminService.GetSystemHealthAsync();
        health.Status.Should().Be("Healthy");
        health.DatabaseConnected.Should().BeTrue();
    }

    [Fact]
    public async Task CreateBackupAsync_CreatesBackupJob()
    {
        var backup = await _adminService.CreateBackupAsync(null);
        backup.Name.Should().StartWith("backup-");
        (await _context.BackupJobs.CountAsync()).Should().Be(1);
    }

    public void Dispose() => _context.Dispose();

    private sealed class NoOpPermissionService : IPermissionService
    {
        public Task<IReadOnlyList<string>> GetUserPermissionsAsync(Guid userId, CancellationToken cancellationToken = default) =>
            Task.FromResult<IReadOnlyList<string>>(Array.Empty<string>());
        public Task<bool> UserHasPermissionAsync(Guid userId, string permissionCode, CancellationToken cancellationToken = default) =>
            Task.FromResult(false);
        public void InvalidateCache(Guid userId) { }
        public Task InvalidateCacheForRoleAsync(Guid roleId, CancellationToken cancellationToken = default) =>
            Task.CompletedTask;
    }
}
