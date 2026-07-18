import { useCallback, useEffect, useState } from 'react';
import {
  Alert, Box, Button, Chip, Dialog, DialogActions, DialogContent, DialogTitle,
  Drawer, FormControl, FormControlLabel, IconButton, InputLabel, MenuItem, Paper,
  Select, Snackbar, Switch, Table, TableBody, TableCell, TableContainer, TableHead,
  TablePagination, TableRow, TableSortLabel, TextField, Toolbar, Typography,
  Checkbox, ListItemText, OutlinedInput, Divider, Stack,
} from '@mui/material';
import { Add, Close, Refresh } from '@mui/icons-material';
import {
  adminUsersApi,
  PLATFORM_ROLES,
  USER_STATUSES,
  type AdminUserDetail,
  type AdminUserListItem,
  type AdminUserListQuery,
  type CreateAdminUserRequest,
  type UpdateAdminUserRequest,
  type UserPermissionEntry,
  type UserPermissionMatrix,
} from './adminUsersApi';
import { getApiErrorMessage } from '../utils/apiError';
import {
  canCreateUsers,
  canDeleteUsers,
  canEditUsers,
  canSuspendUsers,
  canVerifyUserEmail,
  hasPermission,
} from '../utils/permissions';
import { useAuth } from '../context/AuthContext';

const PAGE_SIZES = [10, 25, 50, 100];

const emptyCreateForm: CreateAdminUserRequest = {
  email: '',
  phone: '',
  password: '',
  firstName: '',
  lastName: '',
  role: 'Customer',
  preferredLanguage: 'ar',
  status: 'Active',
  sendVerificationEmail: true,
};

function StatusChip({ status }: { status: string }) {
  const color = status === 'Active' ? 'success' : status === 'Suspended' || status === 'Banned' ? 'error' : 'warning';
  return <Chip label={status} size="small" color={color} />;
}

export interface AdminUsersPageProps {
  title?: string;
  defaultRoleFilter?: string;
  lockRoleFilter?: boolean;
  defaultCreateRole?: string;
}

export default function AdminUsersPage({
  title = 'Users',
  defaultRoleFilter = '',
  lockRoleFilter = false,
  defaultCreateRole,
}: AdminUsersPageProps = {}) {
  const { user: authUser } = useAuth();
  const canEditPermissions = hasPermission(authUser, 'Permissions.Manage');
  const canCreate = canCreateUsers(authUser);
  const canEdit = canEditUsers(authUser);
  const canDelete = canDeleteUsers(authUser);
  const canSuspend = canSuspendUsers(authUser);
  const canVerifyEmail = canVerifyUserEmail(authUser);
  const [users, setUsers] = useState<AdminUserListItem[]>([]);
  const [totalCount, setTotalCount] = useState(0);
  const [loading, setLoading] = useState(true);
  const [searchInput, setSearchInput] = useState('');
  const [search, setSearch] = useState('');
  const [statusFilter, setStatusFilter] = useState('');
  const [roleFilter, setRoleFilter] = useState(defaultRoleFilter);
  const [page, setPage] = useState(0);
  const [pageSize, setPageSize] = useState(25);
  const [sortBy, setSortBy] = useState('createdAt');
  const [sortDirection, setSortDirection] = useState<'asc' | 'desc'>('desc');
  const [snack, setSnack] = useState<{ message: string; severity: 'success' | 'error' } | null>(null);

  const [createOpen, setCreateOpen] = useState(false);
  const [createForm, setCreateForm] = useState(emptyCreateForm);
  const [createLoading, setCreateLoading] = useState(false);

  const [detailOpen, setDetailOpen] = useState(false);
  const [selectedId, setSelectedId] = useState<string | null>(null);
  const [detail, setDetail] = useState<AdminUserDetail | null>(null);
  const [editForm, setEditForm] = useState<UpdateAdminUserRequest | null>(null);
  const [selectedRoles, setSelectedRoles] = useState<string[]>([]);
  const [primaryRole, setPrimaryRole] = useState('');
  const [suspendReason, setSuspendReason] = useState('');
  const [detailLoading, setDetailLoading] = useState(false);
  const [saving, setSaving] = useState(false);

  const [permMatrix, setPermMatrix] = useState<UserPermissionMatrix | null>(null);
  const [permOverrides, setPermOverrides] = useState<Record<string, 'inherit' | 'grant' | 'deny'>>({});
  const [permLoading, setPermLoading] = useState(false);

  const buildQuery = useCallback((): AdminUserListQuery => ({
    search: search || undefined,
    status: statusFilter || undefined,
    role: roleFilter || undefined,
    sortBy,
    sortDirection,
    page: page + 1,
    pageSize,
  }), [search, statusFilter, roleFilter, sortBy, sortDirection, page, pageSize]);

  const loadUsers = useCallback(async () => {
    setLoading(true);
    try {
      const { data } = await adminUsersApi.list(buildQuery());
      setUsers(data.data.items);
      setTotalCount(data.data.totalCount);
    } catch (err) {
      setSnack({ message: getApiErrorMessage(err, 'Failed to load users'), severity: 'error' });
    } finally {
      setLoading(false);
    }
  }, [buildQuery]);

  useEffect(() => { loadUsers(); }, [loadUsers]);

  const loadDetail = async (id: string) => {
    setDetailLoading(true);
    setPermLoading(true);
    try {
      const { data } = await adminUsersApi.getById(id);
      const user = data.data;
      setDetail(user);
      setEditForm({
        phone: user.phone,
        firstName: user.firstName,
        lastName: user.lastName,
        preferredLanguage: user.preferredLanguage,
        status: user.status,
      });
      setSelectedRoles([...user.roles]);
      setPrimaryRole(user.primaryRole);

      try {
        const permRes = await adminUsersApi.getPermissions(id);
        const matrix = permRes.data.data;
        setPermMatrix(matrix);
        const overrides: Record<string, 'inherit' | 'grant' | 'deny'> = {};
        matrix.permissions.forEach((p) => {
          overrides[p.permissionId] = p.override === null ? 'inherit' : p.override ? 'grant' : 'deny';
        });
        setPermOverrides(overrides);
      } catch {
        setPermMatrix(null);
        setPermOverrides({});
      }
    } catch (err) {
      setSnack({ message: getApiErrorMessage(err, 'Failed to load user'), severity: 'error' });
    } finally {
      setDetailLoading(false);
      setPermLoading(false);
    }
  };

  const openDetail = (id: string) => {
    setSelectedId(id);
    setDetailOpen(true);
    loadDetail(id);
  };

  const closeDetail = () => {
    setDetailOpen(false);
    setSelectedId(null);
    setDetail(null);
    setEditForm(null);
    setSuspendReason('');
    setPermMatrix(null);
    setPermOverrides({});
  };

  const handleSavePermissions = async () => {
    if (!selectedId || !permMatrix) return;
    setSaving(true);
    try {
      const overrides = Object.entries(permOverrides)
        .filter(([, state]) => state !== 'inherit')
        .map(([permissionId, state]) => ({
          permissionId,
          isGranted: state === 'grant',
        }));
      const { data } = await adminUsersApi.updatePermissions(selectedId, overrides);
      setPermMatrix(data.data);
      const next: Record<string, 'inherit' | 'grant' | 'deny'> = {};
      data.data.permissions.forEach((p) => {
        next[p.permissionId] = p.override === null ? 'inherit' : p.override ? 'grant' : 'deny';
      });
      setPermOverrides(next);
      setSnack({ message: 'Permission overrides saved', severity: 'success' });
      if (detail) {
        setDetail({ ...detail, permissions: data.data.permissions.filter((p) => p.effective).map((p) => p.code) });
      }
    } catch (err) {
      setSnack({ message: getApiErrorMessage(err, 'Failed to save permissions'), severity: 'error' });
    } finally {
      setSaving(false);
    }
  };

  const setPermState = (entry: UserPermissionEntry, state: 'inherit' | 'grant' | 'deny') => {
    setPermOverrides((prev) => ({ ...prev, [entry.permissionId]: state }));
  };

  const permModules = permMatrix
    ? Array.from(new Set(permMatrix.permissions.map((p) => p.module))).sort()
    : [];

  const handleSort = (key: string) => {
    if (sortBy === key) setSortDirection((d) => (d === 'asc' ? 'desc' : 'asc'));
    else { setSortBy(key); setSortDirection('asc'); }
    setPage(0);
  };

  const handleCreate = async () => {
    setCreateLoading(true);
    try {
      await adminUsersApi.create(createForm);
      setSnack({ message: 'User created successfully', severity: 'success' });
      setCreateOpen(false);
      setCreateForm(emptyCreateForm);
      loadUsers();
    } catch (err) {
      setSnack({ message: getApiErrorMessage(err, 'Failed to create user'), severity: 'error' });
    } finally {
      setCreateLoading(false);
    }
  };

  const handleSaveProfile = async () => {
    if (!selectedId || !editForm) return;
    setSaving(true);
    try {
      const { data } = await adminUsersApi.update(selectedId, editForm);
      setDetail(data.data);
      setSnack({ message: 'User updated', severity: 'success' });
      loadUsers();
    } catch (err) {
      setSnack({ message: getApiErrorMessage(err, 'Failed to update user'), severity: 'error' });
    } finally {
      setSaving(false);
    }
  };

  const handleSaveRoles = async () => {
    if (!selectedId) return;
    if (!selectedRoles.includes(primaryRole)) {
      setSnack({ message: 'Primary role must be included in assigned roles', severity: 'error' });
      return;
    }
    setSaving(true);
    try {
      const { data } = await adminUsersApi.assignRoles(selectedId, { roles: selectedRoles, primaryRole });
      setDetail(data.data);
      setSelectedRoles([...data.data.roles]);
      setPrimaryRole(data.data.primaryRole);
      setSnack({ message: 'Roles updated', severity: 'success' });
      loadUsers();
    } catch (err) {
      setSnack({ message: getApiErrorMessage(err, 'Failed to assign roles'), severity: 'error' });
    } finally {
      setSaving(false);
    }
  };

  const handleSuspend = async () => {
    if (!selectedId) return;
    setSaving(true);
    try {
      await adminUsersApi.suspend(selectedId, suspendReason || undefined);
      setSnack({ message: 'User suspended', severity: 'success' });
      await loadDetail(selectedId);
      loadUsers();
    } catch (err) {
      setSnack({ message: getApiErrorMessage(err, 'Failed to suspend user'), severity: 'error' });
    } finally {
      setSaving(false);
    }
  };

  const handleActivate = async () => {
    if (!selectedId) return;
    setSaving(true);
    try {
      await adminUsersApi.activate(selectedId);
      setSnack({ message: 'User activated', severity: 'success' });
      await loadDetail(selectedId);
      loadUsers();
    } catch (err) {
      setSnack({ message: getApiErrorMessage(err, 'Failed to activate user'), severity: 'error' });
    } finally {
      setSaving(false);
    }
  };

  const handleVerifyEmail = async () => {
    if (!selectedId) return;
    setSaving(true);
    try {
      await adminUsersApi.verifyEmail(selectedId);
      setSnack({ message: 'Email verified', severity: 'success' });
      await loadDetail(selectedId);
      loadUsers();
    } catch (err) {
      setSnack({ message: getApiErrorMessage(err, 'Failed to verify email'), severity: 'error' });
    } finally {
      setSaving(false);
    }
  };

  const handleDelete = async () => {
    if (!selectedId || !window.confirm('Delete this user? This is a soft delete.')) return;
    setSaving(true);
    try {
      await adminUsersApi.delete(selectedId);
      setSnack({ message: 'User deleted', severity: 'success' });
      closeDetail();
      loadUsers();
    } catch (err) {
      setSnack({ message: getApiErrorMessage(err, 'Failed to delete user'), severity: 'error' });
    } finally {
      setSaving(false);
    }
  };

  return (
    <Box>
      <Paper elevation={0} sx={{ border: 1, borderColor: 'divider' }}>
        <Toolbar sx={{ gap: 1, flexWrap: 'wrap', py: 2 }}>
          <Typography variant="h6" sx={{ flexGrow: 1, fontWeight: 700 }}>{title}</Typography>
          {canCreate && (
            <Button
              variant="contained"
              startIcon={<Add />}
              onClick={() => {
                setCreateForm({
                  ...emptyCreateForm,
                  role: defaultCreateRole ?? defaultRoleFilter ?? emptyCreateForm.role,
                });
                setCreateOpen(true);
              }}
            >
              Create User
            </Button>
          )}
          <IconButton onClick={loadUsers}><Refresh /></IconButton>
          <TextField
            size="small"
            placeholder="Search email, phone, name..."
            value={searchInput}
            onChange={(e) => setSearchInput(e.target.value)}
            onKeyDown={(e) => { if (e.key === 'Enter') { setSearch(searchInput); setPage(0); } }}
            sx={{ minWidth: 220 }}
          />
          <FormControl size="small" sx={{ minWidth: 120 }}>
            <InputLabel>Status</InputLabel>
            <Select label="Status" value={statusFilter} onChange={(e) => { setStatusFilter(e.target.value); setPage(0); }}>
              <MenuItem value="">All</MenuItem>
              {USER_STATUSES.map((s) => <MenuItem key={s} value={s}>{s}</MenuItem>)}
            </Select>
          </FormControl>
          <FormControl size="small" sx={{ minWidth: 140 }} disabled={lockRoleFilter}>
            <InputLabel>Role</InputLabel>
            <Select label="Role" value={roleFilter} onChange={(e) => { setRoleFilter(e.target.value); setPage(0); }}>
              <MenuItem value="">All</MenuItem>
              {PLATFORM_ROLES.map((r) => <MenuItem key={r} value={r}>{r}</MenuItem>)}
            </Select>
          </FormControl>
          <Button size="small" onClick={() => { setSearch(searchInput); setPage(0); }}>Search</Button>
        </Toolbar>

        <TableContainer>
          <Table size="small">
            <TableHead>
              <TableRow>
                <TableCell>
                  <TableSortLabel active={sortBy === 'email'} direction={sortBy === 'email' ? sortDirection : 'asc'} onClick={() => handleSort('email')}>
                    Email
                  </TableSortLabel>
                </TableCell>
                <TableCell>
                  <TableSortLabel active={sortBy === 'firstName'} direction={sortBy === 'firstName' ? sortDirection : 'asc'} onClick={() => handleSort('firstName')}>
                    Name
                  </TableSortLabel>
                </TableCell>
                <TableCell>Phone</TableCell>
                <TableCell>
                  <TableSortLabel active={sortBy === 'role'} direction={sortBy === 'role' ? sortDirection : 'asc'} onClick={() => handleSort('role')}>
                    Role
                  </TableSortLabel>
                </TableCell>
                <TableCell>
                  <TableSortLabel active={sortBy === 'status'} direction={sortBy === 'status' ? sortDirection : 'asc'} onClick={() => handleSort('status')}>
                    Status
                  </TableSortLabel>
                </TableCell>
                <TableCell>Verified</TableCell>
                <TableCell>
                  <TableSortLabel active={sortBy === 'createdAt'} direction={sortBy === 'createdAt' ? sortDirection : 'asc'} onClick={() => handleSort('createdAt')}>
                    Created
                  </TableSortLabel>
                </TableCell>
              </TableRow>
            </TableHead>
            <TableBody>
              {loading ? (
                <TableRow><TableCell colSpan={7} align="center">Loading...</TableCell></TableRow>
              ) : users.length === 0 ? (
                <TableRow><TableCell colSpan={7} align="center">No users found</TableCell></TableRow>
              ) : users.map((user) => (
                <TableRow key={user.id} hover sx={{ cursor: 'pointer' }} onClick={() => openDetail(user.id)}>
                  <TableCell>{user.email}</TableCell>
                  <TableCell>{user.firstName} {user.lastName}</TableCell>
                  <TableCell>{user.phone}</TableCell>
                  <TableCell><Chip label={user.primaryRole} size="small" variant="outlined" /></TableCell>
                  <TableCell><StatusChip status={user.status} /></TableCell>
                  <TableCell>
                    <Chip label={user.emailVerified ? 'Email' : 'Unverified'} size="small" color={user.emailVerified ? 'success' : 'default'} />
                  </TableCell>
                  <TableCell>{new Date(user.createdAt).toLocaleDateString()}</TableCell>
                </TableRow>
              ))}
            </TableBody>
          </Table>
        </TableContainer>

        <TablePagination
          component="div"
          count={totalCount}
          page={page}
          onPageChange={(_, p) => setPage(p)}
          rowsPerPage={pageSize}
          onRowsPerPageChange={(e) => { setPageSize(parseInt(e.target.value, 10)); setPage(0); }}
          rowsPerPageOptions={PAGE_SIZES}
        />
      </Paper>

      {/* Create User Dialog */}
      <Dialog open={createOpen} onClose={() => setCreateOpen(false)} maxWidth="sm" fullWidth>
        <DialogTitle>Create User</DialogTitle>
        <DialogContent sx={{ display: 'flex', flexDirection: 'column', gap: 2, pt: 1 }}>
          <TextField label="Email" type="email" value={createForm.email} onChange={(e) => setCreateForm({ ...createForm, email: e.target.value })} fullWidth required />
          <TextField label="Phone" value={createForm.phone} onChange={(e) => setCreateForm({ ...createForm, phone: e.target.value })} fullWidth required />
          <TextField label="Password" type="password" value={createForm.password} onChange={(e) => setCreateForm({ ...createForm, password: e.target.value })} fullWidth required />
          <TextField label="First Name" value={createForm.firstName} onChange={(e) => setCreateForm({ ...createForm, firstName: e.target.value })} fullWidth required />
          <TextField label="Last Name" value={createForm.lastName} onChange={(e) => setCreateForm({ ...createForm, lastName: e.target.value })} fullWidth required />
          <FormControl fullWidth>
            <InputLabel>Role</InputLabel>
            <Select label="Role" value={createForm.role} onChange={(e) => setCreateForm({ ...createForm, role: e.target.value })}>
              {PLATFORM_ROLES.map((r) => <MenuItem key={r} value={r}>{r}</MenuItem>)}
            </Select>
          </FormControl>
          <FormControl fullWidth>
            <InputLabel>Status</InputLabel>
            <Select label="Status" value={createForm.status} onChange={(e) => setCreateForm({ ...createForm, status: e.target.value })}>
              {USER_STATUSES.map((s) => <MenuItem key={s} value={s}>{s}</MenuItem>)}
            </Select>
          </FormControl>
          <FormControl fullWidth>
            <InputLabel>Language</InputLabel>
            <Select label="Language" value={createForm.preferredLanguage} onChange={(e) => setCreateForm({ ...createForm, preferredLanguage: e.target.value })}>
              <MenuItem value="ar">Arabic</MenuItem>
              <MenuItem value="en">English</MenuItem>
            </Select>
          </FormControl>
          <FormControlLabel
            control={<Switch checked={createForm.sendVerificationEmail} onChange={(e) => setCreateForm({ ...createForm, sendVerificationEmail: e.target.checked })} />}
            label="Send verification email"
          />
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setCreateOpen(false)}>Cancel</Button>
          <Button variant="contained" onClick={handleCreate} disabled={createLoading}>
            {createLoading ? 'Creating...' : 'Create'}
          </Button>
        </DialogActions>
      </Dialog>

      {/* User Detail Drawer */}
      <Drawer anchor="right" open={detailOpen} onClose={closeDetail} slotProps={{ paper: { sx: { width: { xs: '100%', sm: 480 } } } }}>
        <Box sx={{ p: 2, display: 'flex', alignItems: 'center', justifyContent: 'space-between' }}>
          <Typography variant="h6" sx={{ fontWeight: 700 }}>User Details</Typography>
          <IconButton onClick={closeDetail}><Close /></IconButton>
        </Box>
        <Divider />
        {detailLoading || !detail || !editForm ? (
          <Box sx={{ p: 3 }}><Typography>Loading...</Typography></Box>
        ) : (
          <Box sx={{ p: 2, display: 'flex', flexDirection: 'column', gap: 2, overflow: 'auto' }}>
            <Typography variant="body2" color="text.secondary">{detail.email}</Typography>
            <Stack direction="row" spacing={1} sx={{ flexWrap: 'wrap' }}>
              <StatusChip status={detail.status} />
              <Chip label={detail.primaryRole} size="small" />
              {detail.emailVerified && <Chip label="Email verified" size="small" color="success" />}
              {detail.subscriptionStatus && (
                <Chip
                  label={`Subscription: ${detail.subscriptionStatus}`}
                  size="small"
                  variant="outlined"
                />
              )}
            </Stack>
            {detail.subscriptionExpiresAt && (
              <Typography variant="caption" color="text.secondary">
                Subscription expires: {new Date(detail.subscriptionExpiresAt).toLocaleDateString()}
              </Typography>
            )}

            <Typography variant="subtitle2" sx={{ fontWeight: 600 }}>Profile</Typography>
            <TextField label="First Name" size="small" value={editForm.firstName} onChange={(e) => setEditForm({ ...editForm, firstName: e.target.value })} fullWidth disabled={!canEdit} />
            <TextField label="Last Name" size="small" value={editForm.lastName} onChange={(e) => setEditForm({ ...editForm, lastName: e.target.value })} fullWidth disabled={!canEdit} />
            <TextField label="Phone" size="small" value={editForm.phone} onChange={(e) => setEditForm({ ...editForm, phone: e.target.value })} fullWidth disabled={!canEdit} />
            <FormControl size="small" fullWidth>
              <InputLabel>Status</InputLabel>
              <Select label="Status" value={editForm.status} onChange={(e) => setEditForm({ ...editForm, status: e.target.value })} disabled={!canEdit}>
                {USER_STATUSES.map((s) => <MenuItem key={s} value={s}>{s}</MenuItem>)}
              </Select>
            </FormControl>
            <FormControl size="small" fullWidth>
              <InputLabel>Language</InputLabel>
              <Select label="Language" value={editForm.preferredLanguage} onChange={(e) => setEditForm({ ...editForm, preferredLanguage: e.target.value })} disabled={!canEdit}>
                <MenuItem value="ar">Arabic</MenuItem>
                <MenuItem value="en">English</MenuItem>
              </Select>
            </FormControl>
            {canEdit && (
              <Button variant="contained" onClick={handleSaveProfile} disabled={saving}>Save Profile</Button>
            )}

            <Divider />

            <Typography variant="subtitle2" sx={{ fontWeight: 600 }}>Roles</Typography>
            <FormControl size="small" fullWidth>
              <InputLabel>Assigned Roles</InputLabel>
              <Select
                multiple
                value={selectedRoles}
                onChange={(e) => setSelectedRoles(typeof e.target.value === 'string' ? e.target.value.split(',') : e.target.value)}
                input={<OutlinedInput label="Assigned Roles" />}
                renderValue={(selected) => selected.join(', ')}
                disabled={!canEdit}
              >
                {PLATFORM_ROLES.map((r) => (
                  <MenuItem key={r} value={r}>
                    <Checkbox checked={selectedRoles.includes(r)} />
                    <ListItemText primary={r} />
                  </MenuItem>
                ))}
              </Select>
            </FormControl>
            <FormControl size="small" fullWidth>
              <InputLabel>Primary Role</InputLabel>
              <Select label="Primary Role" value={primaryRole} onChange={(e) => setPrimaryRole(e.target.value)} disabled={!canEdit}>
                {selectedRoles.map((r) => <MenuItem key={r} value={r}>{r}</MenuItem>)}
              </Select>
            </FormControl>
            <Button variant="outlined" onClick={handleSaveRoles} disabled={saving || !canEdit}>Save Roles</Button>

            <Divider />

            <Typography variant="subtitle2" sx={{ fontWeight: 600 }}>Actions</Typography>
            {detail.status === 'Suspended' ? (
              canEdit && <Button color="success" variant="outlined" onClick={handleActivate} disabled={saving}>Activate</Button>
            ) : (
              canSuspend && (
                <>
                  <TextField label="Suspend reason (optional)" size="small" value={suspendReason} onChange={(e) => setSuspendReason(e.target.value)} fullWidth />
                  <Button color="warning" variant="outlined" onClick={handleSuspend} disabled={saving}>Suspend</Button>
                </>
              )
            )}
            {!detail.emailVerified && canVerifyEmail && (
              <Button variant="outlined" onClick={handleVerifyEmail} disabled={saving}>Verify Email</Button>
            )}
            {canDelete && (
              <Button color="error" variant="outlined" onClick={handleDelete} disabled={saving}>Delete User</Button>
            )}

            {permMatrix && (
              <>
                <Divider />
                <Typography variant="subtitle2" sx={{ fontWeight: 600 }}>
                  Permission overrides
                  {permLoading && ' (loading…)'}
                </Typography>
                <Typography variant="caption" color="text.secondary">
                  Inherit uses role permissions. Grant/deny applies a direct override.
                </Typography>
                {permModules.map((mod) => (
                  <Box key={mod} sx={{ mt: 1 }}>
                    <Typography variant="caption" sx={{ fontWeight: 600, color: 'text.secondary' }}>{mod}</Typography>
                    <Stack spacing={0.5} sx={{ mt: 0.5 }}>
                      {permMatrix.permissions.filter((p) => p.module === mod).map((p) => {
                        const state = permOverrides[p.permissionId] ?? 'inherit';
                        return (
                          <Box key={p.permissionId} sx={{ display: 'flex', alignItems: 'center', gap: 1, flexWrap: 'wrap' }}>
                            <Typography variant="body2" sx={{ flex: 1, minWidth: 160 }}>{p.code}</Typography>
                            {p.fromRole && <Chip label="role" size="small" variant="outlined" />}
                            {canEditPermissions ? (
                              <FormControl size="small" sx={{ minWidth: 110 }}>
                                <Select
                                  value={state}
                                  onChange={(e) => setPermState(p, e.target.value as 'inherit' | 'grant' | 'deny')}
                                >
                                  <MenuItem value="inherit">Inherit</MenuItem>
                                  <MenuItem value="grant">Grant</MenuItem>
                                  <MenuItem value="deny">Deny</MenuItem>
                                </Select>
                              </FormControl>
                            ) : (
                              <Chip
                                label={p.effective ? 'allowed' : 'denied'}
                                size="small"
                                color={p.effective ? 'success' : 'default'}
                              />
                            )}
                          </Box>
                        );
                      })}
                    </Stack>
                  </Box>
                ))}
                {canEditPermissions && (
                  <Button variant="outlined" onClick={handleSavePermissions} disabled={saving || permLoading}>
                    Save permission overrides
                  </Button>
                )}
              </>
            )}

            {!permMatrix && detail.permissions.length > 0 && (
              <>
                <Divider />
                <Typography variant="subtitle2" sx={{ fontWeight: 600 }}>Permissions ({detail.permissions.length})</Typography>
                <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 0.5 }}>
                  {detail.permissions.slice(0, 12).map((p) => <Chip key={p} label={p} size="small" />)}
                  {detail.permissions.length > 12 && <Chip label={`+${detail.permissions.length - 12} more`} size="small" />}
                </Box>
              </>
            )}
          </Box>
        )}
      </Drawer>

      {snack && (
        <Snackbar open autoHideDuration={4000} onClose={() => setSnack(null)}>
          <Alert severity={snack.severity} onClose={() => setSnack(null)}>{snack.message}</Alert>
        </Snackbar>
      )}
    </Box>
  );
}
