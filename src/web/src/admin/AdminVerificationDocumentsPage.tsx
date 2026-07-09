import { useCallback, useEffect, useState } from 'react';
import {
  Alert, Box, Button, Chip, CircularProgress, Drawer, IconButton, Link, MenuItem, Paper,
  Snackbar, Stack, Table, TableBody, TableCell, TableContainer, TableHead,
  TablePagination, TableRow, TextField, Toolbar, Typography,
} from '@mui/material';
import { Check, Close, Refresh } from '@mui/icons-material';
import { adminVerificationApi } from './adminVerificationApi';
import type { VerificationDocument } from './adminVerificationApi';
import type { PagedResult } from '../services/api';
import { getApiErrorMessage } from '../utils/apiError';

const PAGE_SIZES = [10, 25, 50];

function statusColor(status: string): 'default' | 'warning' | 'success' | 'error' {
  switch (status) {
    case 'PendingReview': return 'warning';
    case 'Verified': return 'success';
    case 'Rejected': return 'error';
    default: return 'default';
  }
}

export default function AdminVerificationDocumentsPage() {
  const [data, setData] = useState<PagedResult<VerificationDocument> | null>(null);
  const [loading, setLoading] = useState(true);
  const [status, setStatus] = useState('PendingReview');
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [detail, setDetail] = useState<VerificationDocument | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [rejectReason, setRejectReason] = useState('');
  const [saving, setSaving] = useState(false);
  const [snack, setSnack] = useState<{ message: string; severity: 'success' | 'error' } | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const res = await adminVerificationApi.list({ status: status || undefined, page: page + 1, pageSize });
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
      const res = await adminVerificationApi.getById(id);
      setDetail(res.data.data);
      setRejectReason('');
      setDrawerOpen(true);
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    }
  };

  const approve = async () => {
    if (!detail) return;
    setSaving(true);
    try {
      const res = await adminVerificationApi.approve(detail.id);
      setDetail(res.data.data);
      setSnack({ message: 'Document approved.', severity: 'success' });
      load();
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    } finally {
      setSaving(false);
    }
  };

  const reject = async () => {
    if (!detail || !rejectReason.trim()) return;
    setSaving(true);
    try {
      const res = await adminVerificationApi.reject(detail.id, rejectReason.trim());
      setDetail(res.data.data);
      setSnack({ message: 'Document rejected.', severity: 'success' });
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
        <Typography variant="h5" sx={{ fontWeight: 700 }}>Verification Documents</Typography>
        <IconButton onClick={load}><Refresh /></IconButton>
      </Box>
      <Paper sx={{ mb: 2 }}>
        <Toolbar>
          <TextField
            select
            label="Status"
            size="small"
            value={status}
            onChange={(e) => { setStatus(e.target.value); setPage(0); }}
            sx={{ minWidth: 180 }}
          >
            <MenuItem value="">All</MenuItem>
            <MenuItem value="PendingReview">Pending Review</MenuItem>
            <MenuItem value="Verified">Verified</MenuItem>
            <MenuItem value="Rejected">Rejected</MenuItem>
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
                    <TableCell>User</TableCell>
                    <TableCell>Type</TableCell>
                    <TableCell>Status</TableCell>
                    <TableCell>Submitted</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {data?.items.map((row) => (
                    <TableRow key={row.id} hover sx={{ cursor: 'pointer' }} onClick={() => openDetail(row.id)}>
                      <TableCell>{row.userEmail}</TableCell>
                      <TableCell>{row.documentType}</TableCell>
                      <TableCell><Chip label={row.status} size="small" color={statusColor(row.status)} /></TableCell>
                      <TableCell>{new Date(row.createdAt).toLocaleString()}</TableCell>
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

      <Drawer anchor="right" open={drawerOpen} onClose={() => setDrawerOpen(false)}
        slotProps={{ paper: { sx: { width: { xs: '100%', sm: 420 }, p: 3 } } }}>
        {detail && (
          <Stack spacing={2}>
            <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
              <Typography variant="h6">Document Review</Typography>
              <IconButton onClick={() => setDrawerOpen(false)}><Close /></IconButton>
            </Box>
            <Typography variant="body2" color="text.secondary">{detail.userEmail} · {detail.userRole}</Typography>
            <Chip label={detail.status} color={statusColor(detail.status)} size="small" sx={{ alignSelf: 'flex-start' }} />
            <Typography><strong>Type:</strong> {detail.documentType}</Typography>
            <Typography>
              <strong>Document:</strong>{' '}
              <Link href={detail.documentUrl} target="_blank" rel="noopener noreferrer">
                View file
              </Link>
            </Typography>
            {detail.rejectionReason && (
              <Alert severity="error">{detail.rejectionReason}</Alert>
            )}
            {detail.status === 'PendingReview' && (
              <>
                <TextField
                  label="Rejection reason"
                  multiline
                  minRows={3}
                  value={rejectReason}
                  onChange={(e) => setRejectReason(e.target.value)}
                  fullWidth
                />
                <Stack direction="row" spacing={1}>
                  <Button variant="contained" color="success" startIcon={<Check />} disabled={saving} onClick={approve}>
                    Approve
                  </Button>
                  <Button variant="outlined" color="error" startIcon={<Close />} disabled={saving || !rejectReason.trim()} onClick={reject}>
                    Reject
                  </Button>
                </Stack>
              </>
            )}
          </Stack>
        )}
      </Drawer>

      {snack && (
        <Snackbar open autoHideDuration={5000} onClose={() => setSnack(null)}>
          <Alert severity={snack.severity}>{snack.message}</Alert>
        </Snackbar>
      )}
    </Box>
  );
}
