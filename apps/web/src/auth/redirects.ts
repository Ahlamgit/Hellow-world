import type { ApiRole, PortalRole } from './types';

export function apiRoleToPortal(role: ApiRole): PortalRole | null {
  switch (role) {
    case 'CUSTOMER':
      return 'customer';
    case 'CRAFTSMAN':
      return 'provider';
    case 'STORE':
      return 'store';
    case 'ADMIN':
    case 'FINANCE_ADMIN':
    case 'SUPER_ADMIN':
      return 'admin';
    default:
      return null;
  }
}

export function portalDashboardPath(portal: PortalRole): string {
  switch (portal) {
    case 'customer':
      return '/home';
    case 'provider':
      return '/provider';
    case 'store':
      return '/store';
    case 'admin':
      return '/admin/dashboard';
  }
}

export function portalCanAccessPath(portal: PortalRole, path: string): boolean {
  if (!path.startsWith('/')) {
    return false;
  }

  if (portal === 'customer') {
    return path.startsWith('/customer') || path.startsWith('/home') || /^\/services\/[^/]+\/book$/.test(path);
  }
  if (portal === 'provider') {
    return path.startsWith('/provider');
  }
  if (portal === 'store') {
    return path.startsWith('/store');
  }
  return path.startsWith('/admin');
}

export function resolvePostLoginPath(portal: PortalRole, returnUrl: string | null): string {
  if (returnUrl && portalCanAccessPath(portal, returnUrl)) {
    return returnUrl;
  }
  return portalDashboardPath(portal);
}

export function sanitizeReturnUrl(value: string | null): string | null {
  if (!value || !value.startsWith('/') || value.startsWith('//')) {
    return null;
  }
  return value;
}

export function withReturnUrl(path: string, returnUrl: string | null): string {
  if (!returnUrl) {
    return path;
  }
  return `${path}?returnUrl=${encodeURIComponent(returnUrl)}`;
}
