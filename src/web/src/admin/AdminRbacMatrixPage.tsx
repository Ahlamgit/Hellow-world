import { useCallback, useEffect, useMemo, useState } from 'react';
import {
  Alert, Box, Button, Checkbox, FormControl, InputLabel, MenuItem,
  Paper, Select, Snackbar, Table, TableBody, TableCell, TableContainer, TableHead,
  TableRow, Typography,
} from '@mui/material';
import { Save } from '@mui/icons-material';
import { adminRbacApi, type AdminRole, type RolePermissionMatrix } from './adminRbacApi';
import { getApiErrorMessage } from '../utils/apiError';
import { hasPermission } from '../utils/permissions';
import { useAuth } from '../context/AuthContext';

export default function AdminRbacMatrixPage() {
  const { user } = useAuth();
  const canEdit = hasPermission(user, 'Roles.Manage');

  const [roles, setRoles] = useState<AdminRole[]>([]);
  const [selectedRoleId, setSelectedRoleId] = useState('');
  const [matrix, setMatrix] = useState<RolePermissionMatrix | null>(null);
  const [selected, setSelected] = useState<Set<string>>(new Set());
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [snack, setSnack] = useState<{ message: string; severity: 'success' | 'error' } | null>(null);

  const loadRoles = useCallback(async () => {
    setLoading(true);
    try {
      const res = await adminRbacApi.listRoles();
      setRoles(res.data.data);
      if (!selectedRoleId && res.data.data.length > 0) {
        setSelectedRoleId(res.data.data[0].id);
      }
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    } finally {
      setLoading(false);
    }
  }, [selectedRoleId]);

  const loadMatrix = useCallback(async (roleId: string) => {
    if (!roleId) return;
    setLoading(true);
    try {
      const res = await adminRbacApi.getRolePermissions(roleId);
      setMatrix(res.data.data);
      setSelected(new Set(res.data.data.assignedPermissionIds));
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { loadRoles(); }, [loadRoles]);
  useEffect(() => { if (selectedRoleId) loadMatrix(selectedRoleId); }, [selectedRoleId, loadMatrix]);

  const modules = useMemo(() => {
    if (!matrix) return [];
    const map = new Map<string, typeof matrix.allPermissions>();
    matrix.allPermissions.forEach((p) => {
      const list = map.get(p.module) ?? [];
      list.push(p);
      map.set(p.module, list);
    });
    return Array.from(map.entries()).sort(([a], [b]) => a.localeCompare(b));
  }, [matrix]);

  const toggle = (id: string) => {
    setSelected((prev) => {
      const next = new Set(prev);
      if (next.has(id)) next.delete(id);
      else next.add(id);
      return next;
    });
  };

  const save = async () => {
    if (!selectedRoleId || !canEdit) return;
    setSaving(true);
    try {
      const res = await adminRbacApi.updateRolePermissions(selectedRoleId, Array.from(selected));
      setMatrix(res.data.data);
      setSelected(new Set(res.data.data.assignedPermissionIds));
      setSnack({ message: 'Role permissions saved', severity: 'success' });
      await loadRoles();
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    } finally {
      setSaving(false);
    }
  };

  return (
    <Box>
      <Typography variant="h5" sx={{ fontWeight: 700, mb: 2 }}>Role Permissions</Typography>

      <Paper sx={{ p: 2, mb: 3 }}>
        <FormControl size="small" sx={{ minWidth: 280 }}>
          <InputLabel>Role</InputLabel>
          <Select
            label="Role"
            value={selectedRoleId}
            onChange={(e) => setSelectedRoleId(e.target.value)}
          >
            {roles.map((role) => (
              <MenuItem key={role.id} value={role.id}>
                {role.name} ({role.permissionCount} permissions)
              </MenuItem>
            ))}
          </Select>
        </FormControl>
        {canEdit && (
          <Button
            sx={{ ml: 2 }}
            variant="contained"
            startIcon={<Save />}
            disabled={saving || loading || !selectedRoleId}
            onClick={save}
          >
            Save permissions
          </Button>
        )}
      </Paper>

      {loading && !matrix ? (
        <Typography>Loading...</Typography>
      ) : matrix ? (
        modules.map(([module, permissions]) => (
          <Paper key={module} sx={{ mb: 2, p: 2 }}>
            <Typography variant="subtitle1" sx={{ fontWeight: 600, mb: 1 }}>{module}</Typography>
            <TableContainer>
              <Table size="small">
                <TableHead>
                  <TableRow>
                    <TableCell width={48} />
                    <TableCell>Code</TableCell>
                    <TableCell>Name</TableCell>
                  </TableRow>
                </TableHead>
                <TableBody>
                  {permissions.map((perm) => (
                    <TableRow key={perm.id} hover>
                      <TableCell>
                        <Checkbox
                          checked={selected.has(perm.id)}
                          onChange={() => toggle(perm.id)}
                          disabled={!canEdit}
                        />
                      </TableCell>
                      <TableCell sx={{ fontFamily: 'monospace' }}>{perm.code}</TableCell>
                      <TableCell>{perm.nameEn}</TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </Paper>
        ))
      ) : null}

      <Snackbar open={!!snack} autoHideDuration={4000} onClose={() => setSnack(null)}>
        {snack ? <Alert severity={snack.severity}>{snack.message}</Alert> : undefined}
      </Snackbar>
    </Box>
  );
}
