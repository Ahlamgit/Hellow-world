import { useCallback, useEffect, useState } from 'react';
import {
  Alert, Box, Button, Chip, Dialog, DialogActions, DialogTitle,
  Drawer, FormControl, FormControlLabel, IconButton, InputLabel, MenuItem,
  Paper, Select, Snackbar, Stack, Switch, Table, TableBody, TableCell,
  TableContainer, TableHead, TablePagination, TableRow, TextField, Toolbar, Typography,
} from '@mui/material';
import { Add, Close, Refresh } from '@mui/icons-material';
import {
  adminCitiesApi, adminRegionsApi, emptyCityForm,
  type City, type CityListQuery, type CreateCityRequest, type Region,
} from './adminLocationApi';
import { getApiErrorMessage } from '../utils/apiError';

const PAGE_SIZES = [10, 25, 50];

export default function AdminCitiesPage() {
  const [items, setItems] = useState<City[]>([]);
  const [regions, setRegions] = useState<Region[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [searchInput, setSearchInput] = useState('');
  const [search, setSearch] = useState('');
  const [regionFilter, setRegionFilter] = useState('');
  const [activeOnly, setActiveOnly] = useState(false);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [snack, setSnack] = useState<{ message: string; severity: 'success' | 'error' } | null>(null);
  const [drawerOpen, setDrawerOpen] = useState(false);
  const [drawerMode, setDrawerMode] = useState<'create' | 'edit'>('create');
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [form, setForm] = useState<CreateCityRequest>(emptyCityForm());
  const [saving, setSaving] = useState(false);
  const [deleteOpen, setDeleteOpen] = useState(false);
  const [deleteId, setDeleteId] = useState<string | null>(null);

  useEffect(() => {
    adminRegionsApi.list({ pageSize: 200 }).then((res) => setRegions(res.data.data.items)).catch(() => {});
  }, []);

  const buildQuery = useCallback((): CityListQuery => ({
    search: search || undefined,
    regionId: regionFilter || undefined,
    isActive: activeOnly ? true : undefined,
    page: page + 1,
    pageSize,
  }), [search, regionFilter, activeOnly, page, pageSize]);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const res = await adminCitiesApi.list(buildQuery());
      setItems(res.data.data.items);
      setTotalCount(res.data.data.totalCount);
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    } finally {
      setLoading(false);
    }
  }, [buildQuery]);

  useEffect(() => { load(); }, [load]);

  const set = <K extends keyof CreateCityRequest>(key: K, value: CreateCityRequest[K]) =>
    setForm({ ...form, [key]: value });

  const openCreate = () => {
    setDrawerMode('create');
    setSelectedId(null);
    setForm({ ...emptyCityForm(), regionId: regions[0]?.id ?? '' });
    setDrawerOpen(true);
  };

  const openEdit = async (id: string) => {
    setDrawerMode('edit');
    setSelectedId(id);
    setDrawerOpen(true);
    try {
      const res = await adminCitiesApi.getById(id);
      const c = res.data.data;
      setForm({ regionId: c.regionId, nameEn: c.nameEn, nameAr: c.nameAr, code: c.code, isActive: c.isActive });
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
      setDrawerOpen(false);
    }
  };

  const save = async () => {
    setSaving(true);
    try {
      if (drawerMode === 'create') await adminCitiesApi.create(form);
      else if (selectedId) await adminCitiesApi.update(selectedId, form);
      setSnack({ message: 'City saved.', severity: 'success' });
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
        <Typography variant="h5" sx={{ fontWeight: 700 }}>Cities</Typography>
        <Button variant="contained" startIcon={<Add />} onClick={openCreate} disabled={!regions.length}>New city</Button>
      </Box>
      <Paper sx={{ mb: 2 }}>
        <Toolbar sx={{ gap: 2, flexWrap: 'wrap' }}>
          <TextField size="small" label="Search" value={searchInput}
            onChange={(e) => setSearchInput(e.target.value)}
            onKeyDown={(e) => e.key === 'Enter' && (setSearch(searchInput), setPage(0))} />
          <FormControl size="small" sx={{ minWidth: 180 }}>
            <InputLabel>Region</InputLabel>
            <Select label="Region" value={regionFilter} onChange={(e) => { setRegionFilter(e.target.value); setPage(0); }}>
              <MenuItem value="">All</MenuItem>
              {regions.map((r) => <MenuItem key={r.id} value={r.id}>{r.nameEn}</MenuItem>)}
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
              <TableCell>Code</TableCell>
              <TableCell>Name</TableCell>
              <TableCell>Region</TableCell>
              <TableCell>Status</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? <TableRow><TableCell colSpan={5} align="center">Loading…</TableCell></TableRow>
              : items.map((row) => (
                <TableRow key={row.id} hover>
                  <TableCell sx={{ fontFamily: 'monospace' }}>{row.code}</TableCell>
                  <TableCell>{row.nameEn}</TableCell>
                  <TableCell>{row.regionName}</TableCell>
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

      <Drawer anchor="right" open={drawerOpen} onClose={() => setDrawerOpen(false)} slotProps={{ paper: { sx: { width: 400, p: 3 } } }}>
        <Box sx={{ display: 'flex', justifyContent: 'space-between', mb: 2 }}>
          <Typography variant="h6" sx={{ fontWeight: 700 }}>{drawerMode === 'create' ? 'Create city' : 'Edit city'}</Typography>
          <IconButton onClick={() => setDrawerOpen(false)}><Close /></IconButton>
        </Box>
        <Stack spacing={2}>
          <FormControl fullWidth required>
            <InputLabel>Region</InputLabel>
            <Select label="Region" value={form.regionId} onChange={(e) => set('regionId', e.target.value)}>
              {regions.map((r) => <MenuItem key={r.id} value={r.id}>{r.nameEn}</MenuItem>)}
            </Select>
          </FormControl>
          <TextField label="Code" value={form.code} onChange={(e) => set('code', e.target.value.toUpperCase())} required fullWidth />
          <TextField label="Name (EN)" value={form.nameEn} onChange={(e) => set('nameEn', e.target.value)} required fullWidth />
          <TextField label="Name (AR)" value={form.nameAr} onChange={(e) => set('nameAr', e.target.value)} required fullWidth />
          <FormControlLabel control={<Switch checked={form.isActive} onChange={(e) => set('isActive', e.target.checked)} />} label="Active" />
        </Stack>
        <Box sx={{ mt: 3, display: 'flex', gap: 1 }}>
          <Button variant="contained" onClick={save} disabled={saving || !form.regionId}>Save</Button>
          <Button onClick={() => setDrawerOpen(false)}>Cancel</Button>
        </Box>
      </Drawer>

      <Dialog open={deleteOpen} onClose={() => setDeleteOpen(false)}>
        <DialogTitle>Delete city?</DialogTitle>
        <DialogActions>
          <Button onClick={() => setDeleteOpen(false)}>Cancel</Button>
          <Button color="error" variant="contained" onClick={async () => {
            if (!deleteId) return;
            try {
              await adminCitiesApi.delete(deleteId);
              setSnack({ message: 'City deleted.', severity: 'success' });
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
