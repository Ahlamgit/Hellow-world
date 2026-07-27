import { CircularProgress, Box } from '@mui/material';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAuth } from './AuthContext';
import type { PortalRole } from './types';
import { portalCanAccessPath, portalDashboardPath, sanitizeReturnUrl } from './redirects';

type RequireRoleProps = {
  roles: PortalRole[];
  loginPath?: string;
};

export function RequireRole({ roles, loginPath = '/login' }: RequireRoleProps) {
  const { authStatus, isAuthenticated, portal } = useAuth();
  const location = useLocation();

  if (authStatus === 'INITIALIZING') {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', py: 8 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (!isAuthenticated || !portal || !roles.includes(portal)) {
    const returnUrl = sanitizeReturnUrl(`${location.pathname}${location.search}`);
    const search = returnUrl ? `?returnUrl=${encodeURIComponent(returnUrl)}` : '';
    return <Navigate to={`${loginPath}${search}`} replace state={{ from: location }} />;
  }

  return <Outlet />;
}

export function GuestOnly({ children }: { children: React.ReactNode }) {
  const { authStatus, isAuthenticated, portal } = useAuth();
  const location = useLocation();
  const params = new URLSearchParams(location.search);
  const returnUrl = sanitizeReturnUrl(params.get('returnUrl'));

  if (authStatus === 'INITIALIZING') {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', py: 8 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (isAuthenticated && portal) {
    const target =
      returnUrl && portalCanAccessPath(portal, returnUrl)
        ? returnUrl
        : portalDashboardPath(portal);
    return <Navigate to={target} replace />;
  }

  return children;
}
