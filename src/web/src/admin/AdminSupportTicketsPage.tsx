import { useCallback, useEffect, useState } from 'react';
import {
  Alert, Box, Button, Chip, CircularProgress, Drawer, IconButton, MenuItem, Paper,
  Snackbar, Stack, Table, TableBody, TableCell, TableContainer, TableHead,
  TablePagination, TableRow, TextField, Toolbar, Typography,
} from '@mui/material';
import { Close, Refresh } from '@mui/icons-material';
import { adminSupportTicketsApi } from './adminOpsApi';
import type { AdminSupportTicketDetail } from './adminOpsApi';
import type { AdminListResult } from './moduleConfig';
import { getApiErrorMessage } from '../utils/apiError';

const PAGE_SIZES = [10, 25, 50];

export default function AdminSupportTicketsPage() {
  const [data, setData] = useState<AdminListResult | null>(null);
  const [loading, setLoading] = useState(true);
  const [status, setStatus] = useState('');
  const [search, setSearch] = useState('');
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [detail, setDetail] = useState<AdminSupportTicketDetail | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [saving, setSaving] = useState(false);
  const [snack, setSnack] = useState<{ message: string; severity: 'success' | 'error' } | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const res = await adminSupportTicketsApi.list({ status: status || undefined, search: search || undefined, page: page + 1, pageSize });
      setData(res.data.data);
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    } finally {
      setLoading(false);
    }
  }, [status, search, page, pageSize]);

  useEffect(() => { load(); }, [load]);

  const openDetail = async (id: string) => {
    try {
      const res = await adminSupportTicketsApi.getById(id);
      setDetail(res.data.data);
      setDrawerOpen(true);
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    }
  };

  const closeTicket = async () => {
    if (!detail) return;
    setSaving(true);
    try {
      const res = await adminSupportTicketsApi.close(detail.id);
      setDetail(res.data.data);
      setSnack({ message: 'Ticket closed.', severity: 'success' });
      load();
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    } finally {
      setSaving(false);
    }
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
        <Typography variant="h5" sx={{ fontWeight: 700 }}>Support tickets</Typography>
        <IconButton onClick={load}><Refresh /></IconButton>
      </Box>
      <Paper sx={{ mb: 2 }}>
        <Toolbar sx={{ gap: 2 }}>
          <TextField label="Search" size="small" value={search} onChange={(e) => setSearch(e.target.value)} onKeyDown={(e) => e.key === 'Enter' && load()} />
          <TextField select label="Status" size="small" value={status} onChange={(e) => { setStatus(e.target.value); setPage(0); }} sx={{ minWidth: 140 }}>
            <MenuItem value="">All</MenuItem>
            <MenuItem value="Open">Open</MenuItem>
            <MenuItem value="Closed">Closed</MenuItem>
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
                    <TableCell>Ticket #</TableCell>
                    <TableCell>Subject</TableCell>
                    <TableCell>Status</TableCell>
                    <TableCell>Priority</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {data?.items.map((row) => (
                    <TableRow key={row.id} hover sx={{ cursor: 'pointer' }} onClick={() => openDetail(row.id)}>
                      <TableCell>{row.columns.ticketNumber}</TableCell>
                      <TableCell>{row.columns.subject}</TableCell>
                      <TableCell><Chip label={row.columns.status} size="small" /></TableCell>
                      <TableCell>{row.columns.priority}</TableCell>
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
          <Typography variant="h6">Support ticket</Typography>
          <IconButton onClick={() => setDrawerOpen(false)}><Close /></IconButton>
        </Stack>
        {detail && (
          <Stack spacing={1.5}>
            <Typography><strong>Ticket:</strong> {detail.ticketNumber}</Typography>
            <Typography><strong>User:</strong> {detail.userEmail}</Typography>
            <Typography><strong>Category:</strong> {detail.category}</Typography>
            <Typography><strong>Status:</strong> <Chip label={detail.status} size="small" /></Typography>
            <Typography><strong>Priority:</strong> {detail.priority}</Typography>
            <Typography><strong>Subject:</strong> {detail.subject}</Typography>
            <Typography sx={{ whiteSpace: 'pre-wrap' }}>{detail.description}</Typography>
            {detail.status !== 'Closed' && (
              <Button variant="contained" color="warning" onClick={closeTicket} disabled={saving}>
                Close ticket
              </Button>
            )}
            {detail.closedAt && <Typography variant="body2" color="text.secondary">Closed {new Date(detail.closedAt).toLocaleString()}</Typography>}
          </Stack>
        )}
      </Drawer>

      <Snackbar open={!!snack} autoHideDuration={5000} onClose={() => setSnack(null)}>
        {snack ? <Alert severity={snack.severity}>{snack.message}</Alert> : undefined}
      </Snackbar>
    </Box>
  );
}
