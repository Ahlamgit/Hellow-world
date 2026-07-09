import { useCallback, useEffect, useState } from 'react';
import {
  Alert, Box, Button, Chip, Dialog, DialogActions, DialogTitle,
  Drawer, FormControl, FormControlLabel, IconButton, InputLabel, MenuItem,
  Paper, Select, Snackbar, Stack, Switch, Table, TableBody, TableCell,
  TableContainer, TableHead, TablePagination, TableRow, TextField, Toolbar, Typography,
} from '@mui/material';
import { Add, Close, Refresh } from '@mui/icons-material';
import {
  AD_PLACEMENTS, adminAdvertisementsApi, emptyAdvertisementForm,
  type Advertisement, type AdvertisementListQuery, type CreateAdvertisementRequest,
} from './adminMarketingApi';
import { getApiErrorMessage } from '../utils/apiError';

const PAGE_SIZES = [10, 25, 50];

export default function AdminAdvertisementsPage() {
  const [items, setItems] = useState<Advertisement[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [searchInput, setSearchInput] = useState('');
  const [search, setSearch] = useState('');
  const [placementFilter, setPlacementFilter] = useState('');
  const [activeOnly, setActiveOnly] = useState(false);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [snack, setSnack] = useState<{ message: string; severity: 'success' | 'error' } | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [drawerMode, setDrawerMode] = useState<'create' | 'edit'>('create');
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [form, setForm] = useState<CreateAdvertisementRequest>(emptyAdvertisementForm());
  const [saving, setSaving] = useState(false);
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [deleteId, setDeleteId] = useState<string | null>(null);

  const buildQuery = useCallback((): AdvertisementListQuery => ({
    search: search || undefined,
    placement: placementFilter || undefined,
    isActive: activeOnly ? true : undefined,
    page: page + 1,
    pageSize,
  }), [search, placementFilter, activeOnly, page, pageSize]);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const res = await adminAdvertisementsApi.list(buildQuery());
      setItems(res.data.data.items);
      setTotalCount(res.data.data.totalCount);
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    } finally {
      setLoading(false);
    }
  }, [buildQuery]);

  useEffect(() => { load(); }, [load]);

  const set = <K extends keyof CreateAdvertisementRequest>(key: K, value: CreateAdvertisementRequest[K]) =>
    setForm({ ...form, [key]: value });

  const toPayload = (f: CreateAdvertisementRequest) => ({
    ...f,
    startDate: new Date(f.startDate).toISOString(),
    endDate: f.endDate ? new Date(f.endDate).toISOString() : undefined,
  });

  const openCreate = () => {
    setDrawerMode('create');
    setSelectedId(null);
    setForm(emptyAdvertisementForm());
    setDrawerOpen(true);
  };

  const openEdit = async (id: string) => {
    setDrawerMode('edit');
    setSelectedId(id);
    setDrawerOpen(true);
    try {
      const res = await adminAdvertisementsApi.getById(id);
      const a = res.data.data;
      setForm({
        titleEn: a.titleEn,
        titleAr: a.titleAr,
        descriptionEn: a.descriptionEn,
        imageUrl: a.imageUrl,
        placement: a.placement,
        targetUserId: a.targetUserId,
        startDate: a.startDate.slice(0, 10),
        endDate: a.endDate?.slice(0, 10),
        isActive: a.isActive,
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
      if (drawerMode === 'create') await adminAdvertisementsApi.create(payload);
      else if (selectedId) await adminAdvertisementsApi.update(selectedId, payload);
      setSnack({ message: 'Advertisement saved.', severity: 'success' });
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
        <Typography variant="h5" sx={{ fontWeight: 700 }}>Advertisements</Typography>
        <Button variant="contained" startIcon={<Add />} onClick={openCreate}>New ad</Button>
      </Box>
      <Paper sx={{ mb: 2 }}>
        <Toolbar sx={{ gap: 2, flexWrap: 'wrap' }}>
          <TextField size="small" label="Search" value={searchInput}
            onChange={(e) => setSearchInput(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && (setSearch(searchInput), setPage(0))} />
          <FormControl size="small" sx={{ minWidth: 140 }}>
            <InputLabel>Placement</InputLabel>
            <Select label="Placement" value={placementFilter} onChange={(e) => { setPlacementFilter(e.target.value); setPage(0); }}>
              <MenuItem value="">All</MenuItem>
              {AD_PLACEMENTS.map((p) => <MenuItem key={p} value={p}>{p}</MenuItem>)}
            </Select>
          </FormControl>
          <FormControlLabel control={<Switch checked={activeOnly} onChange={(e) => { setActiveOnly(e.target.checked); setPage(0); }} />} label="Active only" />
          <IconButton onClick={load}><Refresh /></IconButton>
        </Toolbar>
      </Paper>
      <TableContainer component={Paper}>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>Title</TableCell>
              <TableCell>Placement</TableCell>
              <TableCell>Start</TableCell>
              <TableCell>Stats</TableCell>
              <TableCell>Status</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? <TableRow><TableCell colSpan={6} align="center">Loading…</TableCell></TableRow>
              : items.map((row) => (
                <TableRow key={row.id} hover>
                  <TableCell>{row.titleEn}</TableCell>
                  <TableCell><Chip label={row.placement} size="small" variant="outlined" /></TableCell>
                  <TableCell>{new Date(row.startDate).toLocaleDateString()}</TableCell>
                  <TableCell>{row.impressions} views · {row.clicks} clicks</TableCell>
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
          <Typography variant="h6" sx={{ fontWeight: 700 }}>{drawerMode === 'create' ? 'Create ad' : 'Edit ad'}</Typography>
          <IconButton onClick={() => setDrawerOpen(false)}><Close /></IconButton>
        </Box>
        <Stack spacing={2}>
          <TextField label="Title (EN)" value={form.titleEn} onChange={(e) => set('titleEn', e.target.value)} required fullWidth />
          <TextField label="Title (AR)" value={form.titleAr} onChange={(e) => set('titleAr', e.target.value)} required fullWidth />
          <TextField label="Description (EN)" value={form.descriptionEn ?? ''} onChange={(e) => set('descriptionEn', e.target.value)} multiline rows={2} fullWidth />
          <TextField label="Image URL" value={form.imageUrl ?? ''} onChange={(e) => set('imageUrl', e.target.value)} fullWidth />
          <FormControl fullWidth>
            <InputLabel>Placement</InputLabel>
            <Select label="Placement" value={form.placement} onChange={(e) => set('placement', e.target.value)}>
              {AD_PLACEMENTS.map((p) => <MenuItem key={p} value={p}>{p}</MenuItem>)}
            </Select>
          </FormControl>
          <TextField type="date" label="Start date" value={form.startDate} onChange={(e) => set('startDate', e.target.value)} slotProps={{ inputLabel: { shrink: true } }} fullWidth />
          <TextField type="date" label="End date (optional)" value={form.endDate ?? ''} onChange={(e) => set('endDate', e.target.value || undefined)} slotProps={{ inputLabel: { shrink: true } }} fullWidth />
          <FormControlLabel control={<Switch checked={form.isActive} onChange={(e) => set('isActive', e.target.checked)} />} label="Active" />
        </Stack>
        <Box sx={{ mt: 3, display: 'flex', gap: 1 }}>
          <Button variant="contained" onClick={save} disabled={saving}>Save</Button>
          <Button onClick={() => setDrawerOpen(false)}>Cancel</Button>
        </Box>
      </Drawer>

      <Dialog open={deleteOpen} onClose={() => setDeleteOpen(false)}>
        <DialogTitle>Delete advertisement?</DialogTitle>
        <DialogActions>
          <Button onClick={() => setDeleteOpen(false)}>Cancel</Button>
          <Button color="error" variant="contained" onClick={async () => {
            if (!deleteId) return;
            try {
              await adminAdvertisementsApi.delete(deleteId);
              setSnack({ message: 'Advertisement deleted.', severity: 'success' });
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
