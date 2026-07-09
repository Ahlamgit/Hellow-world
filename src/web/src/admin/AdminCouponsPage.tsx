import { useCallback, useEffect, useState } from 'react';
import {
  Alert, Box, Button, Chip, Dialog, DialogActions, DialogTitle,
  Drawer, FormControlLabel, IconButton, Paper, Snackbar, Stack, Switch,
  Table, TableBody, TableCell, TableContainer, TableHead, TablePagination, TableRow,
  TextField, Toolbar, Typography,
} from '@mui/material';
import { Add, Close, Refresh } from '@mui/icons-material';
import {
  adminCouponsApi, emptyCouponForm,
  type Coupon, type CouponListQuery, type CreateCouponRequest,
} from './adminMarketingApi';
import { getApiErrorMessage } from '../utils/apiError';

const PAGE_SIZES = [10, 25, 50];

export default function AdminCouponsPage() {
  const [items, setItems] = useState<Coupon[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [searchInput, setSearchInput] = useState('');
  const [search, setSearch] = useState('');
  const [activeOnly, setActiveOnly] = useState(false);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [snack, setSnack] = useState<{ message: string; severity: 'success' | 'error' } | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [drawerMode, setDrawerMode] = useState<'create' | 'edit'>('create');
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [form, setForm] = useState<CreateCouponRequest>(emptyCouponForm());
  const [saving, setSaving] = useState(false);
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [deleteId, setDeleteId] = useState<string | null>(null);

  const buildQuery = useCallback((): CouponListQuery => ({
    search: search || undefined,
    isActive: activeOnly ? true : undefined,
    page: page + 1,
    pageSize,
  }), [search, activeOnly, page, pageSize]);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const res = await adminCouponsApi.list(buildQuery());
      setItems(res.data.data.items);
      setTotalCount(res.data.data.totalCount);
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    } finally {
      setLoading(false);
    }
  }, [buildQuery]);

  useEffect(() => { load(); }, [load]);

  const set = <K extends keyof CreateCouponRequest>(key: K, value: CreateCouponRequest[K]) =>
    setForm({ ...form, [key]: value });

  const toPayload = (f: CreateCouponRequest) => ({
    ...f,
    validFrom: new Date(f.validFrom).toISOString(),
    validTo: new Date(f.validTo).toISOString(),
  });

  const openCreate = () => {
    setDrawerMode('create');
    setSelectedId(null);
    setForm(emptyCouponForm());
    setDrawerOpen(true);
  };

  const openEdit = async (id: string) => {
    setDrawerMode('edit');
    setSelectedId(id);
    setDrawerOpen(true);
    try {
      const res = await adminCouponsApi.getById(id);
      const c = res.data.data;
      setForm({
        code: c.code,
        descriptionEn: c.descriptionEn,
        descriptionAr: c.descriptionAr,
        discountPercentage: c.discountPercentage,
        maxDiscountAmount: c.maxDiscountAmount,
        maxUses: c.maxUses,
        validFrom: c.validFrom.slice(0, 10),
        validTo: c.validTo.slice(0, 10),
        isActive: c.isActive,
      });
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
      setDrawerOpen(false);
    }
  };

  const save = async () => {
    setSaving(true);
    try {
      const payload = toPayload(form);
      if (drawerMode === 'create') await adminCouponsApi.create(payload);
      else if (selectedId) await adminCouponsApi.update(selectedId, payload);
      setSnack({ message: 'Coupon saved.', severity: 'success' });
      setDrawerOpen(false);
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
        <Typography variant="h5" sx={{ fontWeight: 700 }}>Coupons</Typography>
        <Button variant="contained" startIcon={<Add />} onClick={openCreate}>New coupon</Button>
      </Box>
      <Paper sx={{ mb: 2 }}>
        <Toolbar sx={{ gap: 2 }}>
          <TextField size="small" label="Search code" value={searchInput}
            onChange={(e) => setSearchInput(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && (setSearch(searchInput), setPage(0))} />
          <FormControlLabel control={<Switch checked={activeOnly} onChange={(e) => { setActiveOnly(e.target.checked); setPage(0); }} />} label="Active only" />
          <IconButton onClick={load}><Refresh /></IconButton>
        </Toolbar>
      </Paper>
      <TableContainer component={Paper}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Code</TableCell>
              <TableCell>Discount</TableCell>
              <TableCell>Used</TableCell>
              <TableCell>Valid to</TableCell>
              <TableCell>Status</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? <TableRow><TableCell colSpan={6} align="center">Loading…</TableCell></TableRow>
              : items.map((row) => (
                <TableRow key={row.id} hover>
                  <TableCell sx={{ fontFamily: 'monospace', fontWeight: 600 }}>{row.code}</TableCell>
                  <TableCell>{row.discountPercentage}%</TableCell>
                  <TableCell>{row.usedCount}{row.maxUses > 0 ? ` / ${row.maxUses}` : ''}</TableCell>
                  <TableCell>{new Date(row.validTo).toLocaleDateString()}</TableCell>
                  <TableCell><Chip label={row.isActive ? 'Active' : 'Inactive'} size="small" color={row.isActive ? 'success' : 'default'} /></TableCell>
                  <TableCell align="right">
                    <Button size="small" onClick={() => openEdit(row.id)}>Edit</Button>
                    <Button size="small" color="error" onClick={() => { setDeleteId(row.id); setDeleteOpen(true); }}>Delete</Button>
                  </TableCell>
                </TableRow>
              ))}
          </TableBody>
        </Table>
        <TablePagination component="div" count={totalCount} page={page} onPageChange={(_, p) => setPage(p)}
          rowsPerPage={pageSize} onRowsPerPageChange={(e) => { setPageSize(Number(e.target.value)); setPage(0); }}
          rowsPerPageOptions={PAGE_SIZES} />
      </TableContainer>

      <Drawer anchor="right" open={drawerOpen} onClose={() => setDrawerOpen(false)} slotProps={{ paper: { sx: { width: 420, p: 3 } } }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
          <Typography variant="h6" sx={{ fontWeight: 700 }}>{drawerMode === 'create' ? 'Create coupon' : 'Edit coupon'}</Typography>
          <IconButton onClick={() => setDrawerOpen(false)}><Close /></IconButton>
        </Box>
        <Stack spacing={2}>
          <TextField label="Code" value={form.code} onChange={(e) => set('code', e.target.value.toUpperCase())} required fullWidth />
          <TextField label="Description (EN)" value={form.descriptionEn} onChange={(e) => set('descriptionEn', e.target.value)} fullWidth />
          <TextField label="Description (AR)" value={form.descriptionAr} onChange={(e) => set('descriptionAr', e.target.value)} fullWidth />
          <TextField type="number" label="Discount %" value={form.discountPercentage} onChange={(e) => set('discountPercentage', Number(e.target.value))} fullWidth />
          <TextField type="number" label="Max discount (SAR)" value={form.maxDiscountAmount ?? ''} onChange={(e) => set('maxDiscountAmount', e.target.value ? Number(e.target.value) : undefined)} fullWidth />
          <TextField type="number" label="Max uses (0 = unlimited)" value={form.maxUses} onChange={(e) => set('maxUses', Number(e.target.value))} fullWidth />
          <TextField type="date" label="Valid from" value={form.validFrom} onChange={(e) => set('validFrom', e.target.value)} slotProps={{ inputLabel: { shrink: true } }} fullWidth />
          <TextField type="date" label="Valid to" value={form.validTo} onChange={(e) => set('validTo', e.target.value)} slotProps={{ inputLabel: { shrink: true } }} fullWidth />
          <FormControlLabel control={<Switch checked={form.isActive} onChange={(e) => set('isActive', e.target.checked)} />} label="Active" />
        </Stack>
        <Box sx={{ mt: 3, display: 'flex', gap: 1 }}>
          <Button variant="contained" onClick={save} disabled={saving}>Save</Button>
          <Button onClick={() => setDrawerOpen(false)}>Cancel</Button>
        </Box>
      </Drawer>

      <Dialog open={deleteOpen} onClose={() => setDeleteOpen(false)}>
        <DialogTitle>Delete coupon?</DialogTitle>
        <DialogActions>
          <Button onClick={() => setDeleteOpen(false)}>Cancel</Button>
          <Button color="error" variant="contained" onClick={async () => {
            if (!deleteId) return;
            try {
              await adminCouponsApi.delete(deleteId);
              setSnack({ message: 'Coupon deleted.', severity: 'success' });
              setDeleteOpen(false);
              load();
            } catch (e) {
              setSnack({ message: getApiErrorMessage(e), severity: 'error' });
            }
          }}>Delete</Button>
        </DialogActions>
      </Dialog>

      <Snackbar open={!!snack} autoHideDuration={5000} onClose={() => setSnack(null)}>
        {snack ? <Alert severity={snack.severity}>{snack.message}</Alert> : undefined}
      </Snackbar>
    </Box>
  );
}
