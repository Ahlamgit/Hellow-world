import { useCallback, useEffect, useState } from 'react';
import {
  Box, Button, Checkbox, Chip, FormControl, IconButton, InputAdornment, InputLabel,
  MenuItem, Paper, Select, Snackbar, Alert, Table, TableBody, TableCell, TableContainer,
  TableHead, TablePagination, TableRow, TableSortLabel, TextField, Toolbar, Typography,
} from '@mui/material';
import { Search, FileDownload, PictureAsPdf, FilterList } from '@mui/icons-material';
import { adminApi } from './adminApi';
import type { AdminListQuery, AdminListResult, ModuleConfig } from './moduleConfig';

interface AdminDataTableProps {
  config: ModuleConfig;
  extraActions?: React.ReactNode;
  onRowAction?: (id: string) => void;
}

const PAGE_SIZES = [10, 25, 50, 100];

export default function AdminDataTable({ config, extraActions, onRowAction }: AdminDataTableProps) {
  const [data, setData] = useState<AdminListResult | null>(null);
  const [loading, setLoading] = useState(true);
  const [selected, setSelected] = useState<string[]>([]);
  const [search, setSearch] = useState('');
  const [searchInput, setSearchInput] = useState('');
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortBy, setSortBy] = useState(config.columns.find((c) => c.sortable)?.key ?? '');
  const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('desc');
  const [filters, setFilters] = useState<Record<string, string>>({});
  const [showFilters, setShowFilters] = useState(false);
  const [snack, setSnack] = useState<{ message: string; severity: 'success' | 'error' } | null>(null);
  const [exporting, setExporting] = useState(false);

  const buildQuery = useCallback((): AdminListQuery => ({
    search: search || undefined,
    sortBy: sortBy || undefined,
    sortDirection,
    page: page + 1,
    pageSize,
    status: filters.status || undefined,
    role: filters.role || undefined,
    type: filters.type || undefined,
    category: filters.category || undefined,
    fromDate: filters.fromDate || undefined,
    toDate: filters.toDate || undefined,
    isActive: filters.isActive === 'true' ? true : filters.isActive === 'false' ? false : undefined,
  }), [search, sortBy, sortDirection, page, pageSize, filters]);

  const loadData = useCallback(async () => {
    setLoading(true);
    try {
      const { data: res } = await adminApi.listModule(config.key, buildQuery());
      setData(res.data);
      setSelected([]);
    } catch {
      setSnack({ message: 'Failed to load data', severity: 'error' });
    } finally {
      setLoading(false);
    }
  }, [config.key, buildQuery]);

  useEffect(() => { loadData(); }, [loadData]);

  const handleSort = (key: string) => {
    if (sortBy === key) {
      setSortDirection((d) => (d === 'asc' ? 'desc' : 'asc'));
    } else {
      setSortBy(key);
      setSortDirection('asc');
    }
    setPage(0);
  };

  const handleSelectAll = (checked: boolean) => {
    setSelected(checked ? (data?.items.map((i) => i.id) ?? []) : []);
  };

  const handleBulk = async (action: string) => {
    if (!selected.length) return;
    try {
      const { data: res } = await adminApi.bulkAction(config.key, action, selected);
      setSnack({ message: res.data.message, severity: 'success' });
      loadData();
    } catch {
      setSnack({ message: 'Bulk action failed', severity: 'error' });
    }
  };

  const handleExport = async (format: 'xlsx' | 'pdf') => {
    setExporting(true);
    try {
      await adminApi.exportModule(config.key, format, buildQuery());
      setSnack({ message: `Exported as ${format.toUpperCase()}`, severity: 'success' });
    } catch {
      setSnack({ message: 'Export failed', severity: 'error' });
    } finally {
      setExporting(false);
    }
  };

  const formatCell = (value: string | null | undefined) => {
    if (value === 'true' || value === 'false') {
      return <Chip label={value === 'true' ? 'Yes' : 'No'} size="small" color={value === 'true' ? 'success' : 'default'} />;
    }
    if (value && /^\d{4}-\d{2}-\d{2}T/.test(value)) {
      return new Date(value).toLocaleString();
    }
    return value ?? '—';
  };

  return (
    <Paper elevation={0} sx={{ border: 1, borderColor: 'divider' }}>
      <Toolbar sx={{ gap: 1, flexWrap: 'wrap', py: 2 }}>
        <Typography variant="h6" sx={{ flexGrow: 1, fontWeight: 700 }}>{config.title}</Typography>
        {extraActions}
        <TextField
          size="small"
          placeholder="Search..."
          value={searchInput}
          onChange={(e) => setSearchInput(e.target.value)}
          onKeyDown={(e) => { if (e.key === 'Enter') { setSearch(searchInput); setPage(0); } }}
          slotProps={{
            input: {
              startAdornment: (
                <InputAdornment position="start">
                  <IconButton size="small" onClick={() => { setSearch(searchInput); setPage(0); }}>
                    <Search />
                  </IconButton>
                </InputAdornment>
              ),
            },
          }}
          sx={{ minWidth: 200 }}
        />
        {config.filters && config.filters.length > 0 && (
          <IconButton onClick={() => setShowFilters(!showFilters)} color={showFilters ? 'primary' : 'default'}>
            <FilterList />
          </IconButton>
        )}
        <Button size="small" startIcon={<FileDownload />} disabled={exporting} onClick={() => handleExport('xlsx')}>
          Excel
        </Button>
        <Button size="small" startIcon={<PictureAsPdf />} disabled={exporting} onClick={() => handleExport('pdf')}>
          PDF
        </Button>
      </Toolbar>

      {showFilters && config.filters && (
        <Box sx={{ px: 2, pb: 2, display: 'flex', gap: 2, flexWrap: 'wrap' }}>
          {config.filters.map((f) => (
            <FormControl key={f.key} size="small" sx={{ minWidth: 140 }}>
              <InputLabel>{f.label}</InputLabel>
              <Select
                label={f.label}
                value={filters[f.key] ?? ''}
                onChange={(e) => { setFilters((prev) => ({ ...prev, [f.key]: e.target.value })); setPage(0); }}
              >
                <MenuItem value="">All</MenuItem>
                {f.options?.map((opt) => <MenuItem key={opt} value={opt}>{opt}</MenuItem>)}
              </Select>
            </FormControl>
          ))}
        </Box>
      )}

      {config.bulkActions && selected.length > 0 && (
        <Box sx={{ px: 2, pb: 1, display: 'flex', gap: 1, flexWrap: 'wrap', alignItems: 'center' }}>
          <Typography variant="body2" color="text.secondary">{selected.length} selected</Typography>
          {config.bulkActions.map((action) => (
            <Button key={action} size="small" variant="outlined" onClick={() => handleBulk(action)}>
              {action.charAt(0).toUpperCase() + action.slice(1)}
            </Button>
          ))}
        </Box>
      )}

      <TableContainer>
        <Table size="small">
          <TableHead>
            <TableRow>
              {config.bulkActions && (
                <TableCell padding="checkbox">
                  <Checkbox
                    indeterminate={selected.length > 0 && selected.length < (data?.items.length ?? 0)}
                    checked={!!data?.items.length && selected.length === data.items.length}
                    onChange={(e) => handleSelectAll(e.target.checked)}
                  />
                </TableCell>
              )}
              {config.columns.map((col) => (
                <TableCell key={col.key}>
                  {col.sortable ? (
                    <TableSortLabel
                      active={sortBy === col.key}
                      direction={sortBy === col.key ? sortDirection : 'asc'}
                      onClick={() => handleSort(col.key)}
                    >
                      {col.label}
                    </TableSortLabel>
                  ) : col.label}
                </TableCell>
              ))}
            </TableRow>
          </TableHead>
          <TableBody>
            {loading ? (
              <TableRow><TableCell colSpan={config.columns.length + (config.bulkActions ? 1 : 0)} align="center">Loading...</TableCell></TableRow>
            ) : !data?.items.length ? (
              <TableRow><TableCell colSpan={config.columns.length + (config.bulkActions ? 1 : 0)} align="center">No records found</TableCell></TableRow>
            ) : (
              data.items.map((row) => (
                <TableRow key={row.id} hover selected={selected.includes(row.id)} onClick={() => onRowAction?.(row.id)} sx={{ cursor: onRowAction ? 'pointer' : 'default' }}>
                  {config.bulkActions && (
                    <TableCell padding="checkbox" onClick={(e) => e.stopPropagation()}>
                      <Checkbox
                        checked={selected.includes(row.id)}
                        onChange={(e) => {
                          setSelected((prev) => e.target.checked ? [...prev, row.id] : prev.filter((id) => id !== row.id));
                        }}
                      />
                    </TableCell>
                  )}
                  {config.columns.map((col) => (
                    <TableCell key={col.key}>{formatCell(row.columns[col.key])}</TableCell>
                  ))}
                </TableRow>
              ))
            )}
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

      {snack && (
        <Snackbar open autoHideDuration={4000} onClose={() => setSnack(null)}>
          <Alert severity={snack.severity} onClose={() => setSnack(null)}>{snack.message}</Alert>
        </Snackbar>
      )}
    </Paper>
  );
}
