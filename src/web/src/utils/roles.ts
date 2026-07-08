/** Roles that can access the admin dashboard (matches backend AdminOnly policy). */
const ADMIN_ROLES = new Set(['Admin', 'SuperAdmin', 'Administrator']);

export function isAdminRole(role: string | undefined | null): boolean {
  return !!role && ADMIN_ROLES.has(role);
}

export function hasAdminAccess(user: {
  role?: string;
  primaryRole?: string;
  roles?: string[];
} | null | undefined): boolean {
  if (!user) return false;
  if (isAdminRole(user.role) || isAdminRole(user.primaryRole)) return true;
  return user.roles?.some(isAdminRole) ?? false;
}
