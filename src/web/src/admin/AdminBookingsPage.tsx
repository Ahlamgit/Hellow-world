import { useCallback, useEffect, useState } from 'react';
import {
  Alert, Box, Chip, CircularProgress, Drawer, Grid, IconButton, MenuItem, Paper,
  Snackbar, Stack, Table, TableBody, TableCell, TableContainer, TableHead,
  TablePagination, TableRow, TextField, Toolbar, Typography,
} from '@mui/material';
import { Close, Refresh } from '@mui/icons-material';
import { adminBookingsApi, BOOKING_STATUSES } from './adminOpsApi';
import type { AdminBookingStats } from './adminOpsApi';
import type { Booking } from '../services/api';
import { getApiErrorMessage } from '../utils/apiError';

const PAGE_SIZES = [10, 25, 50];

function StatCard({ label, value }: { label: string; value: number }) {
  return (
    <Paper sx={{ p: 2, border: 1, borderColor: 'divider' }}>
      <Typography variant="body2" color="text.secondary">{label}</Typography>
      <Typography variant="h5" sx={{ fontWeight: 700 }}>{value}</Typography>
    </Paper>
  );
}

export default function AdminBookingsPage() {
  const [items, setItems] = useState<Booking[]>([]);
  const [stats, setStats] = useState<AdminBookingStats | null>(null);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [status, setStatus] = useState('');
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [selected, setSelected] = useState<Booking | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [snack, setSnack] = useState<{ message: string; severity: 'success' | 'error' } | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const [listRes, statsRes] = await Promise.all([
        adminBookingsApi.list({ status: status || undefined, page: page + 1, pageSize }),
        adminBookingsApi.stats(),
      ]);
      setItems(listRes.data.data.items);
      setTotalCount(listRes.data.data.totalCount);
      setStats(statsRes.data.data);
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    } finally {
      setLoading(false);
    }
  }, [status, page, pageSize]);

  useEffect(() => { load(); }, [load]);

  const openDetail = async (id: string) => {
    try {
      const res = await adminBookingsApi.getById(id);
      setSelected(res.data.data);
      setDrawerOpen(true);
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    }
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
        <Typography variant="h5" sx={{ fontWeight: 700 }}>Bookings</Typography>
        <IconButton onClick={load}><Refresh /></IconButton>
      </Box>

      {stats && (
        <Grid container spacing={2} sx={{ mb: 2 }}>
          <Grid size={{ xs: 6, md: 2 }}><StatCard label="Total" value={stats.totalBookings} /></Grid>
          <Grid size={{ xs: 6, md: 2 }}><StatCard label="Awaiting payment" value={stats.pendingPayment} /></Grid>
          <Grid size={{ xs: 6, md: 2 }}><StatCard label="Pending craftsman" value={stats.awaitingConfirmation} /></Grid>
          <Grid size={{ xs: 6, md: 2 }}><StatCard label="Confirmed" value={stats.confirmed} /></Grid>
          <Grid size={{ xs: 6, md: 2 }}><StatCard label="Completed" value={stats.completed} /></Grid>
          <Grid size={{ xs: 6, md: 2 }}><StatCard label="Cancelled" value={stats.cancelled} /></Grid>
        </Grid>
      )}

      <Paper sx={{ mb: 2 }}>
        <Toolbar>
          <TextField select label="Status" size="small" value={status} onChange={(e) => { setStatus(e.target.value); setPage(0); }} sx={{ minWidth: 220 }}>
            <MenuItem value="">All statuses</MenuItem>
            {BOOKING_STATUSES.map((s) => <MenuItem key={s} value={s}>{s}</MenuItem>)}
          </TextField>
        </Toolbar>
      </Paper>

      <Paper>
        {loading ? <Box sx={{ p: 4, textAlign: 'center' }}><CircularProgress /></Box> : (
          <>
            <TableContainer>
              <Table size="small">
                <TableHead>
                  <TableRow>
                    <TableCell>Reference</TableCell>
                    <TableCell>Service</TableCell>
                    <TableCell>Customer</TableCell>
                    <TableCell>Craftsman</TableCell>
                    <TableCell>Status</TableCell>
                    <TableCell>Scheduled</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {items.map((b) => (
                    <TableRow key={b.id} hover sx={{ cursor: 'pointer' }} onClick={() => openDetail(b.id)}>
                      <TableCell>{b.bookingReference}</TableCell>
                      <TableCell>{b.serviceName}</TableCell>
                      <TableCell>{b.customerName}</TableCell>
                      <TableCell>{b.craftsmanName}</TableCell>
                      <TableCell><Chip label={b.status} size="small" /></TableCell>
                      <TableCell>{new Date(b.scheduledAt).toLocaleString()}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
            <TablePagination
              component="div"
              count={totalCount}
              page={page}
              onPageChange={(_, p) => setPage(p)}
              rowsPerPage={pageSize}
              onRowsPerPageChange={(e) => { setPageSize(parseInt(e.target.value, 10)); setPage(0); }}
              rowsPerPageOptions={PAGE_SIZES}
            />
          </>
        )}
      </Paper>

      <Drawer anchor="right" open={drawerOpen} onClose={() => setDrawerOpen(false)} slotProps={{ paper: { sx: { width: { xs: '100%', sm: 480 }, p: 3 } } }}>
        <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
          <Typography variant="h6">Booking detail</Typography>
          <IconButton onClick={() => setDrawerOpen(false)}><Close /></IconButton>
        </Stack>
        {selected && (
          <Stack spacing={1.5}>
            <Typography><strong>Reference:</strong> {selected.bookingReference}</Typography>
            <Typography><strong>Status:</strong> <Chip label={selected.status} size="small" /></Typography>
            <Typography><strong>Service:</strong> {selected.serviceName}</Typography>
            <Typography><strong>Customer:</strong> {selected.customerName}</Typography>
            <Typography><strong>Craftsman:</strong> {selected.craftsmanName}</Typography>
            <Typography><strong>Scheduled:</strong> {new Date(selected.scheduledAt).toLocaleString()}</Typography>
            <Typography><strong>Price:</strong> {selected.finalPrice ?? selected.estimatedPrice} SAR</Typography>
            {selected.description && <Typography><strong>Description:</strong> {selected.description}</Typography>}
            {selected.payment && (
              <Box sx={{ mt: 1, p: 2, bgcolor: 'action.hover', borderRadius: 1 }}>
                <Typography variant="subtitle2">Payment</Typography>
                <Typography variant="body2">{selected.payment.status} — {selected.payment.amount} {selected.payment.currency}</Typography>
                {selected.payment.transactionReference && <Typography variant="body2">Ref: {selected.payment.transactionReference}</Typography>}
              </Box>
            )}
            {selected.statusHistory.length > 0 && (
              <Box>
                <Typography variant="subtitle2" sx={{ mb: 1 }}>Status history</Typography>
                {selected.statusHistory.map((h, i) => (
                  <Typography key={i} variant="body2" color="text.secondary">
                    {new Date(h.createdAt).toLocaleString()} — {h.oldStatus ?? '—'} → {h.newStatus}
                  </Typography>
                ))}
              </Box>
            )}
          </Stack>
        )}
      </Drawer>

      <Snackbar open={!!snack} autoHideDuration={5000} onClose={() => setSnack(null)}>
        {snack ? <Alert severity={snack.severity}>{snack.message}</Alert> : undefined}
      </Snackbar>
    </Box>
  );
}
