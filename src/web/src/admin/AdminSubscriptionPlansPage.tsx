import { useCallback, useEffect, useState } from 'react';
import {
  Alert, Box, Button, Checkbox, Chip, Dialog, DialogActions, DialogContent, DialogTitle,
  Divider, Drawer, FormControl, FormControlLabel, IconButton, InputLabel, MenuItem,
  Paper, Select, Snackbar, Stack, Switch, Table, TableBody, TableCell, TableContainer,
  TableHead, TablePagination, TableRow, TextField, Toolbar, Typography,
} from '@mui/material';
import { Add, Close, Delete, Refresh } from '@mui/icons-material';
import {
  adminSubscriptionPlansApi,
  BILLING_CYCLES,
  COMMON_CURRENCIES,
  emptyPlanForm,
  defaultBillingOption,
  PLAN_STATUSES,
  TARGET_ROLES,
  type AdminSubscriptionPlan,
  type CreateSubscriptionPlanRequest,
  type SubscriptionPlanListQuery,
} from './adminSubscriptionPlansApi';
import type { PlanBillingOption } from '../services/subscriptionsApi';
import { getApiErrorMessage } from '../utils/apiError';
import { useAuth } from '../context/AuthContext';
import { canCreateSubscriptionPlans, canEditSubscriptionPlans } from '../utils/permissions';

const PAGE_SIZES = [10, 25, 50];

function StatusChip({ status }: { status: string }) {
  const color = status === 'Active' ? 'success'
    : status === 'Suspended' ? 'warning'
      : status === 'Archived' ? 'default' : 'error';
  return <Chip label={status} size="small" color={color} variant={status === 'Archived' ? 'outlined' : 'filled'} />;
}

function billingSummary(options: PlanBillingOption[], currency: string) {
  if (!options.length) return '—';
  return options.map((o) => `${o.cycle}: ${o.price} ${currency}`).join(' · ');
}

interface PlanFormProps {
  form: CreateSubscriptionPlanRequest;
  onChange: (form: CreateSubscriptionPlanRequest) => void;
  isEdit?: boolean;
}

function PlanForm({ form, onChange, isEdit }: PlanFormProps) {
  const set = <K extends keyof CreateSubscriptionPlanRequest>(key: K, value: CreateSubscriptionPlanRequest[K]) =>
    onChange({ ...form, [key]: value });

  const updateBilling = (index: number, patch: Partial<PlanBillingOption>) => {
    const next = form.billingOptions.map((b, i) => (i === index ? { ...b, ...patch } : b));
    set('billingOptions', next);
  };

  const addBilling = () => {
    const used = new Set(form.billingOptions.map((b) => b.cycle));
    const nextCycle = BILLING_CYCLES.find((c) => !used.has(c)) ?? 'Monthly';
    set('billingOptions', [...form.billingOptions, defaultBillingOption(nextCycle)]);
  };

  const removeBilling = (index: number) => {
    if (form.billingOptions.length <= 1) return;
    set('billingOptions', form.billingOptions.filter((_, i) => i !== index));
  };

  return (
    <Stack spacing={2.5} sx={{ pt: 1 }}>
      <Typography variant="subtitle2" color="text.secondary">Basic information</Typography>
      <TextField
        label="Plan code"
        value={form.planCode}
        onChange={(e) => set('planCode', e.target.value.toUpperCase())}
        disabled={isEdit}
        helperText="Uppercase letters, numbers, underscores (e.g. CRAFTSMAN_PRO)"
        required
        fullWidth
      />
      <Box sx={{ display: 'flex', gap: 2 }}>
        <TextField label="Name (EN)" value={form.nameEn} onChange={(e) => set('nameEn', e.target.value)} required fullWidth />
        <TextField label="Name (AR)" value={form.nameAr} onChange={(e) => set('nameAr', e.target.value)} required fullWidth />
      </Box>
      <TextField label="Description (EN)" value={form.descriptionEn ?? ''} onChange={(e) => set('descriptionEn', e.target.value)} multiline rows={2} fullWidth />
      <TextField label="Description (AR)" value={form.descriptionAr ?? ''} onChange={(e) => set('descriptionAr', e.target.value)} multiline rows={2} fullWidth />
      <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap' }}>
        <TextField
          label="Currency"
          value={form.currency}
          onChange={(e) => set('currency', e.target.value.toUpperCase().slice(0, 3))}
          helperText="ISO 4217 code (e.g. SAR, USD, EUR)"
          inputProps={{ list: 'plan-currency-options', maxLength: 3 }}
          sx={{ minWidth: 140 }}
          required
        />
        <datalist id="plan-currency-options">
          {COMMON_CURRENCIES.map((c) => <option key={c} value={c} />)}
        </datalist>
        <FormControl sx={{ minWidth: 160 }}>
          <InputLabel>Target role</InputLabel>
          <Select label="Target role" value={form.targetRole} onChange={(e) => set('targetRole', e.target.value)}>
            {TARGET_ROLES.map((r) => <MenuItem key={r} value={r}>{r}</MenuItem>)}
          </Select>
        </FormControl>
        <FormControl sx={{ minWidth: 140 }}>
          <InputLabel>Status</InputLabel>
          <Select label="Status" value={form.status} onChange={(e) => set('status', e.target.value)}>
            {PLAN_STATUSES.map((s) => <MenuItem key={s} value={s}>{s}</MenuItem>)}
          </Select>
        </FormControl>
        <TextField type="number" label="Trial days" value={form.trialDays} onChange={(e) => set('trialDays', Number(e.target.value))} sx={{ width: 120 }} />
      </Box>

      <Divider />
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
        <Typography variant="subtitle2" color="text.secondary">Billing options</Typography>
        <Button size="small" onClick={addBilling} disabled={form.billingOptions.length >= BILLING_CYCLES.length}>Add cycle</Button>
      </Box>
      {form.billingOptions.map((opt, index) => (
        <Box key={index} sx={{ display: 'flex', gap: 1, alignItems: 'center', flexWrap: 'wrap' }}>
          <FormControl size="small" sx={{ minWidth: 130 }}>
            <InputLabel>Cycle</InputLabel>
            <Select label="Cycle" value={opt.cycle} onChange={(e) => {
              const cycle = e.target.value;
              updateBilling(index, { cycle, durationDays: defaultBillingOption(cycle).durationDays });
            }}>
              {BILLING_CYCLES.map((c) => <MenuItem key={c} value={c}>{c}</MenuItem>)}
            </Select>
          </FormControl>
          <TextField size="small" type="number" label="Price" value={opt.price} onChange={(e) => updateBilling(index, { price: Number(e.target.value) })} sx={{ width: 110 }} />
          <TextField size="small" type="number" label="Days" value={opt.durationDays} onChange={(e) => updateBilling(index, { durationDays: Number(e.target.value) })} sx={{ width: 90 }} disabled={opt.cycle === 'Lifetime'} />
          <FormControlLabel control={<Switch size="small" checked={opt.isActive} onChange={(e) => updateBilling(index, { isActive: e.target.checked })} />} label="Active" />
          <IconButton size="small" color="error" onClick={() => removeBilling(index)} disabled={form.billingOptions.length <= 1}><Delete fontSize="small" /></IconButton>
        </Box>
      ))}

      <Divider />
      <Typography variant="subtitle2" color="text.secondary">Limits & visibility</Typography>
      <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap' }}>
        <TextField type="number" label="Max services" value={form.maxServices ?? ''} onChange={(e) => set('maxServices', e.target.value ? Number(e.target.value) : undefined)} sx={{ width: 140 }} />
        <TextField type="number" label="Max categories" value={form.maxCategories ?? ''} onChange={(e) => set('maxCategories', e.target.value ? Number(e.target.value) : undefined)} sx={{ width: 140 }} />
        <TextField type="number" label="Display priority" value={form.displayPriority} onChange={(e) => set('displayPriority', Number(e.target.value))} sx={{ width: 140 }} />
        <TextField type="number" label="Grace period (days)" value={form.gracePeriodDays} onChange={(e) => set('gracePeriodDays', Number(e.target.value))} sx={{ width: 160 }} />
      </Box>
      <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 1 }}>
        {([
          ['isFeatured', 'Featured'],
          ['homePageVisible', 'Home page'],
          ['bannerVisible', 'Banner'],
          ['categoryVisible', 'Category page'],
          ['verificationBadge', 'Verification badge'],
          ['premiumBadge', 'Premium badge'],
          ['statisticsDashboard', 'Statistics'],
          ['analytics', 'Analytics'],
          ['priorityCustomerSupport', 'Priority support'],
          ['renewalReminder', 'Renewal reminder'],
          ['autoRenewal', 'Auto renewal'],
          ['expiryNotification', 'Expiry notification'],
          ['couponSupport', 'Coupon support'],
          ['paymentRequired', 'Payment required'],
        ] as const).map(([key, label]) => (
          <FormControlLabel
            key={key}
            control={<Checkbox checked={form[key]} onChange={(e) => set(key, e.target.checked)} size="small" />}
            label={label}
          />
        ))}
      </Box>
    </Stack>
  );
}

function planToForm(plan: AdminSubscriptionPlan): CreateSubscriptionPlanRequest {
  const { id: _id, createdAt: _c, updatedAt: _u, clonedFromPlanId: _cl, suspendedAt: _s, archivedAt: _a, ...rest } = plan;
  return rest;
}

export default function AdminSubscriptionPlansPage() {
  const { user } = useAuth();
  const canCreate = canCreateSubscriptionPlans(user);
  const canEdit = canEditSubscriptionPlans(user);
  const [plans, setPlans] = useState<AdminSubscriptionPlan[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [searchInput, setSearchInput] = useState('');
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [roleFilter, setRoleFilter] = useState('');
  const [includeArchived, setIncludeArchived] = useState(false);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [snack, setSnack] = useState<{ message: string; severity: 'success' | 'error' } | null>(null);

  const [drawerOpen, setDrawerOpen] = useState(false);
  const [drawerMode, setDrawerMode] = useState<'create' | 'edit'>('create');
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [form, setForm] = useState<CreateSubscriptionPlanRequest>(emptyPlanForm());
  const [saving, setSaving] = useState(false);

  const [cloneOpen, setCloneOpen] = useState(false);
  const [cloneId, setCloneId] = useState<string | null>(null);
  const [cloneCode, setCloneCode] = useState('');

  const [deleteOpen, setDeleteOpen] = useState(false);
  const [deleteId, setDeleteId] = useState<string | null>(null);

  const buildQuery = useCallback((): SubscriptionPlanListQuery => ({
    search: search || undefined,
    status: statusFilter || undefined,
    targetRole: roleFilter || undefined,
    includeArchived,
    page: page + 1,
    pageSize,
  }), [search, statusFilter, roleFilter, includeArchived, page, pageSize]);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const res = await adminSubscriptionPlansApi.list(buildQuery());
      setPlans(res.data.data.items);
      setTotalCount(res.data.data.totalCount);
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    } finally {
      setLoading(false);
    }
  }, [buildQuery]);

  useEffect(() => { load(); }, [load]);

  const openCreate = () => {
    setDrawerMode('create');
    setSelectedId(null);
    setForm(emptyPlanForm());
    setDrawerOpen(true);
  };

  const openEdit = async (id: string) => {
    setDrawerMode('edit');
    setSelectedId(id);
    setDrawerOpen(true);
    try {
      const res = await adminSubscriptionPlansApi.getById(id);
      setForm(planToForm(res.data.data));
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
      setDrawerOpen(false);
    }
  };

  const save = async () => {
    setSaving(true);
    try {
      if (drawerMode === 'create') {
        await adminSubscriptionPlansApi.create(form);
        setSnack({ message: 'Plan created successfully.', severity: 'success' });
      } else if (selectedId) {
        await adminSubscriptionPlansApi.update(selectedId, form);
        setSnack({ message: 'Plan updated successfully.', severity: 'success' });
      }
      setDrawerOpen(false);
      load();
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    } finally {
      setSaving(false);
    }
  };

  const runAction = async (id: string, action: 'activate' | 'deactivate' | 'suspend' | 'archive') => {
    try {
      const fn = adminSubscriptionPlansApi[action];
      const res = await fn(id);
      setSnack({ message: res.data.data?.message ?? res.data.message ?? 'Action completed.', severity: 'success' });
      load();
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    }
  };

  const confirmClone = async () => {
    if (!cloneId || !cloneCode.trim()) return;
    try {
      await adminSubscriptionPlansApi.clone(cloneId, { newPlanCode: cloneCode.trim().toUpperCase() });
      setSnack({ message: 'Plan cloned successfully.', severity: 'success' });
      setCloneOpen(false);
      load();
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    }
  };

  const confirmDelete = async () => {
    if (!deleteId) return;
    try {
      await adminSubscriptionPlansApi.delete(deleteId);
      setSnack({ message: 'Plan deleted.', severity: 'success' });
      setDeleteOpen(false);
      load();
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    }
  };

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Typography variant="h5" sx={{ fontWeight: 700 }}>Subscription Plans</Typography>
        {canCreate && (
          <Button variant="contained" startIcon={<Add />} onClick={openCreate}>New plan</Button>
        )}
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
          <Button size="small" variant="outlined" onClick={() => { setSearch(searchInput); setPage(0); }}>Search</Button>
          <FormControl size="small" sx={{ minWidth: 130 }}>
            <InputLabel>Status</InputLabel>
            <Select label="Status" value={statusFilter} onChange={(e) => { setStatusFilter(e.target.value); setPage(0); }}>
              <MenuItem value="">All</MenuItem>
              {PLAN_STATUSES.map((s) => <MenuItem key={s} value={s}>{s}</MenuItem>)}
            </Select>
          </FormControl>
          <FormControl size="small" sx={{ minWidth: 140 }}>
            <InputLabel>Target role</InputLabel>
            <Select label="Target role" value={roleFilter} onChange={(e) => { setRoleFilter(e.target.value); setPage(0); }}>
              <MenuItem value="">All</MenuItem>
              {TARGET_ROLES.map((r) => <MenuItem key={r} value={r}>{r}</MenuItem>)}
            </Select>
          </FormControl>
          <FormControlLabel
            control={<Switch checked={includeArchived} onChange={(e) => { setIncludeArchived(e.target.checked); setPage(0); }} />}
            label="Include archived"
          />
          <IconButton onClick={load}><Refresh /></IconButton>
        </Toolbar>
      </Paper>

      <TableContainer component={Paper}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Code</TableCell>
              <TableCell>Name</TableCell>
              <TableCell>Role</TableCell>
              <TableCell>Status</TableCell>
              <TableCell>Billing</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? (
              <TableRow><TableCell colSpan={6} align="center">Loading…</TableCell></TableRow>
            ) : plans.length === 0 ? (
              <TableRow><TableCell colSpan={6} align="center">No plans found</TableCell></TableRow>
            ) : plans.map((plan) => (
              <TableRow key={plan.id} hover>
                <TableCell sx={{ fontFamily: 'monospace', fontWeight: 600 }}>{plan.planCode}</TableCell>
                <TableCell>{plan.nameEn}</TableCell>
                <TableCell><Chip label={plan.targetRole} size="small" variant="outlined" /></TableCell>
                <TableCell><StatusChip status={plan.status} /></TableCell>
                <TableCell sx={{ maxWidth: 280, overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                  {billingSummary(plan.billingOptions, plan.currency)}
                </TableCell>
                <TableCell align="right">
                  <Stack direction="row" spacing={0.5} sx={{ justifyContent: 'flex-end', flexWrap: 'wrap' }}>
                    {canEdit && <Button size="small" onClick={() => openEdit(plan.id)}>Edit</Button>}
                    {canEdit && plan.status !== 'Active' && (
                      <Button size="small" color="success" onClick={() => runAction(plan.id, 'activate')}>Activate</Button>
                    )}
                    {canEdit && plan.status === 'Active' && (
                      <Button size="small" onClick={() => runAction(plan.id, 'deactivate')}>Deactivate</Button>
                    )}
                    {canEdit && plan.status !== 'Suspended' && plan.status !== 'Archived' && (
                      <Button size="small" color="warning" onClick={() => runAction(plan.id, 'suspend')}>Suspend</Button>
                    )}
                    {canEdit && plan.status !== 'Archived' && (
                      <Button size="small" onClick={() => runAction(plan.id, 'archive')}>Archive</Button>
                    )}
                    {canCreate && (
                      <Button size="small" onClick={() => { setCloneId(plan.id); setCloneCode(`${plan.planCode}_COPY`); setCloneOpen(true); }}>Clone</Button>
                    )}
                    {canEdit && (
                      <Button size="small" color="error" onClick={() => { setDeleteId(plan.id); setDeleteOpen(true); }}>Delete</Button>
                    )}
                  </Stack>
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
          onRowsPerPageChange={(e) => { setPageSize(Number(e.target.value)); setPage(0); }}
          rowsPerPageOptions={PAGE_SIZES}
        />
      </TableContainer>

      <Drawer anchor="right" open={drawerOpen} onClose={() => setDrawerOpen(false)} slotProps={{ paper: { sx: { width: { xs: '100%', sm: 520 }, p: 3 } } }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
          <Typography variant="h6" sx={{ fontWeight: 700 }}>
            {drawerMode === 'create' ? 'Create subscription plan' : 'Edit subscription plan'}
          </Typography>
          <IconButton onClick={() => setDrawerOpen(false)}><Close /></IconButton>
        </Box>
        <PlanForm form={form} onChange={setForm} isEdit={drawerMode === 'edit'} />
        <Box sx={{ display: 'flex', gap: 1, mt: 3 }}>
          <Button variant="contained" onClick={save} disabled={saving}>
            {saving ? 'Saving…' : drawerMode === 'create' ? 'Create plan' : 'Save changes'}
          </Button>
          <Button onClick={() => setDrawerOpen(false)}>Cancel</Button>
        </Box>
      </Drawer>

      <Dialog open={cloneOpen} onClose={() => setCloneOpen(false)}>
        <DialogTitle>Clone plan</DialogTitle>
        <DialogContent>
          <TextField
            autoFocus
            fullWidth
            label="New plan code"
            value={cloneCode}
            onChange={(e) => setCloneCode(e.target.value.toUpperCase())}
            sx={{ mt: 1 }}
            helperText="Must be unique (uppercase, numbers, underscores)"
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setCloneOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={confirmClone} disabled={!cloneCode.trim()}>Clone</Button>
        </DialogActions>
      </Dialog>

      <Dialog open={deleteOpen} onClose={() => setDeleteOpen(false)}>
        <DialogTitle>Delete plan?</DialogTitle>
        <DialogContent>
          <Typography>This soft-deletes the plan. Existing user subscriptions are preserved.</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteOpen(false)}>Cancel</Button>
          <Button color="error" variant="contained" onClick={confirmDelete}>Delete</Button>
        </DialogActions>
      </Dialog>

      <Snackbar open={!!snack} autoHideDuration={5000} onClose={() => setSnack(null)} anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}>
        {snack ? (
          <Alert severity={snack.severity} onClose={() => setSnack(null)} sx={{ width: '100%' }}>
            {snack.message}
          </Alert>
        ) : undefined}
      </Snackbar>
    </Box>
  );
}
