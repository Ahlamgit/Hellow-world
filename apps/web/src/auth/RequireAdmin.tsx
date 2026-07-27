import { CircularProgress, Box } from '@mui/material';
import { Navigate, Outlet, useLocation } from 'react-router-dom';
import { useAdminAuth } from './AdminAuthContext';
import { sanitizeReturnUrl } from './redirects';

export function RequireAdmin() {
  const { authStatus, isAuthenticated } = useAdminAuth();
  const location = useLocation();

  if (authStatus === 'INITIALIZING') {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', py: 8 }}>
        <CircularProgress />
      </Box>
    );
  }

  if (!isAuthenticated) {
    const returnUrl = sanitizeReturnUrl(`${location.pathname}${location.search}`);
    const search = returnUrl ? `?returnUrl=${encodeURIComponent(returnUrl)}` : '';
    return <Navigate to={`/admin/login${search}`} replace state={{ from: location }} />;
  }

  return <Outlet />;
}

export function AdminGuestOnly({ children }: { children: React.ReactNode }) {
  const { authStatus, isAuthenticated } = useAdminAuth();
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

  if (isAuthenticated) {
    const target = returnUrl?.startsWith('/admin') ? returnUrl : '/admin/dashboard';
    return <Navigate to={target} replace />;
  }

  return children;
}
