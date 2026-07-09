import { useCallback, useEffect, useState } from 'react';
import {
  Alert, Box, Chip, CircularProgress, Drawer, IconButton, MenuItem, Paper,
  Snackbar, Stack, Table, TableBody, TableCell, TableContainer, TableHead,
  TablePagination, TableRow, TextField, Toolbar, Typography,
} from '@mui/material';
import { Close, Refresh } from '@mui/icons-material';
import { adminPaymentsApi } from './adminOpsApi';
import type { AdminPaymentDetail } from './adminOpsApi';
import type { AdminListResult } from './moduleConfig';
import { getApiErrorMessage } from '../utils/apiError';

const PAGE_SIZES = [10, 25, 50];
const PAYMENT_STATUSES = ['Pending', 'Completed', 'Failed', 'Refunded', 'Cancelled'];

export default function AdminPaymentsPage() {
  const [data, setData] = useState<AdminListResult | null>(null);
  const [loading, setLoading] = useState(true);
  const [status, setStatus] = useState('');
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [detail, setDetail] = useState<AdminPaymentDetail | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [snack, setSnack] = useState<{ message: string; severity: 'success' | 'error' } | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const res = await adminPaymentsApi.list({ status: status || undefined, page: page + 1, pageSize });
      setData(res.data.data);
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    } finally {
      setLoading(false);
    }
  }, [status, page, pageSize]);

  useEffect(() => { load(); }, [load]);

  const openDetail = async (id: string) => {
    try {
      const res = await adminPaymentsApi.getById(id);
      setDetail(res.data.data);
      setDrawerOpen(true);
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    }
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
        <Typography variant="h5" sx={{ fontWeight: 700 }}>Payments</Typography>
        <IconButton onClick={load}><Refresh /></IconButton>
      </Box>
      <Paper sx={{ mb: 2 }}>
        <Toolbar>
          <TextField select label="Status" size="small" value={status} onChange={(e) => { setStatus(e.target.value); setPage(0); }} sx={{ minWidth: 180 }}>
            <MenuItem value="">All statuses</MenuItem>
            {PAYMENT_STATUSES.map((s) => <MenuItem key={s} value={s}>{s}</MenuItem>)}
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
                    <TableCell>Amount</TableCell>
                    <TableCell>Currency</TableCell>
                    <TableCell>Status</TableCell>
                    <TableCell>Method</TableCell>
                    <TableCell>Paid at</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {data?.items.map((row) => (
                    <TableRow key={row.id} hover sx={{ cursor: 'pointer' }} onClick={() => openDetail(row.id)}>
                      <TableCell>{row.columns.amount}</TableCell>
                      <TableCell>{row.columns.currency}</TableCell>
                      <TableCell><Chip label={row.columns.status} size="small" /></TableCell>
                      <TableCell>{row.columns.method}</TableCell>
                      <TableCell>{row.columns.paidAt ? new Date(row.columns.paidAt).toLocaleString() : '—'}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
            <TablePagination
              component="div"
              count={data?.totalCount ?? 0}
              page={page}
              onPageChange={(_, p) => setPage(p)}
              rowsPerPage={pageSize}
              onRowsPerPageChange={(e) => { setPageSize(parseInt(e.target.value, 10)); setPage(0); }}
              rowsPerPageOptions={PAGE_SIZES}
            />
          </>
        )}
      </Paper>

      <Drawer anchor="right" open={drawerOpen} onClose={() => setDrawerOpen(false)} slotProps={{ paper: { sx: { width: { xs: '100%', sm: 440 }, p: 3 } } }}>
        <Stack direction="row" sx={{ justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
          <Typography variant="h6">Payment detail</Typography>
          <IconButton onClick={() => setDrawerOpen(false)}><Close /></IconButton>
        </Stack>
        {detail && (
          <Stack spacing={1.5}>
            <Typography><strong>Status:</strong> <Chip label={detail.status} size="small" /></Typography>
            <Typography><strong>Amount:</strong> {detail.amount} {detail.currency}</Typography>
            <Typography><strong>Method:</strong> {detail.paymentMethod}</Typography>
            <Typography><strong>Payer:</strong> {detail.payerEmail}</Typography>
            <Typography><strong>Payee:</strong> {detail.payeeEmail}</Typography>
            <Typography><strong>Booking:</strong> {detail.bookingReference ?? detail.bookingId}</Typography>
            {detail.transactionReference && <Typography><strong>Transaction ref:</strong> {detail.transactionReference}</Typography>}
            {detail.paidAt && <Typography><strong>Paid at:</strong> {new Date(detail.paidAt).toLocaleString()}</Typography>}
            {detail.failureReason && <Alert severity="error">{detail.failureReason}</Alert>}
            <Typography variant="body2" color="text.secondary">Created {new Date(detail.createdAt).toLocaleString()}</Typography>
          </Stack>
        )}
      </Drawer>

      <Snackbar open={!!snack} autoHideDuration={5000} onClose={() => setSnack(null)}>
        {snack ? <Alert severity={snack.severity}>{snack.message}</Alert> : undefined}
      </Snackbar>
    </Box>
  );
}
