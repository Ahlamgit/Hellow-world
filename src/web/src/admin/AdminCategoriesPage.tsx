import { useCallback, useEffect, useState } from 'react';
import {
  Alert, Box, Button, Chip, Dialog, DialogActions, DialogContent, DialogTitle,
  Drawer, FormControlLabel, IconButton, Paper, Snackbar, Stack, Switch,
  Table, TableBody, TableCell, TableContainer, TableHead, TablePagination, TableRow,
  TextField, Toolbar, Typography,
} from '@mui/material';
import { Add, Close, Refresh } from '@mui/icons-material';
import {
  adminCategoriesApi, emptyCategoryForm,
  type Category, type CategoryListQuery, type CreateCategoryRequest,
} from './adminCatalogApi';
import { getApiErrorMessage } from '../utils/apiError';

const PAGE_SIZES = [10, 25, 50];

export default function AdminCategoriesPage() {
  const [items, setItems] = useState<Category[]>([]);
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
  const [form, setForm] = useState<CreateCategoryRequest>(emptyCategoryForm());
  const [saving, setSaving] = useState(false);
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [deleteId, setDeleteId] = useState<string | null>(null);

  const buildQuery = useCallback((): CategoryListQuery => ({
    search: search || undefined,
    isActive: activeOnly ? true : undefined,
    page: page + 1,
    pageSize,
  }), [search, activeOnly, page, pageSize]);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const res = await adminCategoriesApi.list(buildQuery());
      setItems(res.data.data.items);
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
    setForm(emptyCategoryForm());
    setDrawerOpen(true);
  };

  const openEdit = async (id: string) => {
    setDrawerMode('edit');
    setSelectedId(id);
    setDrawerOpen(true);
    try {
      const res = await adminCategoriesApi.getById(id);
      const c = res.data.data;
      setForm({
        nameEn: c.nameEn,
        nameAr: c.nameAr,
        descriptionEn: c.descriptionEn,
        descriptionAr: c.descriptionAr,
        iconUrl: c.iconUrl,
        displayOrder: c.displayOrder,
        isActive: c.isActive,
        parentCategoryId: c.parentCategoryId,
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
        await adminCategoriesApi.create(form);
        setSnack({ message: 'Category created.', severity: 'success' });
      } else if (selectedId) {
        await adminCategoriesApi.update(selectedId, form);
        setSnack({ message: 'Category updated.', severity: 'success' });
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
      await adminCategoriesApi.delete(deleteId);
      setSnack({ message: 'Category deleted.', severity: 'success' });
      setDeleteOpen(false);
      load();
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    }
  };

  const set = <K extends keyof CreateCategoryRequest>(key: K, value: CreateCategoryRequest[K]) =>
    setForm({ ...form, [key]: value });

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', mb: 2 }}>
        <Typography variant="h5" sx={{ fontWeight: 700 }}>Categories</Typography>
        <Button variant="contained" startIcon={<Add />} onClick={openCreate}>New category</Button>
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
              <TableCell>Name (EN)</TableCell>
              <TableCell>Name (AR)</TableCell>
              <TableCell>Order</TableCell>
              <TableCell>Services</TableCell>
              <TableCell>Status</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? (
              <TableRow><TableCell colSpan={6} align="center">Loading…</TableCell></TableRow>
            ) : items.length === 0 ? (
              <TableRow><TableCell colSpan={6} align="center">No categories found</TableCell></TableRow>
            ) : items.map((row) => (
              <TableRow key={row.id} hover>
                <TableCell>{row.nameEn}</TableCell>
                <TableCell>{row.nameAr}</TableCell>
                <TableCell>{row.displayOrder}</TableCell>
                <TableCell>{row.serviceCount}</TableCell>
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
            {drawerMode === 'create' ? 'Create category' : 'Edit category'}
          </Typography>
          <IconButton onClick={() => setDrawerOpen(false)}><Close /></IconButton>
        </Box>
        <Stack spacing={2}>
          <TextField label="Name (EN)" value={form.nameEn} onChange={(e) => set('nameEn', e.target.value)} required fullWidth />
          <TextField label="Name (AR)" value={form.nameAr} onChange={(e) => set('nameAr', e.target.value)} required fullWidth />
          <TextField label="Description (EN)" value={form.descriptionEn ?? ''} onChange={(e) => set('descriptionEn', e.target.value)} multiline rows={2} fullWidth />
          <TextField label="Description (AR)" value={form.descriptionAr ?? ''} onChange={(e) => set('descriptionAr', e.target.value)} multiline rows={2} fullWidth />
          <TextField label="Icon URL" value={form.iconUrl ?? ''} onChange={(e) => set('iconUrl', e.target.value)} fullWidth />
          <TextField type="number" label="Display order" value={form.displayOrder} onChange={(e) => set('displayOrder', Number(e.target.value))} fullWidth />
          <FormControlLabel control={<Switch checked={form.isActive} onChange={(e) => set('isActive', e.target.checked)} />} label="Active" />
        </Stack>
        <Box sx={{ display: 'flex', gap: 1, mt: 3 }}>
          <Button variant="contained" onClick={save} disabled={saving}>{saving ? 'Saving…' : 'Save'}</Button>
          <Button onClick={() => setDrawerOpen(false)}>Cancel</Button>
        </Box>
      </Drawer>

      <Dialog open={deleteOpen} onClose={() => setDeleteOpen(false)}>
        <DialogTitle>Delete category?</DialogTitle>
        <DialogContent><Typography>Soft-deletes the category. Existing services remain linked.</Typography></DialogContent>
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
