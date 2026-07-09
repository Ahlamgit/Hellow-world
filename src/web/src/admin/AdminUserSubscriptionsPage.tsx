import { useCallback, useEffect, useState } from 'react';
import {
  Alert, Box, Button, Chip, Dialog, DialogActions, DialogContent, DialogTitle,
  FormControl, IconButton, InputLabel, MenuItem, Paper, Select, Snackbar,
  FormControlLabel, Checkbox,
  Table, TableBody, TableCell, TableContainer, TableHead, TablePagination, TableRow,
  TextField, Toolbar, Typography,
} from '@mui/material';
import { Add, Cancel, Refresh } from '@mui/icons-material';
import { adminUserSubscriptionsApi, type UserSubscriptionListQuery } from './adminUserSubscriptionsApi';
import { adminSubscriptionPlansApi, type AdminSubscriptionPlan } from './adminSubscriptionPlansApi';
import { adminUsersApi, type AdminUserListItem } from './adminUsersApi';
import type { UserSubscription } from '../services/subscriptionsApi';
import type { PlanBillingOption } from '../services/subscriptionsApi';
import { getApiErrorMessage } from '../utils/apiError';

const STATUSES = ['', 'Trial', 'Active', 'Expired', 'Cancelled'];
const CANCELLABLE = new Set(['Trial', 'Active']);

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

  const [grantOpen, setGrantOpen] = useState(false);
  const [plans, setPlans] = useState<AdminSubscriptionPlan[]>([]);
  const [userSearch, setUserSearch] = useState('');
  const [userResults, setUserResults] = useState<AdminUserListItem[]>([]);
  const [selectedUserId, setSelectedUserId] = useState('');
  const [selectedPlanId, setSelectedPlanId] = useState('');
  const [selectedBillingId, setSelectedBillingId] = useState('');
  const [autoRenew, setAutoRenew] = useState(true);
  const [grantNotes, setGrantNotes] = useState('');
  const [grantLoading, setGrantLoading] = useState(false);

  const [cancelOpen, setCancelOpen] = useState(false);
  const [cancelTarget, setCancelTarget] = useState<UserSubscription | null>(null);
  const [cancelReason, setCancelReason] = useState('');

  const selectedPlan = plans.find((p) => p.id === selectedPlanId);
  const billingOptions: PlanBillingOption[] = selectedPlan?.billingOptions ?? [];

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

  const openGrant = async () => {
    setGrantOpen(true);
    try {
      const res = await adminSubscriptionPlansApi.list({ status: 'Active', pageSize: 100 });
      setPlans(res.data.data.items);
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    }
  };

  const searchUsers = async () => {
    if (!userSearch.trim()) return;
    try {
      const res = await adminUsersApi.list({ search: userSearch.trim(), pageSize: 10 });
      setUserResults(res.data.data.items);
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    }
  };

  const submitGrant = async () => {
    if (!selectedUserId || !selectedPlanId || !selectedBillingId) return;
    setGrantLoading(true);
    try {
      await adminUserSubscriptionsApi.grant(selectedUserId, {
        planId: selectedPlanId,
        billingOptionId: selectedBillingId,
        autoRenew,
        notes: grantNotes.trim() || undefined,
      });
      setSnack({ message: 'Subscription granted', severity: 'success' });
      setGrantOpen(false);
      setSelectedUserId('');
      setUserSearch('');
      setUserResults([]);
      setGrantNotes('');
      await load();
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    } finally {
      setGrantLoading(false);
    }
  };

  const submitCancel = async () => {
    if (!cancelTarget) return;
    try {
      await adminUserSubscriptionsApi.cancel(cancelTarget.id, cancelReason.trim() || undefined);
      setSnack({ message: 'Subscription cancelled', severity: 'success' });
      setCancelOpen(false);
      setCancelTarget(null);
      setCancelReason('');
      await load();
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    }
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Typography variant="h5" sx={{ fontWeight: 700 }}>User Subscriptions</Typography>
        <Button variant="contained" startIcon={<Add />} onClick={openGrant}>Grant subscription</Button>
      </Box>

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
              <TableCell width={100} />
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? (
              <TableRow><TableCell colSpan={9}>Loading...</TableCell></TableRow>
            ) : items.length === 0 ? (
              <TableRow><TableCell colSpan={9}>No subscriptions found</TableCell></TableRow>
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
                <TableCell>
                  {CANCELLABLE.has(row.status) && (
                    <IconButton
                      size="small"
                      color="error"
                      title="Cancel subscription"
                      onClick={() => { setCancelTarget(row); setCancelOpen(true); }}
                    >
                      <Cancel fontSize="small" />
                    </IconButton>
                  )}
                </TableCell>
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

      <Dialog open={grantOpen} onClose={() => setGrantOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Grant subscription</DialogTitle>
        <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 1 }}>
          <Box sx={{ display: 'flex', gap: 1 }}>
            <TextField
              label="Find user (email or name)"
              value={userSearch}
              onChange={(e) => setUserSearch(e.target.value)}
              fullWidth
              onKeyDown={(e) => e.key === 'Enter' && searchUsers()}
            />
            <Button onClick={searchUsers}>Search</Button>
          </Box>
          <FormControl fullWidth>
            <InputLabel>User</InputLabel>
            <Select
              label="User"
              value={selectedUserId}
              onChange={(e) => setSelectedUserId(e.target.value)}
            >
              {userResults.map((u) => (
                <MenuItem key={u.id} value={u.id}>{u.email} — {u.firstName} {u.lastName}</MenuItem>
              ))}
            </Select>
          </FormControl>
          <FormControl fullWidth>
            <InputLabel>Plan</InputLabel>
            <Select
              label="Plan"
              value={selectedPlanId}
              onChange={(e) => { setSelectedPlanId(e.target.value); setSelectedBillingId(''); }}
            >
              {plans.map((p) => (
                <MenuItem key={p.id} value={p.id}>{p.planCode} — {p.nameEn}</MenuItem>
              ))}
            </Select>
          </FormControl>
          <FormControl fullWidth disabled={!selectedPlanId}>
            <InputLabel>Billing option</InputLabel>
            <Select
              label="Billing option"
              value={selectedBillingId}
              onChange={(e) => setSelectedBillingId(e.target.value)}
            >
              {billingOptions.map((b, index) => (
                <MenuItem key={b.id ?? `${b.cycle}-${index}`} value={b.id ?? ''}>
                  {b.cycle} — {b.price} SAR
                </MenuItem>
              ))}
            </Select>
          </FormControl>
          <FormControlLabel
            control={<Checkbox checked={autoRenew} onChange={(e) => setAutoRenew(e.target.checked)} />}
            label="Auto-renew"
          />
          <TextField
            value={grantNotes}
            onChange={(e) => setGrantNotes(e.target.value)}
            multiline
            minRows={2}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setGrantOpen(false)}>Close</Button>
          <Button
            variant="contained"
            disabled={grantLoading || !selectedUserId || !selectedPlanId || !selectedBillingId}
            onClick={submitGrant}
          >
            Grant
          </Button>
        </DialogActions>
      </Dialog>

      <Dialog open={cancelOpen} onClose={() => setCancelOpen(false)} maxWidth="xs" fullWidth>
        <DialogTitle>Cancel subscription</DialogTitle>
        <DialogContent>
          <Typography sx={{ mb: 2 }}>
            Cancel subscription for {cancelTarget?.userEmail} ({cancelTarget?.planCode})?
          </Typography>
          <TextField
            label="Reason (optional)"
            value={cancelReason}
            onChange={(e) => setCancelReason(e.target.value)}
            fullWidth
            multiline
            minRows={2}
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setCancelOpen(false)}>Keep</Button>
          <Button color="error" variant="contained" onClick={submitCancel}>Cancel subscription</Button>
        </DialogActions>
      </Dialog>

      <Snackbar open={!!snack} autoHideDuration={4000} onClose={() => setSnack(null)}>
        {snack ? <Alert severity={snack.severity}>{snack.message}</Alert> : undefined}
      </Snackbar>
    </Box>
  );
}
