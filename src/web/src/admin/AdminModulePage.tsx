import { useParams } from 'react-router-dom';
import { Alert, Box } from '@mui/material';
import AdminDataTable from './AdminDataTable';
import AdminUsersPage from './AdminUsersPage';
import AdminUserSubscriptionsPage from './AdminUserSubscriptionsPage';
import AdminPageGuard from './AdminPageGuard';
import { getModuleConfig } from './moduleConfig';
import AdminBackupPage from './AdminSpecialPages';

export default function AdminModulePage() {
  const { moduleKey } = useParams<{ moduleKey: string }>();
  const config = moduleKey ? getModuleConfig(moduleKey) : undefined;

  if (moduleKey === 'users') {
    return (
      <AdminPageGuard module="users">
        <AdminUsersPage />
      </AdminPageGuard>
    );
  }

  if (moduleKey === 'user-subscriptions') {
    return (
      <AdminPageGuard module="user-subscriptions">
        <AdminUserSubscriptionsPage />
      </AdminPageGuard>
    );
  }

  if (!config) {
    return <Alert severity="error">Module not found: {moduleKey}</Alert>;
  }

  if (moduleKey === 'backup') {
    return (
      <AdminPageGuard module="backup">
        <AdminBackupPage />
      </AdminPageGuard>
    );
  }

  return (
    <AdminPageGuard module={moduleKey!}>
      <Box>
        <AdminDataTable config={config} />
      </Box>
    </AdminPageGuard>
  );
}
