import { useEffect, useState } from 'react';
import {
  Alert, Box, Button, Card, CardContent, Chip, CircularProgress, Grid, Snackbar,
  Table, TableBody, TableCell, TableContainer, TableHead, TableRow, Typography,
} from '@mui/material';
import { Backup, CheckCircle, Error as ErrorIcon } from '@mui/icons-material';
import { adminApi } from './adminApi';
import AdminDataTable from './AdminDataTable';
import { getModuleConfig } from './moduleConfig';
import type { AdminAnalytics, AdminSystemHealth } from './moduleConfig';

function ChartBar({ label, value, max }: { label: string; value: number; max: number }) {
  const pct = max > 0 ? (value / max) * 100 : 0;
  return (
    <Box sx={{ mb: 1.5 }}>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 0.5 }}>
        <Typography variant="body2">{label}</Typography>
        <Typography variant="body2" sx={{ fontWeight: 600 }}>{value.toLocaleString()}</Typography>
      </Box>
      <Box sx={{ bgcolor: 'action.hover', borderRadius: 1, height: 8 }}>
        <Box sx={{ bgcolor: 'primary.main', borderRadius: 1, height: 8, width: `${pct}%` }} />
      </Box>
    </Box>
  );
}

export function AdminAnalyticsPage() {
  const [data, setData] = useState<AdminAnalytics | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    adminApi.getAnalytics()
      .then(({ data: res }) => setData(res.data))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <CircularProgress />;
  if (!data) return <Alert severity="error">Failed to load analytics</Alert>;

  const maxBookings = Math.max(...data.bookingsByMonth.map((x) => x.value), 1);
  const maxRevenue = Math.max(...data.revenueByMonth.map((x) => x.value), 1);
  const maxUsers = Math.max(...data.usersByRole.map((x) => x.value), 1);

  return (
    <Box>
      <Typography variant="h4" sx={{ fontWeight: 700, mb: 3 }}>Analytics</Typography>
      <Grid container spacing={3}>
        <Grid size={{ xs: 12, md: 4 }}>
          <Card elevation={0} sx={{ border: 1, borderColor: 'divider' }}>
            <CardContent>
              <Typography variant="h6" gutterBottom>Bookings by Month</Typography>
              {data.bookingsByMonth.map((p) => <ChartBar key={p.label} label={p.label} value={p.value} max={maxBookings} />)}
            </CardContent>
          </Card>
        </Grid>
        <Grid size={{ xs: 12, md: 4 }}>
          <Card elevation={0} sx={{ border: 1, borderColor: 'divider' }}>
            <CardContent>
              <Typography variant="h6" gutterBottom>Revenue by Month (SAR)</Typography>
              {data.revenueByMonth.map((p) => <ChartBar key={p.label} label={p.label} value={p.value} max={maxRevenue} />)}
            </CardContent>
          </Card>
        </Grid>
        <Grid size={{ xs: 12, md: 4 }}>
          <Card elevation={0} sx={{ border: 1, borderColor: 'divider' }}>
            <CardContent>
              <Typography variant="h6" gutterBottom>Users by Role</Typography>
              {data.usersByRole.map((p) => <ChartBar key={p.label} label={p.label} value={p.value} max={maxUsers} />)}
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Box>
  );
}

export function AdminSystemHealthPage() {
  const [health, setHealth] = useState<AdminSystemHealth | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const load = () => adminApi.getSystemHealth().then(({ data: res }) => setHealth(res.data)).finally(() => setLoading(false));
    load();
    const interval = setInterval(load, 30000);
    return () => clearInterval(interval);
  }, []);

  if (loading) return <CircularProgress />;
  if (!health) return <Alert severity="error">Failed to load system health</Alert>;

  const isHealthy = health.status === 'Healthy';

  return (
    <Box>
      <Typography variant="h4" sx={{ fontWeight: 700, mb: 3 }}>System Health</Typography>
      <Grid container spacing={3}>
        <Grid size={{ xs: 12, md: 6 }}>
          <Card elevation={0} sx={{ border: 1, borderColor: 'divider' }}>
            <CardContent>
              <Box sx={{ display: 'flex', alignItems: 'center', gap: 2, mb: 2 }}>
                {isHealthy ? <CheckCircle color="success" sx={{ fontSize: 48 }} /> : <ErrorIcon color="error" sx={{ fontSize: 48 }} />}
                <Box>
                  <Typography variant="h5" sx={{ fontWeight: 700 }}>{health.status}</Typography>
                  <Typography variant="body2" color="text.secondary">Last checked: {new Date(health.timestamp).toLocaleString()}</Typography>
                </Box>
              </Box>
              <Chip label={`API ${health.apiVersion}`} size="small" sx={{ mr: 1 }} />
              <Chip label={`Memory: ${health.memoryUsedMb} MB`} size="small" />
            </CardContent>
          </Card>
        </Grid>
        <Grid size={{ xs: 12, md: 6 }}>
          <Card elevation={0} sx={{ border: 1, borderColor: 'divider' }}>
            <CardContent>
              <Typography variant="h6" gutterBottom>Database</Typography>
              <TableContainer>
                <Table size="small">
                  <TableBody>
                    <TableRow>
                      <TableCell>Connected</TableCell>
                      <TableCell><Chip label={health.databaseConnected ? 'Yes' : 'No'} color={health.databaseConnected ? 'success' : 'error'} size="small" /></TableCell>
                    </TableRow>
                    <TableRow>
                      <TableCell>Response Time</TableCell>
                      <TableCell>{health.databaseResponseMs} ms</TableCell>
                    </TableRow>
                  </TableBody>
                </Table>
              </TableContainer>
            </CardContent>
          </Card>
        </Grid>
        {health.integrations && health.integrations.length > 0 && (
          <Grid size={{ xs: 12 }}>
            <Card elevation={0} sx={{ border: 1, borderColor: 'divider' }}>
              <CardContent>
                <Box sx={{ display: 'flex', alignItems: 'center', gap: 1, mb: 2 }}>
                  <Typography variant="h6">Production integrations</Typography>
                  <Chip
                    label={health.productionIntegrationsReady ? 'Ready' : 'Not ready'}
                    color={health.productionIntegrationsReady ? 'success' : 'warning'}
                    size="small"
                  />
                </Box>
                <TableContainer>
                  <Table size="small">
                    <TableHead>
                      <TableRow>
                        <TableCell>Category</TableCell>
                        <TableCell>Provider</TableCell>
                        <TableCell>Status</TableCell>
                        <TableCell>Notes</TableCell>
                      </TableRow>
                    </TableHead>
                    <TableBody>
                      {health.integrations.map((integration) => (
                        <TableRow key={integration.category}>
                          <TableCell>{integration.category}</TableCell>
                          <TableCell>{integration.selectedProvider}</TableCell>
                          <TableCell>
                            <Chip
                              label={integration.status}
                              size="small"
                              color={
                                integration.status === 'Ready'
                                  ? 'success'
                                  : integration.status === 'Development'
                                    ? 'default'
                                    : 'warning'
                              }
                            />
                          </TableCell>
                          <TableCell>
                            {integration.missingSettings?.length
                              ? `Missing: ${integration.missingSettings.join(', ')}`
                              : integration.warnings?.length
                                ? integration.warnings.join(' ')
                                : '—'}
                          </TableCell>
                        </TableRow>
                      ))}
                    </TableBody>
                  </Table>
                </TableContainer>
              </CardContent>
            </Card>
          </Grid>
        )}
      </Grid>
    </Box>
  );
}

export default function AdminBackupPage() {
  const config = getModuleConfig('backup')!;
  const [creating, setCreating] = useState(false);
  const [snack, setSnack] = useState('');

  const handleCreate = async () => {
    setCreating(true);
    try {
      const { data } = await adminApi.createBackup();
      setSnack(`Backup created: ${data.data.name}`);
      window.location.reload();
    } catch {
      setSnack('Backup creation failed');
    } finally {
      setCreating(false);
    }
  };

  return (
    <Box>
      <AdminDataTable
        config={config}
        extraActions={
          <Button variant="contained" startIcon={<Backup />} disabled={creating} onClick={handleCreate}>
            Create Backup
          </Button>
        }
      />
      <Snackbar open={!!snack} autoHideDuration={4000} onClose={() => setSnack('')} message={snack} />
    </Box>
  );
}

export function AdminRestorePage() {
  const config = getModuleConfig('restore')!;
  const [restoring, setRestoring] = useState<string | null>(null);
  const [snack, setSnack] = useState<{ message: string; severity: 'success' | 'error' } | null>(null);

  const handleRestore = async (id: string) => {
    if (!window.confirm('Are you sure you want to restore from this backup? This action cannot be undone.')) return;
    setRestoring(id);
    try {
      const { data } = await adminApi.restoreBackup(id);
      setSnack({ message: data.data.message, severity: 'success' });
    } catch {
      setSnack({ message: 'Restore failed', severity: 'error' });
    } finally {
      setRestoring(null);
    }
  };

  return (
    <Box>
      <AdminDataTable
        config={config}
        onRowAction={(id) => handleRestore(id)}
        extraActions={
          <Typography variant="body2" color="text.secondary">
            Click a row to restore from that backup
          </Typography>
        }
      />
      {restoring && <Alert severity="info" sx={{ mt: 2 }}>Restoring backup {restoring}...</Alert>}
      {snack && (
        <Snackbar open autoHideDuration={5000} onClose={() => setSnack(null)}>
          <Alert severity={snack.severity}>{snack.message}</Alert>
        </Snackbar>
      )}
    </Box>
  );
}

export function AdminReportsPage() {
  const config = getModuleConfig('reports')!;
  return <AdminDataTable config={config} />;
}
