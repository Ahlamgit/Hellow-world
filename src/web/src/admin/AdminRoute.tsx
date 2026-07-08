import { Navigate, Outlet } from 'react-router-dom';
import { Box, CircularProgress } from '@mui/material';
import { useAuth } from '../context/AuthContext';
import { hasAdminAccess } from '../utils/roles';

export function AdminRoute() {
  const { isAuthenticated, isLoading, user } = useAuth();

  if (isLoading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '100vh' }}>
        <CircularProgress />
      </Box>
    );
  }

  if (!isAuthenticated) return <Navigate to="/login" replace />;
  if (!hasAdminAccess(user)) return <Navigate to="/dashboard" replace />;

  return <Outlet />;
}
