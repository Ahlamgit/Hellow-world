using Khadamati.Domain.Constants;

namespace Khadamati.Application.Authorization;

/// <summary>Maps admin modules and endpoints to permission codes.</summary>
public static class AdminPermissionMap
{
    public static readonly string[] PortalPermissions =
    [
        PermissionCodes.UsersView,
        PermissionCodes.BookingsView,
        PermissionCodes.BookingsApprove,
        PermissionCodes.SubscriptionsView,
        PermissionCodes.AdvertisementsManage,
        PermissionCodes.ReportsView,
        PermissionCodes.ReportsExport,
        PermissionCodes.PaymentsView,
        PermissionCodes.PaymentsUpdate,
        PermissionCodes.SettingsManage,
        PermissionCodes.RolesView,
        PermissionCodes.RolesManage,
        PermissionCodes.PermissionsView,
        PermissionCodes.PermissionsManage,
        PermissionCodes.AuditLogsView,
        PermissionCodes.SecurityLogsView,
        PermissionCodes.LoginHistoryView,
        PermissionCodes.SessionsView,
    ];

    public static string ViewPermissionForModule(string module) =>
        module.ToLowerInvariant() switch
        {
            "users" or "customers" or "craftsmen" or "stores" or "complaints" or "support-tickets" or "notifications" =>
                PermissionCodes.UsersView,
            "bookings" => PermissionCodes.BookingsView,
            "subscriptions" => PermissionCodes.SubscriptionsView,
            "advertisements" => PermissionCodes.AdvertisementsManage,
            "payments" => PermissionCodes.PaymentsView,
            "reports" => PermissionCodes.ReportsView,
            "categories" or "services" or "coupons" or "cities" or "regions" or "settings" or "backup" or "restore" =>
                PermissionCodes.SettingsManage,
            "roles" => PermissionCodes.RolesView,
            "permissions" => PermissionCodes.PermissionsView,
            "audit-logs" => PermissionCodes.AuditLogsView,
            "activity-logs" => PermissionCodes.SecurityLogsView,
            _ => PermissionCodes.UsersView,
        };

    public static string EditPermissionForModule(string module) =>
        module.ToLowerInvariant() switch
        {
            "users" or "customers" or "craftsmen" or "stores" => PermissionCodes.UsersEdit,
            "bookings" => PermissionCodes.BookingsEdit,
            "subscriptions" => PermissionCodes.SubscriptionsEdit,
            "advertisements" => PermissionCodes.AdvertisementsManage,
            "payments" => PermissionCodes.PaymentsUpdate,
            "categories" or "services" or "coupons" or "cities" or "regions" or "settings" or "backup" or "restore" =>
                PermissionCodes.SettingsManage,
            "roles" => PermissionCodes.RolesManage,
            "permissions" => PermissionCodes.PermissionsManage,
            _ => PermissionCodes.UsersEdit,
        };

    public static string BulkPermissionForModule(string module, string action)
    {
        var normalized = action.ToLowerInvariant();
        if (normalized is "delete" && module is "users" or "customers" or "craftsmen" or "stores")
            return PermissionCodes.UsersDelete;
        if (normalized is "suspend")
            return PermissionCodes.UsersSuspend;
        return EditPermissionForModule(module);
    }
}
