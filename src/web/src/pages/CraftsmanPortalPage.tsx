import { useEffect, useState } from 'react';
import {
  Alert, Box, Button, Card, CardContent, Chip, Container, FormControlLabel,
  Grid, Link, MenuItem, Switch, TextField, Typography,
} from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';
import { craftsmanApi, servicesApi, usersApi, type CraftsmanProfile, type CraftsmanServiceItem } from '../services/api';
import { formatCurrency } from '../config/platform';

const DAYS = ['Sunday', 'Monday', 'Tuesday', 'Wednesday', 'Thursday', 'Friday', 'Saturday'];

export function CraftsmanPortalPage() {
  const [profile, setProfile] = useState<CraftsmanProfile | null>(null);
  const [services, setServices] = useState<{ id: string; nameEn: string }[]>([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');
  const [form, setForm] = useState({ specialization: '', yearsOfExperience: 0, isAvailable: true, serviceRadiusKm: 25 });
  const [newService, setNewService] = useState({ serviceId: '', customPrice: 0 });

  const [hasLocatedAddress, setHasLocatedAddress] = useState(true);

  const load = async () => {
    setLoading(true);
    try {
      const [profileRes, servicesRes, addressesRes] = await Promise.all([
        craftsmanApi.getProfile(),
        servicesApi.getServices(),
        usersApi.listAddresses(),
      ]);
      const p = profileRes.data.data;
      setProfile(p);
      setForm({
        specialization: p.specialization ?? '',
        yearsOfExperience: p.yearsOfExperience,
        isAvailable: p.isAvailable,
        serviceRadiusKm: p.serviceRadiusKm ?? 25,
      });
      setServices(servicesRes.data.data.map((s) => ({ id: s.id, nameEn: s.nameEn })));
      const located = addressesRes.data.data.some((a) => a.isDefault && a.latitude != null && a.longitude != null);
      setHasLocatedAddress(located);
    } catch {
      setError('Failed to load craftsman profile');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, []);

  const saveProfile = async () => {
    await craftsmanApi.updateProfile(form);
    await load();
  };

  const addService = async () => {
    if (!newService.serviceId) return;
    await craftsmanApi.upsertService({ ...newService, isAvailable: true });
    setNewService({ serviceId: '', customPrice: 0 });
    await load();
  };

  if (loading) return <Container sx={{ py: 6 }}><Typography>Loading...</Typography></Container>;
  if (error) return <Container sx={{ py: 6 }}><Alert severity="error">{error}</Alert></Container>;
  if (!profile) return null;

  return (
    <Container maxWidth="lg" sx={{ py: 4 }}>
      <Typography variant="h4" sx={{ fontWeight: 700, mb: 3 }}>Craftsman Portal</Typography>
      {!hasLocatedAddress && (
        <Alert severity="warning" sx={{ mb: 3 }}>
          Add a default address with GPS coordinates so customers can find you in nearby search.{' '}
          <Link component={RouterLink} to="/addresses">Manage addresses</Link>
        </Alert>
      )}
      <Grid container spacing={3}>
        <Grid size={{ xs: 12, md: 6 }}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>Profile</Typography>
              <Box sx={{ display: 'flex', gap: 1, mb: 2 }}>
                <Chip label={`Rating ${profile.rating}`} />
                <Chip label={`${profile.completedJobs} jobs`} />
              </Box>
              <TextField fullWidth label="Specialization" value={form.specialization} onChange={(e) => setForm({ ...form, specialization: e.target.value })} sx={{ mb: 2 }} />
              <TextField fullWidth type="number" label="Years of experience" value={form.yearsOfExperience} onChange={(e) => setForm({ ...form, yearsOfExperience: Number(e.target.value) })} sx={{ mb: 2 }} />
              <TextField fullWidth type="number" label="Service radius (km)" value={form.serviceRadiusKm} onChange={(e) => setForm({ ...form, serviceRadiusKm: Number(e.target.value) })} sx={{ mb: 2 }} />
              <FormControlLabel control={<Switch checked={form.isAvailable} onChange={(e) => setForm({ ...form, isAvailable: e.target.checked })} />} label="Available for bookings" />
              <Button variant="contained" onClick={saveProfile} sx={{ mt: 2 }}>Save profile</Button>
            </CardContent>
          </Card>
        </Grid>
        <Grid size={{ xs: 12, md: 6 }}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>Offered Services</Typography>
              {profile.services.map((s: CraftsmanServiceItem) => (
                <Box key={s.id} sx={{ display: 'flex', justifyContent: 'space-between', py: 1, borderBottom: 1, borderColor: 'divider' }}>
                  <Typography>{s.serviceNameEn}</Typography>
                  <Typography sx={{ fontWeight: 600 }}>{formatCurrency(s.customPrice)}</Typography>
                </Box>
              ))}
              <Box sx={{ display: 'flex', gap: 1, mt: 2 }}>
                <TextField select size="small" label="Service" value={newService.serviceId} onChange={(e) => setNewService({ ...newService, serviceId: e.target.value })} sx={{ flex: 1 }}>
                  {services.map((s) => <MenuItem key={s.id} value={s.id}>{s.nameEn}</MenuItem>)}
                </TextField>
                <TextField size="small" type="number" label="Price" value={newService.customPrice} onChange={(e) => setNewService({ ...newService, customPrice: Number(e.target.value) })} sx={{ width: 100 }} />
                <Button variant="outlined" onClick={addService}>Add</Button>
              </Box>
            </CardContent>
          </Card>
          <Card sx={{ mt: 2 }}>
            <CardContent>
              <Typography variant="h6" gutterBottom>Working Hours</Typography>
              {profile.workingHours.length === 0 && <Typography color="text.secondary">No hours set (defaults 9–18)</Typography>}
              {profile.workingHours.map((h) => (
                <Typography key={h.id} variant="body2">{DAYS[h.dayOfWeek]}: {h.startTime} – {h.endTime}</Typography>
              ))}
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Container>
  );
}
