import { hasAdminAccess as hasLegacyAdminRole } from './roles';

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

export function hasPermission(
  user: { permissions?: string[] } | null | undefined,
  code: string,
): boolean {
  if (!user?.permissions?.length) return false;
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
  user: { permissions?: string[] } | null | undefined,
): boolean {
  return hasAnyPermission(user, PORTAL_PERMISSIONS);
}

/** Legacy admin roles or any portal permission. */
export function canAccessAdmin(
  user: Parameters<typeof hasLegacyAdminRole>[0] & { permissions?: string[] },
): boolean {
  if (!user) return false;
  if (hasLegacyAdminRole(user)) return true;
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
  user: Parameters<typeof canAccessAdmin>[0],
  module: string,
): boolean {
  if (!user) return false;
  if (hasLegacyAdminRole(user)) return true;
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
  user: Parameters<typeof canAccessAdmin>[0],
  path: string,
): boolean {
  if (!canAccessAdmin(user)) return false;
  if (hasLegacyAdminRole(user)) return true;
  const required = permissionForAdminPath(path);
  if (!required) return hasAdminPortalAccess(user);
  return hasPermission(user, required);
}
