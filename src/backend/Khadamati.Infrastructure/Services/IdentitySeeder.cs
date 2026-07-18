using Khadamati.Domain.Constants;
using Khadamati.Domain.Entities;
using Khadamati.Domain.Entities.Identity;
using Khadamati.Domain.Enums;
using Khadamati.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Khadamati.Infrastructure.Services;

public static class IdentitySeeder
{
    public static async Task SeedAsync(ApplicationDbContext context, ILogger logger, CancellationToken ct = default)
    {
        await SeedRolesAndPermissionsAsync(context, ct);
        await FinalizeUserRoleAssignmentsAsync(context, logger, ct);
    }

    public static Task SeedRolesAndPermissionsAsync(ApplicationDbContext context, CancellationToken ct = default) =>
        SeedRolesAndPermissionsInternalAsync(context, ct);

    /// <summary>
    /// Ensures permission catalog and role grants stay in sync when new permissions are added after initial seed.
    /// </summary>
    public static async Task EnsureRolePermissionGrantsAsync(ApplicationDbContext context, CancellationToken ct = default)
    {
        var existingCodes = await context.Permissions.Select(p => p.Code).ToListAsync(ct);
        var missingCodes = PermissionCodes.All
            .Where(code => !existingCodes.Contains(code, StringComparer.OrdinalIgnoreCase))
            .ToList();

        if (missingCodes.Count > 0)
        {
            context.Permissions.AddRange(missingCodes.Select(code => new Permission
            {
                Code = code,
                NameEn = code.Replace('.', ' '),
                NameAr = code,
                Module = code.Split('.')[0],
                RequiresEmailVerification = PermissionCodes.RequiresEmailVerification.Contains(code),
            }));
            await context.SaveChangesAsync(ct);
        }

        var roles = await context.Roles.ToDictionaryAsync(r => r.Name, r => r.Id, ct);
        var permissions = await context.Permissions.ToDictionaryAsync(p => p.Code, p => p.Id, StringComparer.OrdinalIgnoreCase, ct);
        var existingGrants = await context.RolePermissions
            .Select(rp => new { rp.RoleId, rp.PermissionId })
            .ToListAsync(ct);
        var grantSet = existingGrants.Select(g => (g.RoleId, g.PermissionId)).ToHashSet();

        void EnsureGrant(string roleName, params string[] codes)
        {
            if (!roles.TryGetValue(roleName, out var roleId)) return;
            foreach (var code in codes)
            {
                if (!permissions.TryGetValue(code, out var permissionId)) continue;
                if (grantSet.Add((roleId, permissionId)))
                    context.RolePermissions.Add(new RolePermission { RoleId = roleId, PermissionId = permissionId });
            }
        }

        EnsureGrant(RoleNames.SuperAdmin, PermissionCodes.All);
        EnsureGrant(RoleNames.Admin,
            PermissionCodes.UsersView, PermissionCodes.UsersCreate, PermissionCodes.UsersEdit, PermissionCodes.UsersDelete,
            PermissionCodes.UsersSuspend, PermissionCodes.UsersVerifyEmail,
            PermissionCodes.SessionsView, PermissionCodes.SessionsRevoke, PermissionCodes.SessionsRevokeAll,
            PermissionCodes.BookingsView, PermissionCodes.BookingsApprove, PermissionCodes.BookingsCancel,
            PermissionCodes.SubscriptionsView, PermissionCodes.SubscriptionsCreate, PermissionCodes.SubscriptionsEdit,
            PermissionCodes.AdvertisementsManage, PermissionCodes.ReportsView, PermissionCodes.ReportsExport,
            PermissionCodes.PaymentsView, PermissionCodes.PaymentsUpdate, PermissionCodes.SettingsManage,
            PermissionCodes.RolesView, PermissionCodes.PermissionsView,
            PermissionCodes.AuditLogsView, PermissionCodes.SecurityLogsView, PermissionCodes.LoginHistoryView);

        if (context.ChangeTracker.HasChanges())
            await context.SaveChangesAsync(ct);
    }

    public static async Task FinalizeUserRoleAssignmentsAsync(
        ApplicationDbContext context,
        ILogger logger,
        CancellationToken ct = default)
    {
        await MigrateLegacyUsersAsync(context, ct);
        await EnsureSuperAdminAsync(context, logger, ct);
    }

    private static async Task SeedRolesAndPermissionsInternalAsync(ApplicationDbContext context, CancellationToken ct)
    {
        if (await context.Roles.AnyAsync(ct)) return;

        var roleDefs = new (string Name, string NameAr)[]
        {
            (RoleNames.SuperAdmin, "مدير عام"),
            (RoleNames.Admin, "مدير"),
            (RoleNames.SupportAgent, "وكيل دعم"),
            (RoleNames.Moderator, "مشرف"),
            (RoleNames.Customer, "عميل"),
            (RoleNames.Craftsman, "حرفي"),
            (RoleNames.StoreOwner, "صاحب متجر"),
            (RoleNames.StoreEmployee, "موظف متجر"),
            (RoleNames.Accountant, "محاسب"),
        };

        var roles = roleDefs.Select(r => new Role { Name = r.Name, NameAr = r.NameAr, IsSystemRole = true }).ToList();
        context.Roles.AddRange(roles);
        await context.SaveChangesAsync(ct);

        var permissions = PermissionCodes.All.Select(code => new Permission
        {
            Code = code,
            NameEn = code.Replace('.', ' '),
            NameAr = code,
            Module = code.Split('.')[0],
            RequiresEmailVerification = PermissionCodes.RequiresEmailVerification.Contains(code),
        }).ToList();
        context.Permissions.AddRange(permissions);
        await context.SaveChangesAsync(ct);

        var roleMap = roles.ToDictionary(r => r.Name, r => r.Id);
        var permMap = permissions.ToDictionary(p => p.Code, p => p.Id);

        var rolePermissions = new List<RolePermission>();

        void Grant(string role, params string[] perms)
        {
            foreach (var p in perms)
                rolePermissions.Add(new RolePermission { RoleId = roleMap[role], PermissionId = permMap[p] });
        }

        Grant(RoleNames.SuperAdmin, PermissionCodes.All);
        Grant(RoleNames.Admin,
            PermissionCodes.UsersView, PermissionCodes.UsersCreate, PermissionCodes.UsersEdit, PermissionCodes.UsersDelete,
            PermissionCodes.UsersSuspend, PermissionCodes.UsersVerifyEmail,
            PermissionCodes.SessionsView, PermissionCodes.SessionsRevoke, PermissionCodes.SessionsRevokeAll,
            PermissionCodes.BookingsView, PermissionCodes.BookingsApprove, PermissionCodes.BookingsCancel,
            PermissionCodes.SubscriptionsView, PermissionCodes.SubscriptionsCreate, PermissionCodes.SubscriptionsEdit,
            PermissionCodes.AdvertisementsManage, PermissionCodes.ReportsView, PermissionCodes.ReportsExport,
            PermissionCodes.PaymentsView, PermissionCodes.PaymentsUpdate, PermissionCodes.SettingsManage,
            PermissionCodes.RolesView, PermissionCodes.PermissionsView,
            PermissionCodes.AuditLogsView, PermissionCodes.SecurityLogsView, PermissionCodes.LoginHistoryView);
        Grant(RoleNames.SupportAgent, PermissionCodes.UsersView, PermissionCodes.BookingsView, PermissionCodes.LoginHistoryView);
        Grant(RoleNames.Moderator, PermissionCodes.UsersView, PermissionCodes.BookingsView, PermissionCodes.BookingsCancel);
        Grant(RoleNames.Customer, PermissionCodes.BookingsView, PermissionCodes.BookingsCreate, PermissionCodes.BookingsCancel, PermissionCodes.BookingsEdit, PermissionCodes.SessionsView);
        Grant(RoleNames.Craftsman, PermissionCodes.BookingsView, PermissionCodes.BookingsApprove, PermissionCodes.BookingsCancel, PermissionCodes.BookingsEdit, PermissionCodes.SessionsView);
        Grant(RoleNames.StoreOwner, PermissionCodes.BookingsView, PermissionCodes.AdvertisementsManage, PermissionCodes.SessionsView);
        Grant(RoleNames.StoreEmployee, PermissionCodes.BookingsView, PermissionCodes.SessionsView);
        Grant(RoleNames.Accountant, PermissionCodes.PaymentsView, PermissionCodes.PaymentsUpdate, PermissionCodes.ReportsView, PermissionCodes.ReportsExport);

        context.RolePermissions.AddRange(rolePermissions);
        await context.SaveChangesAsync(ct);
    }

    private static async Task MigrateLegacyUsersAsync(ApplicationDbContext context, CancellationToken ct)
    {
        var roles = await context.Roles.ToDictionaryAsync(r => r.Name, r => r, ct);
        var users = await context.Users.Include(u => u.UserRoles).Where(u => u.PrimaryRoleId == null).ToListAsync(ct);

        foreach (var user in users)
        {
            var roleName = user.Role switch
            {
                UserRole.Administrator => RoleNames.Admin,
                UserRole.Store => RoleNames.StoreOwner,
                UserRole.Craftsman => RoleNames.Craftsman,
                _ => RoleNames.Customer,
            };

            if (!roles.TryGetValue(roleName, out var role)) continue;
            user.PrimaryRoleId = role.Id;

            if (!user.UserRoles.Any(ur => ur.RoleId == role.Id))
            {
                context.UserRoles.Add(new UserRoleAssignment
                {
                    UserId = user.Id,
                    RoleId = role.Id,
                    IsPrimary = true,
                    AssignedBy = "Migration",
                });
            }
        }

        if (users.Count > 0)
            await context.SaveChangesAsync(ct);
    }

    private static async Task EnsureSuperAdminAsync(ApplicationDbContext context, ILogger logger, CancellationToken ct)
    {
        var superAdminRole = await context.Roles.FirstOrDefaultAsync(r => r.Name == RoleNames.SuperAdmin, ct);
        if (superAdminRole is null) return;

        var admin = await context.Users
            .Include(u => u.Profile)
            .Include(u => u.UserRoles)
            .Where(u => u.Role == UserRole.Administrator)
            .OrderBy(u => u.Id)
            .FirstOrDefaultAsync(ct);

        if (admin is null) return;

        admin.PrimaryRoleId = superAdminRole.Id;
        admin.Role = UserRole.Administrator;
        admin.EmailVerifiedAt ??= DateTime.UtcNow;
        admin.VerificationStatus = VerificationStatus.Verified;
        admin.Status = UserStatus.Active;

        if (!admin.UserRoles.Any(ur => ur.RoleId == superAdminRole.Id))
        {
            context.UserRoles.Add(new UserRoleAssignment
            {
                UserId = admin.Id,
                RoleId = superAdminRole.Id,
                IsPrimary = true,
                AssignedBy = "Seeder",
            });
        }

        await context.SaveChangesAsync(ct);
        logger.LogInformation("SuperAdmin role assigned to {Email}.", admin.Email);
    }
}
