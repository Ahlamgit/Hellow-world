using FluentAssertions;
using Khadamati.Application.DTOs.Users;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Entities.Identity;
using Khadamati.Domain.Enums;
using Khadamati.Domain.Interfaces;
using Khadamati.Infrastructure.Data;
using Khadamati.Infrastructure.Repositories;
using Khadamati.Infrastructure.Services;
using Khadamati.Infrastructure.Services.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;

namespace Khadamati.Tests.Services;

public class UserManagementServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly UserManagementService _service;
    private readonly Guid _adminUserId;
    private readonly Role _adminRole;
    private readonly Role _customerRole;

    public UserManagementServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        _context = new ApplicationDbContext(options);

        _adminRole = new Role { Name = RoleNames.Admin, NameAr = "مدير" };
        _customerRole = new Role { Name = RoleNames.Customer, NameAr = "عميل" };
        _context.Roles.AddRange(_adminRole, _customerRole);

        var permissions = PermissionCodes.All.Select(code => new Permission
        {
            Code = code,
            NameEn = code,
            NameAr = code,
            Module = code.Split('.')[0],
        }).ToList();
        _context.Permissions.AddRange(permissions);
        _context.SaveChanges();

        foreach (var perm in permissions)
            _context.RolePermissions.Add(new RolePermission { RoleId = _adminRole.Id, PermissionId = perm.Id });
        _context.SaveChanges();

        var adminUser = new User
        {
            Email = "admin@test.com",
            Phone = "+966500000001",
            PasswordHash = "x",
            Role = UserRole.Administrator,
            PrimaryRoleId = _adminRole.Id,
            Status = UserStatus.Active,
            Profile = new UserProfile { FirstName = "Admin", LastName = "User" },
        };
        _context.Users.Add(adminUser);
        _context.SaveChanges();
        _context.UserRoles.Add(new UserRoleAssignment { UserId = adminUser.Id, RoleId = _adminRole.Id, IsPrimary = true });
        _context.SaveChanges();
        _adminUserId = adminUser.Id;

        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Auth:PasswordPolicy:MinLength"] = "8",
                ["Auth:PasswordPolicy:RequireSpecialChar"] = "true",
            })
            .Build();

        var unitOfWork = new UnitOfWork(_context);
        var passwordHasher = new PasswordHasher();
        var identityRepository = new IdentityRepository(_context);
        var permissionService = new PermissionService(identityRepository, new MemoryCache(new MemoryCacheOptions()));
        var passwordPolicy = new PasswordPolicyService(config, identityRepository, passwordHasher);
        var tokenService = new TokenService(new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Jwt:Secret"] = "Khadamati-Super-Secret-Key-Minimum-32-Characters-Long-2024!" })
            .Build());

        _service = new UserManagementService(
            new UserRepository(_context),
            identityRepository,
            new AuthRepository(_context),
            unitOfWork,
            passwordHasher,
            passwordPolicy,
            permissionService,
            Mock.Of<IEmailService>(),
            tokenService,
            Mock.Of<IAuditService>(),
            Mock.Of<ILogger<UserManagementService>>());
    }

    [Fact]
    public async Task Create_ShouldCreateCustomerUser()
    {
        var dto = new CreateAdminUserDto
        {
            Email = "new@test.com",
            Phone = "+966509999999",
            Password = "Password1!",
            FirstName = "New",
            LastName = "User",
            Role = RoleNames.Customer,
            Status = "Active",
            SendVerificationEmail = false,
        };

        var result = await _service.CreateAsync(dto, _adminUserId, "127.0.0.1");

        result.Email.Should().Be("new@test.com");
        result.PrimaryRole.Should().Be(RoleNames.Customer);
        result.Status.Should().Be("Active");
    }

    [Fact]
    public async Task Search_ShouldReturnPagedUsers()
    {
        var result = await _service.SearchAsync(new AdminUserListQueryDto { Page = 1, PageSize = 10 });
        result.Items.Should().NotBeEmpty();
        result.TotalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Suspend_ShouldUpdateStatus()
    {
        var created = await _service.CreateAsync(new CreateAdminUserDto
        {
            Email = "suspend@test.com",
            Phone = "+966508888888",
            Password = "Password1!",
            FirstName = "Suspend",
            LastName = "Me",
            Role = RoleNames.Customer,
            Status = "Active",
            SendVerificationEmail = false,
        }, _adminUserId, null);

        var result = await _service.SuspendAsync(created.Id, new SuspendUserDto { Reason = "Test" }, _adminUserId);
        result.Status.Should().Be("Suspended");
    }

    [Fact]
    public async Task AssignRoles_ShouldUpdatePrimaryRole()
    {
        var craftsmanRole = new Role { Name = RoleNames.Craftsman, NameAr = "حرفي" };
        _context.Roles.Add(craftsmanRole);
        await _context.SaveChangesAsync();

        var created = await _service.CreateAsync(new CreateAdminUserDto
        {
            Email = "roles@test.com",
            Phone = "+966507777777",
            Password = "Password1!",
            FirstName = "Role",
            LastName = "Test",
            Role = RoleNames.Customer,
            Status = "Active",
            SendVerificationEmail = false,
        }, _adminUserId, null);

        var updated = await _service.AssignRolesAsync(created.Id, new AssignUserRolesDto
        {
            Roles = [RoleNames.Craftsman, RoleNames.Customer],
            PrimaryRole = RoleNames.Craftsman,
        }, _adminUserId);

        updated.PrimaryRole.Should().Be(RoleNames.Craftsman);
        updated.Roles.Should().Contain(RoleNames.Craftsman);
    }

    public void Dispose() => _context.Dispose();
}
