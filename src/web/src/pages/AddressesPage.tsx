import { useEffect, useState } from 'react';
import {
  Alert, Box, Button, Card, CardContent, Chip, CircularProgress, Container,
  Grid, IconButton, MenuItem, TextField, Typography,
} from '@mui/material';
import { Delete, Edit } from '@mui/icons-material';
import { useTranslation } from 'react-i18next';
import { usersApi, type AddressDto, type CreateAddressDto } from '../services/api';
import { getApiErrorMessage } from '../utils/apiError';
import { useSaudiLocations } from '../hooks/useSaudiLocations';

const SAUDI_COUNTRY = 'SA';

const emptyForm = (): CreateAddressDto & { regionId?: string } => ({
  label: 'Home',
  street: '',
  city: '',
  district: '',
  postalCode: '',
  country: SAUDI_COUNTRY,
  latitude: undefined,
  longitude: undefined,
  isDefault: false,
  regionId: '',
});

export default function AddressesPage() {
  const { t } = useTranslation();
  const { regions, cities, regionLabel, cityLabel, loadCities } = useSaudiLocations();
  const [addresses, setAddresses] = useState<AddressDto[]>([]);
  const [form, setForm] = useState(emptyForm());
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);
  const [locating, setLocating] = useState(false);
  const [error, setError] = useState('');
  const [editingId, setEditingId] = useState<string | null>(null);
  const [success, setSuccess] = useState('');

  const load = async () => {
    setLoading(true);
    try {
      const { data } = await usersApi.listAddresses();
      setAddresses(data.data);
    } catch (e) {
      setError(getApiErrorMessage(e, t('common.error')));
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => { load(); }, []);

  const handleRegionChange = async (regionId: string) => {
    setForm((prev) => ({ ...prev, regionId, city: '' }));
    await loadCities(regionId);
  };

  const useCurrentLocation = () => {
    if (!navigator.geolocation) {
      setError(t('addresses.geolocationUnsupported'));
      return;
    }
    setLocating(true);
    setError('');
    navigator.geolocation.getCurrentPosition(
      (position) => {
        setForm((prev) => ({
          ...prev,
          latitude: position.coords.latitude,
          longitude: position.coords.longitude,
        }));
        setLocating(false);
      },
      () => {
        setError(t('addresses.geolocationFailed'));
        setLocating(false);
      },
    );
  };

  const resetForm = () => {
    setForm(emptyForm());
    setEditingId(null);
  };

  const handleAdd = async (e: React.FormEvent) => {
    e.preventDefault();
    setSaving(true);
    setError('');
    setSuccess('');
    try {
      const payload: CreateAddressDto = {
        label: form.label,
        street: form.street,
        city: form.city,
        district: form.district,
        postalCode: form.postalCode,
        country: SAUDI_COUNTRY,
        latitude: form.latitude,
        longitude: form.longitude,
        isDefault: form.isDefault,
      };
      if (editingId) {
        await usersApi.updateAddress(editingId, payload);
        setSuccess(t('addresses.updated'));
      } else {
        await usersApi.addAddress(payload);
        setSuccess(t('addresses.saved'));
      }
      resetForm();
      await load();
    } catch (err) {
      setError(getApiErrorMessage(err, t('common.error')));
    } finally {
      setSaving(false);
    }
  };

  const startEdit = async (address: AddressDto) => {
    let regionId = '';
    for (const region of regions) {
      const regionCities = await loadCities(region.id);
      const match = regionCities.find((c) => c.nameEn === address.city || c.nameAr === address.city);
      if (match) {
        regionId = region.id;
        break;
      }
    }
    setEditingId(address.id);
    setForm({
      label: address.label,
      street: address.street,
      city: address.city,
      district: address.district,
      postalCode: address.postalCode,
      country: SAUDI_COUNTRY,
      latitude: address.latitude,
      longitude: address.longitude,
      isDefault: address.isDefault,
      regionId,
    });
    if (regionId) await loadCities(regionId);
    setSuccess('');
    setError('');
  };

  const handleDelete = async (id: string) => {
    setError('');
    try {
      await usersApi.deleteAddress(id);
      await load();
    } catch (err) {
      setError(getApiErrorMessage(err, t('common.error')));
    }
  };

  return (
    <Container maxWidth="md" sx={{ py: 4 }}>
      <Typography variant="h4" sx={{ fontWeight: 700 }} gutterBottom>{t('addresses.title')}</Typography>
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      {success && <Alert severity="success" sx={{ mb: 2 }}>{success}</Alert>}

      <Card sx={{ mb: 3 }}>
        <CardContent>
          <Typography variant="h6" gutterBottom>{editingId ? t('addresses.edit') : t('addresses.add')}</Typography>
          <Box component="form" onSubmit={handleAdd}>
            <Grid container spacing={2}>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField label={t('addresses.label')} value={form.label} onChange={(e) => setForm({ ...form, label: e.target.value })} required fullWidth />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField label={t('addresses.street')} value={form.street} onChange={(e) => setForm({ ...form, street: e.target.value })} required fullWidth />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField label={t('addresses.country')} value={t('locations.saudiArabia')} disabled fullWidth />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField
                  select
                  label={t('identity.region')}
                  value={form.regionId ?? ''}
                  onChange={(e) => handleRegionChange(e.target.value)}
                  required
                  fullWidth
                >
                  {regions.map((region) => (
                    <MenuItem key={region.id} value={region.id}>{regionLabel(region)}</MenuItem>
                  ))}
                </TextField>
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField
                  select
                  label={t('addresses.city')}
                  value={form.city}
                  onChange={(e) => setForm({ ...form, city: e.target.value })}
                  disabled={!form.regionId}
                  required
                  fullWidth
                >
                  {cities.map((city) => (
                    <MenuItem key={city.id} value={cityLabel(city)}>{cityLabel(city)}</MenuItem>
                  ))}
                </TextField>
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField label={t('addresses.district')} value={form.district ?? ''} onChange={(e) => setForm({ ...form, district: e.target.value })} fullWidth />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField label={t('addresses.postalCode')} value={form.postalCode ?? ''} onChange={(e) => setForm({ ...form, postalCode: e.target.value })} fullWidth />
              </Grid>
              {form.latitude != null && form.longitude != null && (
                <Grid size={{ xs: 12 }}>
                  <Typography variant="body2" color="text.secondary">
                    {t('addresses.coordinates')}: {form.latitude.toFixed(5)}, {form.longitude.toFixed(5)}
                  </Typography>
                </Grid>
              )}
              <Grid size={{ xs: 12 }}>
                <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap' }}>
                  <Button type="button" variant="outlined" onClick={useCurrentLocation} disabled={locating}>
                    {locating ? <CircularProgress size={20} /> : t('addresses.useLocation')}
                  </Button>
                  <Button type="submit" variant="contained" disabled={saving}>
                    {saving ? <CircularProgress size={20} /> : (editingId ? t('addresses.update') : t('addresses.save'))}
                  </Button>
                  {editingId && (
                    <Button type="button" variant="text" onClick={resetForm}>{t('common.cancel')}</Button>
                  )}
                </Box>
              </Grid>
            </Grid>
          </Box>
        </CardContent>
      </Card>

      <Typography variant="h6" gutterBottom>{t('addresses.savedList')}</Typography>
      {loading ? (
        <Box sx={{ display: 'flex', justifyContent: 'center', py: 4 }}><CircularProgress /></Box>
      ) : addresses.length === 0 ? (
        <Typography color="text.secondary">{t('addresses.empty')}</Typography>
      ) : (
        <Box sx={{ display: 'flex', flexDirection: 'column', gap: 1 }}>
          {addresses.map((address) => (
            <Card key={address.id}>
              <CardContent sx={{ display: 'flex', justifyContent: 'space-between', gap: 2 }}>
                <Box>
                  <Box sx={{ display: 'flex', gap: 1, alignItems: 'center', mb: 1 }}>
                    <Typography variant="subtitle1" sx={{ fontWeight: 600 }}>{address.label}</Typography>
                    {address.isDefault && <Chip label={t('addresses.default')} size="small" color="primary" />}
                  </Box>
                  <Typography variant="body2">{address.street}, {address.city}</Typography>
                  {address.latitude != null && address.longitude != null && (
                    <Typography variant="caption" color="text.secondary">
                      {address.latitude.toFixed(5)}, {address.longitude.toFixed(5)}
                    </Typography>
                  )}
                </Box>
                <Box sx={{ display: 'flex' }}>
                  <IconButton onClick={() => startEdit(address)} aria-label={t('common.edit')}><Edit /></IconButton>
                  <IconButton color="error" onClick={() => handleDelete(address.id)} aria-label={t('common.delete')}>
                    <Delete />
                  </IconButton>
                </Box>
              </CardContent>
            </Card>
          ))}
        </Box>
      )}
    </Container>
  );
}
