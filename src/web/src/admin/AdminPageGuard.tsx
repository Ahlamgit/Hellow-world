import type { ReactNode } from 'react';
import { Navigate } from 'react-router-dom';
import { Alert, Box } from '@mui/material';
import { useAuth } from '../context/AuthContext';
import { canAccessAdmin, canAccessAdminPath, canViewModule, hasElevatedAdminRole } from '../utils/permissions';

interface AdminPageGuardProps {
  children: ReactNode;
  /** Admin path e.g. /admin/analytics — used with permissionForAdminPath */
  path?: string;
  /** Module key e.g. bookings — used with viewPermissionForModule */
  module?: string;
}

export default function AdminPageGuard({ children, path, module }: AdminPageGuardProps) {
  const { user } = useAuth();

  if (!user || !canAccessAdmin(user)) {
    return (
      <Box sx={{ py: 4 }}>
        <Alert severity="warning">You do not have permission to access this section.</Alert>
      </Box>
    );
  }

  // SuperAdmin / Admin / Administrator — full navigation inside the portal.
  if (hasElevatedAdminRole(user)) {
    return <>{children}</>;
  }

  const allowed = module
    ? canViewModule(user, module)
    : path
      ? canAccessAdminPath(user, path)
      : true;

  if (!allowed) {
    return (
      <Box sx={{ py: 4 }}>
        <Alert severity="warning">You do not have permission to access this section.</Alert>
      </Box>
    );
  }

  return <>{children}</>;
}

/** Redirect when user lacks path permission (for top-level route guards). */
export function AdminPathRedirect({ path, children }: { path: string; children: ReactNode }) {
  const { user } = useAuth();
  if (!canAccessAdminPath(user, path)) return <Navigate to="/admin" replace />;
  return <>{children}</>;
}
