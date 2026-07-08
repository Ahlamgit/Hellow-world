import { useCallback, useEffect, useState } from 'react';
import {
  Alert, Box, Chip, MenuItem, Paper, Select, Snackbar, Table, TableBody, TableCell, TableContainer,
  TableHead, TablePagination, TableRow, TextField, Toolbar, Typography, FormControl, InputLabel,
} from '@mui/material';
import { Refresh } from '@mui/icons-material';
import IconButton from '@mui/material/IconButton';
import { adminUserSubscriptionsApi, type UserSubscriptionListQuery } from './adminUserSubscriptionsApi';
import type { UserSubscription } from '../services/subscriptionsApi';
import { getApiErrorMessage } from '../utils/apiError';

const STATUSES = ['', 'Trial', 'Active', 'Expired', 'Cancelled'];

function StatusChip({ status }: { status: string }) {
  const color = status === 'Active' || status === 'Trial' ? 'success'
    : status === 'Cancelled' ? 'error' : 'default';
  return <Chip label={status} size="small" color={color} />;
}

export default function AdminUserSubscriptionsPage() {
  const [items, setItems] = useState<UserSubscription[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [searchInput, setSearchInput] = useState('');
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [snack, setSnack] = useState<{ message: string; severity: 'success' | 'error' } | null>(null);

  const buildQuery = useCallback((): UserSubscriptionListQuery => ({
    search: search || undefined,
    status: statusFilter || undefined,
    page: page + 1,
    pageSize,
  }), [search, statusFilter, page, pageSize]);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const res = await adminUserSubscriptionsApi.list(buildQuery());
      setItems(res.data.data.items);
      setTotalCount(res.data.data.totalCount);
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    } finally {
      setLoading(false);
    }
  }, [buildQuery]);

  useEffect(() => { load(); }, [load]);

  return (
    <Box>
      <Typography variant="h5" sx={{ fontWeight: 700, mb: 2 }}>User Subscriptions</Typography>

      <Paper sx={{ mb: 2 }}>
        <Toolbar sx={{ gap: 2, flexWrap: 'wrap' }}>
          <TextField
            size="small"
            label="Search"
            value={searchInput}
            onChange={(e) => setSearchInput(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && (setSearch(searchInput), setPage(0))}
          />
          <FormControl size="small" sx={{ minWidth: 140 }}>
            <InputLabel>Status</InputLabel>
            <Select
              label="Status"
              value={statusFilter}
              onChange={(e) => { setStatusFilter(e.target.value); setPage(0); }}
            >
              {STATUSES.map((s) => <MenuItem key={s || 'all'} value={s}>{s || 'All'}</MenuItem>)}
            </Select>
          </FormControl>
          <IconButton onClick={load}><Refresh /></IconButton>
        </Toolbar>
      </Paper>

      <TableContainer component={Paper}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>User</TableCell>
              <TableCell>Plan</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Cycle</TableCell>
              <TableCell>Amount</TableCell>
              <TableCell>Start</TableCell>
              <TableCell>End</TableCell>
              <TableCell>Auto-renew</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? (
              <TableRow><TableCell colSpan={8}>Loading...</TableCell></TableRow>
            ) : items.length === 0 ? (
              <TableRow><TableCell colSpan={8}>No subscriptions found</TableCell></TableRow>
            ) : items.map((row) => (
              <TableRow key={row.id} hover>
                <TableCell>
                  <Typography variant="body2">{row.userName || row.userEmail}</Typography>
                  <Typography variant="caption" color="text.secondary">{row.userEmail}</Typography>
                </TableCell>
                <TableCell>
                  <Typography variant="body2">{row.planCode}</Typography>
                  <Typography variant="caption" color="text.secondary">{row.planNameEn}</Typography>
                </TableCell>
                <TableCell><StatusChip status={row.status} /></TableCell>
                <TableCell>{row.billingCycle ?? '—'}</TableCell>
                <TableCell>{row.amountPaid} {row.currency}</TableCell>
                <TableCell>{new Date(row.startDate).toLocaleDateString()}</TableCell>
                <TableCell>{row.endDate ? new Date(row.endDate).toLocaleDateString() : '—'}</TableCell>
                <TableCell>{row.autoRenew ? 'Yes' : 'No'}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
        <TablePagination
          component="div"
          count={totalCount}
          page={page}
          onPageChange={(_, p) => setPage(p)}
          rowsPerPage={pageSize}
          onRowsPerPageChange={(e) => { setPageSize(parseInt(e.target.value, 10)); setPage(0); }}
          rowsPerPageOptions={[10, 25, 50]}
        />
      </TableContainer>

      <Snackbar open={!!snack} autoHideDuration={4000} onClose={() => setSnack(null)}>
        {snack ? <Alert severity={snack.severity}>{snack.message}</Alert> : undefined}
      </Snackbar>
    </Box>
  );
}
