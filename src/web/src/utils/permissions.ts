/** Permission codes that grant access to the admin portal (mirrors backend AdminPermissionMap.PortalPermissions). */
export const PORTAL_PERMISSIONS = [
  'Users.View',
  'Bookings.View',
  'Bookings.Approve',
  'Subscriptions.View',
  'Advertisements.Manage',
  'Reports.View',
  'Reports.Export',
  'Payments.View',
  'Payments.Update',
  'Settings.Manage',
  'Roles.View',
  'Roles.Manage',
  'Permissions.View',
  'Permissions.Manage',
  'AuditLogs.View',
  'SecurityLogs.View',
  'LoginHistory.View',
  'Sessions.View',
] as const;

export type PermissionCode = (typeof PORTAL_PERMISSIONS)[number] | string;

const ELEVATED_ADMIN_ROLES = ['SuperAdmin', 'Admin', 'Administrator'] as const;

function hasElevatedAdminRole(
  user: { role?: string; primaryRole?: string; roles?: string[] } | null | undefined,
): boolean {
  if (!user) return false;
  const role = user.primaryRole ?? user.role ?? '';
  if (ELEVATED_ADMIN_ROLES.includes(role as typeof ELEVATED_ADMIN_ROLES[number])) return true;
  return user.roles?.some((r) => ELEVATED_ADMIN_ROLES.includes(r as typeof ELEVATED_ADMIN_ROLES[number])) ?? false;
}

export function hasPermission(
  user: { permissions?: string[]; role?: string; primaryRole?: string; roles?: string[] } | null | undefined,
  code: string,
): boolean {
  if (!user) return false;
  if (hasElevatedAdminRole(user)) return true;
  if (!user.permissions?.length) return false;
  const normalized = code.toLowerCase();
  return user.permissions.some((p) => p.toLowerCase() === normalized);
}

export function hasAnyPermission(
  user: { permissions?: string[] } | null | undefined,
  codes: readonly string[],
): boolean {
  return codes.some((code) => hasPermission(user, code));
}

/** True when the user has any admin-portal permission. */
export function hasAdminPortalAccess(
  user: { permissions?: string[]; role?: string; primaryRole?: string; roles?: string[] } | null | undefined,
): boolean {
  if (!user) return false;
  if (hasElevatedAdminRole(user)) return true;
  return hasAnyPermission(user, PORTAL_PERMISSIONS);
}

/** Portal access is permission-based only. */
export function canAccessAdmin(
  user: { permissions?: string[] } | null | undefined,
): boolean {
  return hasAdminPortalAccess(user);
}

/** View permission for a generic admin module key (mirrors backend AdminPermissionMap). */
export function viewPermissionForModule(module: string): string {
  switch (module.toLowerCase()) {
    case 'users':
    case 'customers':
    case 'craftsmen':
    case 'stores':
    case 'complaints':
    case 'support-tickets':
    case 'verification-documents':
    case 'notifications':
      return 'Users.View';
    case 'bookings':
      return 'Bookings.View';
    case 'subscriptions':
    case 'user-subscriptions':
      return 'Subscriptions.View';
    case 'advertisements':
      return 'Advertisements.Manage';
    case 'payments':
      return 'Payments.View';
    case 'reports':
      return 'Reports.View';
    case 'categories':
    case 'services':
    case 'coupons':
    case 'cities':
    case 'regions':
    case 'settings':
    case 'backup':
    case 'restore':
      return 'Settings.Manage';
    case 'roles':
      return 'Roles.View';
    case 'permissions':
      return 'Permissions.View';
    case 'audit-logs':
      return 'AuditLogs.View';
    case 'activity-logs':
      return 'SecurityLogs.View';
    default:
      return 'Users.View';
  }
}

export function canViewModule(
  user: { permissions?: string[] } | null | undefined,
  module: string,
): boolean {
  if (!user) return false;
  return hasPermission(user, viewPermissionForModule(module));
}

/** Nav path → required view permission. */
export function permissionForAdminPath(path: string): string | null {
  if (path === '/admin') return null; // dashboard — portal access only
  const key = path.replace(/^\/admin\/?/, '').split('/')[0];
  if (!key) return null;
  if (key === 'analytics' || key === 'reports') return 'Reports.View';
  if (key === 'system-health') return 'Settings.Manage';
  return viewPermissionForModule(key);
}

export function canAccessAdminPath(
  user: { permissions?: string[] } | null | undefined,
  path: string,
): boolean {
  if (!canAccessAdmin(user)) return false;
  const required = permissionForAdminPath(path);
  if (!required) return hasAdminPortalAccess(user);
  return hasPermission(user, required);
}

export function canCreateUsers(user: { permissions?: string[] } | null | undefined): boolean {
  return hasPermission(user, 'Users.Create');
}

export function canEditUsers(user: { permissions?: string[] } | null | undefined): boolean {
  return hasPermission(user, 'Users.Edit');
}

export function canDeleteUsers(user: { permissions?: string[] } | null | undefined): boolean {
  return hasPermission(user, 'Users.Delete');
}

export function canSuspendUsers(user: { permissions?: string[] } | null | undefined): boolean {
  return hasPermission(user, 'Users.Suspend');
}

export function canVerifyUserEmail(user: { permissions?: string[] } | null | undefined): boolean {
  return hasPermission(user, 'Users.VerifyEmail');
}

export function canCreateSubscriptionPlans(user: { permissions?: string[] } | null | undefined): boolean {
  return hasPermission(user, 'Subscriptions.Create');
}

export function canEditSubscriptionPlans(user: { permissions?: string[] } | null | undefined): boolean {
  return hasPermission(user, 'Subscriptions.Edit');
}
