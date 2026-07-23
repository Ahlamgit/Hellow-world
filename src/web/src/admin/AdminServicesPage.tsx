import { useCallback, useEffect, useState } from 'react';
import {
  Alert, Box, Button, Chip, Dialog, DialogActions, DialogContent, DialogTitle,
  Drawer, FormControl, FormControlLabel, IconButton, InputLabel, MenuItem,
  Paper, Select, Snackbar, Stack, Switch, Table, TableBody, TableCell,
  TableContainer, TableHead, TablePagination, TableRow, TextField, Toolbar, Typography,
} from '@mui/material';
import { Add, Close, Refresh } from '@mui/icons-material';
import {
  adminCategoriesApi, adminServicesApi, emptyServiceForm,
  type Category, type CreateServiceRequest, type Service, type ServiceListQuery,
} from './adminCatalogApi';
import { DEFAULT_CURRENCY } from '../config/platform';
import { getApiErrorMessage } from '../utils/apiError';

const PAGE_SIZES = [10, 25, 50];

export default function AdminServicesPage() {
  const [items, setItems] = useState<Service[]>([]);
  const [categories, setCategories] = useState<Category[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [searchInput, setSearchInput] = useState('');
  const [search, setSearch] = useState('');
  const [categoryFilter, setCategoryFilter] = useState('');
  const [activeOnly, setActiveOnly] = useState(false);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [snack, setSnack] = useState<{ message: string; severity: 'success' | 'error' } | null>(null);

  const [drawerOpen, setDrawerOpen] = useState(false);
  const [drawerMode, setDrawerMode] = useState<'create' | 'edit'>('create');
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [form, setForm] = useState<CreateServiceRequest>(emptyServiceForm());
  const [saving, setSaving] = useState(false);
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [deleteId, setDeleteId] = useState<string | null>(null);

  const loadCategories = useCallback(async () => {
    try {
      const res = await adminCategoriesApi.list({ pageSize: 200 });
      setCategories(res.data.data.items);
    } catch { /* non-fatal */ }
  }, []);

  const buildQuery = useCallback((): ServiceListQuery => ({
    search: search || undefined,
    categoryId: categoryFilter || undefined,
    isActive: activeOnly ? true : undefined,
    page: page + 1,
    pageSize,
  }), [search, categoryFilter, activeOnly, page, pageSize]);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const res = await adminServicesApi.list(buildQuery());
      setItems(res.data.data.items);
      setTotalCount(res.data.data.totalCount);
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    } finally {
      setLoading(false);
    }
  }, [buildQuery]);

  useEffect(() => { loadCategories(); }, [loadCategories]);
  useEffect(() => { load(); }, [load]);

  const openCreate = () => {
    setDrawerMode('create');
    setSelectedId(null);
    setForm({ ...emptyServiceForm(), categoryId: categories[0]?.id ?? '' });
    setDrawerOpen(true);
  };

  const openEdit = async (id: string) => {
    setDrawerMode('edit');
    setSelectedId(id);
    setDrawerOpen(true);
    try {
      const res = await adminServicesApi.getById(id);
      const s = res.data.data;
      setForm({
        categoryId: s.categoryId,
        nameEn: s.nameEn,
        nameAr: s.nameAr,
        descriptionEn: s.descriptionEn,
        descriptionAr: s.descriptionAr,
        basePrice: s.basePrice,
        imageUrl: s.imageUrl,
        isActive: s.isActive,
        estimatedDurationMinutes: s.estimatedDurationMinutes,
      });
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
      setDrawerOpen(false);
    }
  };

  const save = async () => {
    setSaving(true);
    try {
      if (drawerMode === 'create') {
        await adminServicesApi.create(form);
        setSnack({ message: 'Service created.', severity: 'success' });
      } else if (selectedId) {
        await adminServicesApi.update(selectedId, form);
        setSnack({ message: 'Service updated.', severity: 'success' });
      }
      setDrawerOpen(false);
      load();
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    } finally {
      setSaving(false);
    }
  };

  const confirmDelete = async () => {
    if (!deleteId) return;
    try {
      await adminServicesApi.delete(deleteId);
      setSnack({ message: 'Service deleted.', severity: 'success' });
      setDeleteOpen(false);
      load();
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    }
  };

  const set = <K extends keyof CreateServiceRequest>(key: K, value: CreateServiceRequest[K]) =>
    setForm({ ...form, [key]: value });

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Typography variant="h5" sx={{ fontWeight: 700 }}>Services</Typography>
        <Button variant="contained" startIcon={<Add />} onClick={openCreate} disabled={!categories.length}>New service</Button>
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
          <FormControl size="small" sx={{ minWidth: 180 }}>
            <InputLabel>Category</InputLabel>
            <Select label="Category" value={categoryFilter} onChange={(e) => { setCategoryFilter(e.target.value); setPage(0); }}>
              <MenuItem value="">All</MenuItem>
              {categories.map((c) => <MenuItem key={c.id} value={c.id}>{c.nameEn}</MenuItem>)}
            </Select>
          </FormControl>
          <FormControlLabel
            control={<Switch checked={activeOnly} onChange={(e) => { setActiveOnly(e.target.checked); setPage(0); }} />}
            label="Active only"
          />
          <IconButton onClick={load}><Refresh /></IconButton>
        </Toolbar>
      </Paper>

      <TableContainer component={Paper}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Name</TableCell>
              <TableCell>Category</TableCell>
              <TableCell>Price ({DEFAULT_CURRENCY})</TableCell>
              <TableCell>Duration (min)</TableCell>
              <TableCell>Status</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? (
              <TableRow><TableCell colSpan={6} align="center">Loading…</TableCell></TableRow>
            ) : items.length === 0 ? (
              <TableRow><TableCell colSpan={6} align="center">No services found</TableCell></TableRow>
            ) : items.map((row) => (
              <TableRow key={row.id} hover>
                <TableCell>{row.nameEn}</TableCell>
                <TableCell>{row.categoryName}</TableCell>
                <TableCell>{row.basePrice.toFixed(2)}</TableCell>
                <TableCell>{row.estimatedDurationMinutes}</TableCell>
                <TableCell>
                  <Chip label={row.isActive ? 'Active' : 'Inactive'} size="small" color={row.isActive ? 'success' : 'default'} />
                </TableCell>
                <TableCell align="right">
                  <Stack direction="row" spacing={0.5} sx={{ justifyContent: 'flex-end' }}>
                    <Button size="small" onClick={() => openEdit(row.id)}>Edit</Button>
                    <Button size="small" color="error" onClick={() => { setDeleteId(row.id); setDeleteOpen(true); }}>Delete</Button>
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

      <Drawer anchor="right" open={drawerOpen} onClose={() => setDrawerOpen(false)} slotProps={{ paper: { sx: { width: { xs: '100%', sm: 420 }, p: 3 } } }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
          <Typography variant="h6" sx={{ fontWeight: 700 }}>
            {drawerMode === 'create' ? 'Create service' : 'Edit service'}
          </Typography>
          <IconButton onClick={() => setDrawerOpen(false)}><Close /></IconButton>
        </Box>
        <Stack spacing={2}>
          <FormControl fullWidth required>
            <InputLabel>Category</InputLabel>
            <Select label="Category" value={form.categoryId} onChange={(e) => set('categoryId', e.target.value)}>
              {categories.map((c) => <MenuItem key={c.id} value={c.id}>{c.nameEn}</MenuItem>)}
            </Select>
          </FormControl>
          <TextField label="Name (EN)" value={form.nameEn} onChange={(e) => set('nameEn', e.target.value)} required fullWidth />
          <TextField label="Name (AR)" value={form.nameAr} onChange={(e) => set('nameAr', e.target.value)} required fullWidth />
          <TextField type="number" label={`Base price (${DEFAULT_CURRENCY})`} value={form.basePrice} onChange={(e) => set('basePrice', Number(e.target.value))} fullWidth />
          <TextField type="number" label="Duration (minutes)" value={form.estimatedDurationMinutes} onChange={(e) => set('estimatedDurationMinutes', Number(e.target.value))} fullWidth />
          <TextField label="Image URL" value={form.imageUrl ?? ''} onChange={(e) => set('imageUrl', e.target.value)} fullWidth />
          <FormControlLabel control={<Switch checked={form.isActive} onChange={(e) => set('isActive', e.target.checked)} />} label="Active" />
        </Stack>
        <Box sx={{ display: 'flex', gap: 1, mt: 3 }}>
          <Button variant="contained" onClick={save} disabled={saving || !form.categoryId}>{saving ? 'Saving…' : 'Save'}</Button>
          <Button onClick={() => setDrawerOpen(false)}>Cancel</Button>
        </Box>
      </Drawer>

      <Dialog open={deleteOpen} onClose={() => setDeleteOpen(false)}>
        <DialogTitle>Delete service?</DialogTitle>
        <DialogContent><Typography>Soft-deletes the service.</Typography></DialogContent>
        <DialogActions>
          <Button onClick={() => setDeleteOpen(false)}>Cancel</Button>
          <Button color="error" variant="contained" onClick={confirmDelete}>Delete</Button>
        </DialogActions>
      </Dialog>

      <Snackbar open={!!snack} autoHideDuration={5000} onClose={() => setSnack(null)} anchorOrigin={{ vertical: 'bottom', horizontal: 'center' }}>
        {snack ? <Alert severity={snack.severity} onClose={() => setSnack(null)}>{snack.message}</Alert> : undefined}
      </Snackbar>
    </Box>
  );
}
