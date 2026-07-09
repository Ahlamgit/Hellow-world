import { useCallback, useEffect, useState } from 'react';
import {
  Alert, Box, Button, Paper, Snackbar, Table, TableBody, TableCell, TableContainer,
  TableHead, TableRow, TextField, Typography,
} from '@mui/material';
import { Save } from '@mui/icons-material';
import { adminSettingsApi, type SystemSetting } from './adminSettingsApi';
import { getApiErrorMessage } from '../utils/apiError';

export default function AdminSettingsPage() {
  const [settings, setSettings] = useState<SystemSetting[]>([]);
  const [drafts, setDrafts] = useState<Record<string, string>>({});
  const [loading, setLoading] = useState(true);
  const [savingId, setSavingId] = useState<string | null>(null);
  const [snack, setSnack] = useState<{ message: string; severity: 'success' | 'error' } | null>(null);

  const load = useCallback(async () => {
    setLoading(true);
    try {
      const res = await adminSettingsApi.list();
      setSettings(res.data.data);
      setDrafts(Object.fromEntries(res.data.data.map((s) => [s.id, s.value])));
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    } finally {
      setLoading(false);
    }
  }, []);

  useEffect(() => { load(); }, [load]);

  const save = async (setting: SystemSetting) => {
    setSavingId(setting.id);
    try {
      const res = await adminSettingsApi.update(setting.id, drafts[setting.id] ?? '');
      setSettings((prev) => prev.map((s) => (s.id === setting.id ? res.data.data : s)));
      setSnack({ message: `Updated ${setting.key}`, severity: 'success' });
    } catch (e) {
      setSnack({ message: getApiErrorMessage(e), severity: 'error' });
    } finally {
      setSavingId(null);
    }
  };

  const grouped = settings.reduce<Record<string, SystemSetting[]>>((acc, s) => {
    (acc[s.category] ??= []).push(s);
    return acc;
  }, {});

  return (
    <Box>
      <Typography variant="h5" sx={{ fontWeight: 700, mb: 2 }}>System Settings</Typography>
      {loading ? (
        <Typography>Loading...</Typography>
      ) : (
        Object.entries(grouped).map(([category, items]) => (
          <Paper key={category} sx={{ mb: 3, p: 2 }}>
            <Typography variant="h6" sx={{ mb: 2 }}>{category}</Typography>
            <TableContainer>
              <Table size="small">
                <TableHead>
                  <TableRow>
                    <TableCell>Key</TableCell>
                    <TableCell>Value</TableCell>
                    <TableCell>Description</TableCell>
                    <TableCell width={100} />
                  </TableRow>
                </TableHead>
                <TableBody>
                  {items.map((setting) => (
                    <TableRow key={setting.id}>
                      <TableCell sx={{ fontFamily: 'monospace' }}>{setting.key}</TableCell>
                      <TableCell>
                        <TextField
                          size="small"
                          fullWidth
                          value={drafts[setting.id] ?? ''}
                          onChange={(e) => setDrafts({ ...drafts, [setting.id]: e.target.value })}
                          disabled={setting.isEncrypted || savingId === setting.id}
                          helperText={setting.isEncrypted ? 'Encrypted — edit via secrets manager' : undefined}
                        />
                      </TableCell>
                      <TableCell>{setting.description ?? '—'}</TableCell>
                      <TableCell>
                        <Button
                          size="small"
                          startIcon={<Save />}
                          disabled={setting.isEncrypted || savingId === setting.id || drafts[setting.id] === setting.value}
                          onClick={() => save(setting)}
                        >
                          Save
                        </Button>
                      </TableCell>
                    </TableRow>
                  ))}
                </TableBody>
              </Table>
            </TableContainer>
          </Paper>
        ))
      )}
      <Snackbar open={!!snack} autoHideDuration={4000} onClose={() => setSnack(null)}>
        {snack ? <Alert severity={snack.severity}>{snack.message}</Alert> : undefined}
      </Snackbar>
    </Box>
  );
}
