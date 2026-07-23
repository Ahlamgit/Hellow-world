import { useCallback, useEffect, useState } from 'react';
import {
  Alert, Box, Button, Card, CardContent, Chip, CircularProgress, Grid, IconButton, Paper, Snackbar,
  Table, TableBody, TableCell, TableContainer, TableHead, TableRow, TextField, Typography,
} from '@mui/material';
import { Backup, CheckCircle, Download, Error as ErrorIcon, Refresh } from '@mui/icons-material';
import { adminApi } from './adminApi';
import { DEFAULT_CURRENCY } from '../config/platform';
import AdminDataTable from './AdminDataTable';
import { getModuleConfig } from './moduleConfig';
import type { AdminAnalytics, AdminListResult, AdminSystemHealth } from './moduleConfig';

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
  const maxPayments = Math.max(...(data.paymentsByStatus ?? []).map((x) => x.value), 1);

  return (
    <Box>
      <Typography variant="h4" sx={{ fontWeight: 700, mb: 3 }}>Analytics</Typography>
      <Grid container spacing={3}>
        <Grid size={{ xs: 12, md: 3 }}>
          <Card elevation={0} sx={{ border: 1, borderColor: 'divider' }}>
            <CardContent>
              <Typography variant="h6" gutterBottom>Bookings by Month</Typography>
              {data.bookingsByMonth.map((p) => <ChartBar key={p.label} label={p.label} value={p.value} max={maxBookings} />)}
            </CardContent>
          </Card>
        </Grid>
        <Grid size={{ xs: 12, md: 3 }}>
          <Card elevation={0} sx={{ border: 1, borderColor: 'divider' }}>
            <CardContent>
              <Typography variant="h6" gutterBottom>Revenue by Month ({DEFAULT_CURRENCY})</Typography>
              {data.revenueByMonth.map((p) => <ChartBar key={p.label} label={p.label} value={p.value} max={maxRevenue} />)}
            </CardContent>
          </Card>
        </Grid>
        <Grid size={{ xs: 12, md: 3 }}>
          <Card elevation={0} sx={{ border: 1, borderColor: 'divider' }}>
            <CardContent>
              <Typography variant="h6" gutterBottom>Users by Role</Typography>
              {data.usersByRole.map((p) => <ChartBar key={p.label} label={p.label} value={p.value} max={maxUsers} />)}
            </CardContent>
          </Card>
        </Grid>
        <Grid size={{ xs: 12, md: 3 }}>
          <Card elevation={0} sx={{ border: 1, borderColor: 'divider' }}>
            <CardContent>
              <Typography variant="h6" gutterBottom>Payments by Status</Typography>
              {(data.paymentsByStatus ?? []).map((p) => <ChartBar key={p.label} label={p.label} value={p.value} max={maxPayments} />)}
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
  const [rows, setRows] = useState<AdminListResult['items']>([]);
  const [loading, setLoading] = useState(true);
  const [creating, setCreating] = useState(false);
  const [snack, setSnack] = useState<{ message: string; severity: 'success' | 'error' } | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const { data } = await adminApi.listModule('backup', { page: 1, pageSize: 50 });
      setRows(data.data.items);
    } catch {
      setSnack({ message: 'Failed to load backups', severity: 'error' });
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { load(); }, [load]);

  const handleCreate = async () => {
    setCreating(true);
    try {
      const { data } = await adminApi.createBackup();
      const backup = data.data;
      if (backup.status === 'Failed') {
        setSnack({ message: backup.errorMessage ?? 'Backup creation failed', severity: 'error' });
      } else {
        setSnack({ message: `Backup created: ${backup.name}`, severity: 'success' });
      }
      await load();
    } catch {
      setSnack({ message: 'Backup creation failed', severity: 'error' });
    } finally {
      setCreating(false);
    }
  };

  const formatSize = (bytes?: string | null) => {
    const n = Number(bytes ?? 0);
    if (!n) return '—';
    if (n < 1024) return `${n} B`;
    if (n < 1024 * 1024) return `${(n / 1024).toFixed(1)} KB`;
    return `${(n / (1024 * 1024)).toFixed(1)} MB`;
  };

  const statusChip = (status?: string | null) => {
    const color = status === 'Completed' ? 'success' : status === 'Failed' ? 'error' : 'warning';
    return <Chip label={status ?? 'Unknown'} size="small" color={color} />;
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Typography variant="h5" sx={{ fontWeight: 700 }}>Backup</Typography>
        <Box sx={{ display: 'flex', gap: 1 }}>
          <IconButton onClick={load}><Refresh /></IconButton>
          <Button variant="contained" startIcon={<Backup />} disabled={creating} onClick={handleCreate}>
            Create Backup
          </Button>
        </Box>
      </Box>
      <Alert severity="info" sx={{ mb: 2 }}>
        Catalog/configuration snapshots are stored as gzip-compressed JSON (categories, services, regions, cities, coupons, ads, settings).
      </Alert>
      <Paper>
        {loading ? (
          <Box sx={{ p: 4, textAlign: 'center' }}><CircularProgress /></Box>
        ) : (
          <TableContainer>
            <Table size="small">
              <TableHead>
                <TableRow>
                  <TableCell>Name</TableCell>
                  <TableCell>Status</TableCell>
                  <TableCell>Size</TableCell>
                  <TableCell>File</TableCell>
                  <TableCell>Created</TableCell>
                  <TableCell align="right">Actions</TableCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {rows.map((row) => (
                  <TableRow key={row.id}>
                    <TableCell>{row.columns.name}</TableCell>
                    <TableCell>{statusChip(row.columns.status)}</TableCell>
                    <TableCell>{formatSize(row.columns.sizeBytes)}</TableCell>
                    <TableCell sx={{ maxWidth: 220, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                      {row.columns.filePath ?? '—'}
                    </TableCell>
                    <TableCell>{row.columns.createdAt ? new Date(row.columns.createdAt).toLocaleString() : '—'}</TableCell>
                    <TableCell align="right">
                      {row.columns.status === 'Completed' && (
                        <Button size="small" startIcon={<Download />} onClick={() => adminApi.downloadBackup(row.id)}>
                          Download
                        </Button>
                      )}
                    </TableCell>
                  </TableRow>
                ))}
                {rows.length === 0 && (
                  <TableRow>
                    <TableCell colSpan={6} align="center">No backups yet.</TableCell>
                  </TableRow>
                )}
              </TableBody>
            </Table>
          </TableContainer>
        )}
      </Paper>
      {rows.some((r) => r.columns.errorMessage) && (
        <Box sx={{ mt: 2 }}>
          {rows.filter((r) => r.columns.errorMessage).map((r) => (
            <Alert key={r.id} severity="error" sx={{ mb: 1 }}>
              {r.columns.name}: {r.columns.errorMessage}
            </Alert>
          ))}
        </Box>
      )}
      {snack && (
        <Snackbar open autoHideDuration={5000} onClose={() => setSnack(null)}>
          <Alert severity={snack.severity}>{snack.message}</Alert>
        </Snackbar>
      )}
    </Box>
  );
}

export function AdminRestorePage() {
  const config = getModuleConfig('restore')!;
  const [restoring, setRestoring] = useState<string | null>(null);
  const [snack, setSnack] = useState<{ message: string; severity: 'success' | 'error' } | null>(null);

  const handleRestore = async (id: string) => {
    if (!window.confirm('Restore catalog data from this backup? This cannot be undone.')) return;
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
      <Alert severity="warning" sx={{ mb: 2 }}>
        Restore replaces catalog/configuration tables from a completed backup. User accounts and bookings are not affected.
      </Alert>
      <AdminDataTable
        config={config}
        onRowAction={(id) => handleRestore(id)}
        extraActions={
          <Typography variant="body2" color="text.secondary">
            Click a completed backup row to restore catalog data
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
  const [reports, setReports] = useState<Array<{ id: string; name: string; type: string; status: string }>>([]);
  const [fromDate, setFromDate] = useState('');
  const [toDate, setToDate] = useState('');
  const [generating, setGenerating] = useState<string | null>(null);
  const [snack, setSnack] = useState<{ message: string; severity: 'success' | 'error' } | null>(null);

  useEffect(() => {
    adminApi.getReports()
      .then(({ data }) => setReports(data.data))
      .catch(() => setSnack({ message: 'Failed to load reports', severity: 'error' }));
  }, []);

  const handleGenerate = async (reportId: string, format: 'xlsx' | 'pdf') => {
    setGenerating(`${reportId}-${format}`);
    try {
      await adminApi.generateReport(reportId, format, {
        fromDate: fromDate || undefined,
        toDate: toDate || undefined,
        page: 1,
        pageSize: 10000,
      });
      setSnack({ message: 'Report downloaded.', severity: 'success' });
    } catch {
      setSnack({ message: 'Report generation failed', severity: 'error' });
    } finally {
      setGenerating(null);
    }
  };

  return (
    <Box>
      <Typography variant="h5" sx={{ fontWeight: 700, mb: 2 }}>Reports</Typography>
      <Alert severity="info" sx={{ mb: 2 }}>
        Generate on-demand exports for payments, bookings, and users. Optional date range filters payment and booking reports.
      </Alert>
      <Paper sx={{ p: 2, mb: 2 }}>
        <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap' }}>
          <TextField type="date" label="From" size="small" value={fromDate} onChange={(e) => setFromDate(e.target.value)} slotProps={{ inputLabel: { shrink: true } }} />
          <TextField type="date" label="To" size="small" value={toDate} onChange={(e) => setToDate(e.target.value)} slotProps={{ inputLabel: { shrink: true } }} />
        </Box>
      </Paper>
      <Paper>
        <TableContainer>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>Report</TableCell>
                <TableCell>Type</TableCell>
                <TableCell>Status</TableCell>
                <TableCell align="right">Actions</TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {reports.map((report) => (
                <TableRow key={report.id}>
                  <TableCell>{report.name}</TableCell>
                  <TableCell>{report.type}</TableCell>
                  <TableCell><Chip label={report.status} size="small" color="success" /></TableCell>
                  <TableCell align="right">
                    <Button size="small" sx={{ mr: 1 }} disabled={!!generating} onClick={() => handleGenerate(report.id, 'xlsx')}>
                      {generating === `${report.id}-xlsx` ? '...' : 'Excel'}
                    </Button>
                    <Button size="small" variant="outlined" disabled={!!generating} onClick={() => handleGenerate(report.id, 'pdf')}>
                      {generating === `${report.id}-pdf` ? '...' : 'PDF'}
                    </Button>
                  </TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>
      </Paper>
      {snack && (
        <Snackbar open autoHideDuration={5000} onClose={() => setSnack(null)}>
          <Alert severity={snack.severity}>{snack.message}</Alert>
        </Snackbar>
      )}
    </Box>
  );
}
