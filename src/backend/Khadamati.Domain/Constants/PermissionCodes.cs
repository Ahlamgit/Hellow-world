namespace Khadamati.Domain.Constants;

public static class PermissionCodes
{
    public const string UsersView = "Users.View";
    public const string UsersCreate = "Users.Create";
    public const string UsersEdit = "Users.Edit";
    public const string UsersDelete = "Users.Delete";
    public const string UsersSuspend = "Users.Suspend";
    public const string UsersVerifyEmail = "Users.VerifyEmail";

    public const string SessionsView = "Sessions.View";
    public const string SessionsRevoke = "Sessions.Revoke";
    public const string SessionsRevokeAll = "Sessions.RevokeAll";

    public const string BookingsView = "Bookings.View";
    public const string BookingsCreate = "Bookings.Create";
    public const string BookingsEdit = "Bookings.Edit";
    public const string BookingsCancel = "Bookings.Cancel";
    public const string BookingsApprove = "Bookings.Approve";

    public const string SubscriptionsView = "Subscriptions.View";
    public const string SubscriptionsCreate = "Subscriptions.Create";
    public const string SubscriptionsEdit = "Subscriptions.Edit";

    public const string AdvertisementsManage = "Advertisements.Manage";

    public const string ReportsView = "Reports.View";
    public const string ReportsExport = "Reports.Export";

    public const string PaymentsView = "Payments.View";
    public const string PaymentsUpdate = "Payments.Update";

    public const string SettingsManage = "Settings.Manage";

    public const string RolesView = "Roles.View";
    public const string RolesManage = "Roles.Manage";

    public const string PermissionsView = "Permissions.View";
    public const string PermissionsManage = "Permissions.Manage";

    public const string AuditLogsView = "AuditLogs.View";
    public const string SecurityLogsView = "SecurityLogs.View";
    public const string LoginHistoryView = "LoginHistory.View";

    public static readonly string[] All =
    [
        UsersView, UsersCreate, UsersEdit, UsersDelete, UsersSuspend, UsersVerifyEmail,
        SessionsView, SessionsRevoke, SessionsRevokeAll,
        BookingsView, BookingsCreate, BookingsEdit, BookingsCancel, BookingsApprove,
        SubscriptionsView, SubscriptionsCreate, SubscriptionsEdit,
        AdvertisementsManage,
        ReportsView, ReportsExport,
        PaymentsView, PaymentsUpdate,
        SettingsManage,
        RolesView, RolesManage,
        PermissionsView, PermissionsManage,
        AuditLogsView, SecurityLogsView, LoginHistoryView
    ];

    /// <summary>Permissions that require a verified email address.</summary>
    public static readonly HashSet<string> RequiresEmailVerification =
    [
        SubscriptionsCreate, SubscriptionsEdit,
        AdvertisementsManage,
        BookingsApprove,
        PaymentsUpdate,
        UsersCreate // creating store
    ];
}
