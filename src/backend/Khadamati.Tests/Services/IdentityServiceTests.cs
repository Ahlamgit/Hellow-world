using FluentAssertions;
using Khadamati.Application.Interfaces;
using Khadamati.Domain.Constants;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Entities.Identity;
using Khadamati.Infrastructure.Data;
using Khadamati.Infrastructure.Repositories;
using Khadamati.Infrastructure.Services.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

namespace Khadamati.Tests.Services;

public class PermissionServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly PermissionService _permissionService;

    public PermissionServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options;
        _context = new ApplicationDbContext(options);

        var adminRole = new Role { Name = RoleNames.Admin, NameAr = "مدير" };
        var customerRole = new Role { Name = RoleNames.Customer, NameAr = "عميل" };
        _context.Roles.AddRange(adminRole, customerRole);

        var usersView = new Permission { Code = PermissionCodes.UsersView, NameEn = "View Users", NameAr = "عرض", Module = "Users" };
        var usersCreate = new Permission { Code = PermissionCodes.UsersCreate, NameEn = "Create Users", NameAr = "إنشاء", Module = "Users" };
        _context.Permissions.AddRange(usersView, usersCreate);
        _context.SaveChanges();

        _context.RolePermissions.Add(new RolePermission { RoleId = adminRole.Id, PermissionId = usersView.Id });
        _context.RolePermissions.Add(new RolePermission { RoleId = adminRole.Id, PermissionId = usersCreate.Id });
        _context.RolePermissions.Add(new RolePermission { RoleId = customerRole.Id, PermissionId = usersView.Id });

        var adminUser = new User { Email = "admin@test.com", Phone = "+966500000001", PasswordHash = "x", Role = Domain.Enums.UserRole.Administrator };
        var customerUser = new User { Email = "c@test.com", Phone = "+966500000002", PasswordHash = "x", Role = Domain.Enums.UserRole.Customer };
        _context.Users.AddRange(adminUser, customerUser);
        _context.SaveChanges();

        _context.UserRoles.Add(new UserRoleAssignment { UserId = adminUser.Id, RoleId = adminRole.Id, IsPrimary = true });
        _context.UserRoles.Add(new UserRoleAssignment { UserId = customerUser.Id, RoleId = customerRole.Id, IsPrimary = true });
        _context.SaveChanges();

        _permissionService = new PermissionService(new IdentityRepository(_context), new MemoryCache(new MemoryCacheOptions()));
        _adminUserId = adminUser.Id;
        _customerUserId = customerUser.Id;
    }

    private readonly Guid _adminUserId;
    private readonly Guid _customerUserId;

    [Fact]
    public async Task Admin_ShouldHaveUsersCreatePermission()
    {
        var has = await _permissionService.UserHasPermissionAsync(_adminUserId, PermissionCodes.UsersCreate);
        has.Should().BeTrue();
    }

    [Fact]
    public async Task Customer_ShouldNotHaveUsersCreatePermission()
    {
        var has = await _permissionService.UserHasPermissionAsync(_customerUserId, PermissionCodes.UsersCreate);
        has.Should().BeFalse();
    }

    [Fact]
    public async Task Customer_ShouldHaveUsersViewPermission()
    {
        var perms = await _permissionService.GetUserPermissionsAsync(_customerUserId);
        perms.Should().Contain(PermissionCodes.UsersView);
    }

    public void Dispose() => _context.Dispose();
}

public class PasswordPolicyServiceTests
{
  private readonly PasswordPolicyService _policy;

    public PasswordPolicyServiceTests()
    {
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Auth:PasswordPolicy:MinLength"] = "8",
                ["Auth:PasswordPolicy:RequireSpecialChar"] = "true",
            }).Build();
        var ctx = new ApplicationDbContext(new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString()).Options);
        _policy = new PasswordPolicyService(config, new IdentityRepository(ctx), new PasswordHasher());
    }

    [Fact]
    public void WeakPassword_ShouldThrow()
    {
        var act = () => _policy.ValidatePassword("short");
        act.Should().Throw<Application.Common.ValidationException>();
    }

    [Fact]
    public void StrongPassword_ShouldPass()
    {
        var act = () => _policy.ValidatePassword("Str0ng!Pass");
        act.Should().NotThrow();
    }
}
