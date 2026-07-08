import { useParams } from 'react-router-dom';
import { Alert, Box } from '@mui/material';
import AdminDataTable from './AdminDataTable';
import AdminUsersPage from './AdminUsersPage';
import { getModuleConfig } from './moduleConfig';
import AdminBackupPage from './AdminSpecialPages';

export default function AdminModulePage() {
  const { moduleKey } = useParams<{ moduleKey: string }>();
  const config = moduleKey ? getModuleConfig(moduleKey) : undefined;

  if (moduleKey === 'users') {
    return <AdminUsersPage />;
  }

  if (!config) {
    return <Alert severity="error">Module not found: {moduleKey}</Alert>;
  }

  if (moduleKey === 'backup') {
    return <AdminBackupPage />;
  }

  return (
    <Box>
      <AdminDataTable config={config} />
    </Box>
  );
}
