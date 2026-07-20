import { useEffect, useState } from 'react';
import {
  Container, Typography, Card, CardContent, Grid, TextField, Button, Alert, MenuItem, Chip, Box,
} from '@mui/material';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { identityApi, type ProfileDto } from '../services/api';
import { getApiErrorMessage } from '../utils/apiError';
import { useAuth } from '../context/AuthContext';
import { useLocationCatalog } from '../hooks/useLocationCatalog';

const emptyForm = {
  firstName: '',
  lastName: '',
  gender: '',
  birthDate: '',
  nationality: '',
  profilePictureUrl: '',
  addressLine: '',
  country: '',
  city: '',
  region: '',
  preferredLanguage: 'ar',
  timezone: 'Asia/Riyadh',
};

export default function ProfilePage() {
  const { t } = useTranslation();
  const { user, refreshUser } = useAuth();
  const { regions, cities, loading: locationsLoading, error: locationsError, regionLabel, cityLabel, loadCities, findRegionByName } = useLocationCatalog();
  const [form, setForm] = useState(emptyForm);
  const [selectedRegionId, setSelectedRegionId] = useState('');
  const [profile, setProfile] = useState<ProfileDto | null>(null);
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(true);
  const [saving, setSaving] = useState(false);

  useEffect(() => {
    (async () => {
      try {
        const { data } = await identityApi.getProfile();
        const p = data.data;
        setProfile(p);
        setForm({
          firstName: p.firstName,
          lastName: p.lastName,
          gender: p.gender ?? '',
          birthDate: p.birthDate ? p.birthDate.slice(0, 10) : '',
          nationality: p.nationality ?? '',
          profilePictureUrl: p.profilePictureUrl ?? '',
          addressLine: p.addressLine ?? '',
          country: p.country ?? '',
          city: p.city ?? '',
          region: p.region ?? '',
          preferredLanguage: p.preferredLanguage,
          timezone: p.timezone,
        });
      } catch (err) {
        setError(getApiErrorMessage(err, t('common.error')));
      } finally {
        setLoading(false);
      }
    })();
  }, [t]);

  useEffect(() => {
    if (!regions.length || !form.region) return;
    const region = findRegionByName(form.region);
    if (region) {
      setSelectedRegionId(region.id);
      loadCities(region.id);
    }
  }, [regions, form.region, findRegionByName, loadCities]);

  const handleRegionChange = async (regionId: string) => {
    const region = regions.find((r) => r.id === regionId);
    setSelectedRegionId(regionId);
    setForm((prev) => ({
      ...prev,
      region: region ? regionLabel(region) : '',
      city: '',
      country: region?.code ?? '',
    }));
    await loadCities(regionId);
  };

  const handleCityChange = (cityName: string) => {
    setForm((prev) => ({ ...prev, city: cityName }));
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setSuccess('');
    setSaving(true);
    try {
      const { data } = await identityApi.updateProfile({
        firstName: form.firstName,
        lastName: form.lastName,
        gender: form.gender || undefined,
        birthDate: form.birthDate || undefined,
        nationality: form.nationality || undefined,
        profilePictureUrl: form.profilePictureUrl || undefined,
        addressLine: form.addressLine || undefined,
        country: form.country || undefined,
        city: form.city || undefined,
        region: form.region || undefined,
        preferredLanguage: form.preferredLanguage,
        timezone: form.timezone,
      });
      setProfile(data.data);
      setSuccess(t('identity.profileSaved'));
      await refreshUser();
    } catch (err) {
      setError(getApiErrorMessage(err, t('common.error')));
    } finally {
      setSaving(false);
    }
  };

  if (loading) {
    return (
      <Container maxWidth="md" sx={{ py: 6 }}>
        <Typography>{t('common.loading')}</Typography>
      </Container>
    );
  }

  return (
    <Container maxWidth="md" sx={{ py: 6 }}>
      <Typography variant="h4" sx={{ fontWeight: 700 }} gutterBottom>{t('nav.profile')}</Typography>

      {profile && (
        <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap', mb: 2 }}>
          <Chip label={profile.email} size="small" />
          <Chip label={profile.phone} size="small" />
          <Chip
            label={profile.emailVerified ? t('identity.emailVerified') : t('identity.emailNotVerified')}
            color={profile.emailVerified ? 'success' : 'warning'}
            size="small"
          />
          <Chip
            label={profile.phoneVerified ? t('identity.phoneVerified') : t('identity.phoneNotVerified')}
            color={profile.phoneVerified ? 'success' : 'default'}
            size="small"
          />
        </Box>
      )}

      {!profile?.emailVerified && (
        <Alert severity="warning" sx={{ mb: 2 }}>
          {t('identity.verifyEmailPrompt')}{' '}
          <Link to="/verify-email">{t('identity.verifyEmail')}</Link>
        </Alert>
      )}

      {locationsError && <Alert severity="warning" sx={{ mb: 2 }}>{t('locations.loadError')}</Alert>}
      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      {success && <Alert severity="success" sx={{ mb: 2 }}>{success}</Alert>}

      <Card>
        <CardContent>
          <Box component="form" onSubmit={handleSubmit}>
            <Grid container spacing={2}>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField label={t('auth.firstName')} value={form.firstName} onChange={(e) => setForm({ ...form, firstName: e.target.value })} required fullWidth />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField label={t('auth.lastName')} value={form.lastName} onChange={(e) => setForm({ ...form, lastName: e.target.value })} required fullWidth />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField select label={t('identity.gender')} value={form.gender} onChange={(e) => setForm({ ...form, gender: e.target.value })} fullWidth>
                  <MenuItem value="">{t('identity.notSpecified')}</MenuItem>
                  <MenuItem value="Male">{t('identity.male')}</MenuItem>
                  <MenuItem value="Female">{t('identity.female')}</MenuItem>
                </TextField>
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField label={t('identity.birthDate')} type="date" value={form.birthDate} onChange={(e) => setForm({ ...form, birthDate: e.target.value })} fullWidth slotProps={{ inputLabel: { shrink: true } }} />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField label={t('identity.nationality')} value={form.nationality} onChange={(e) => setForm({ ...form, nationality: e.target.value })} fullWidth />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField label={t('identity.profilePictureUrl')} value={form.profilePictureUrl} onChange={(e) => setForm({ ...form, profilePictureUrl: e.target.value })} fullWidth />
              </Grid>
              <Grid size={12}>
                <TextField label={t('identity.addressLine')} value={form.addressLine} onChange={(e) => setForm({ ...form, addressLine: e.target.value })} fullWidth />
              </Grid>
              <Grid size={{ xs: 12, sm: 4 }}>
                <TextField
                  select
                  label={t('identity.region')}
                  value={selectedRegionId}
                  onChange={(e) => handleRegionChange(e.target.value)}
                  disabled={locationsLoading || !regions.length}
                  fullWidth
                >
                  <MenuItem value="">{t('locations.selectRegion')}</MenuItem>
                  {regions.map((region) => (
                    <MenuItem key={region.id} value={region.id}>{regionLabel(region)}</MenuItem>
                  ))}
                </TextField>
              </Grid>
              <Grid size={{ xs: 12, sm: 4 }}>
                <TextField
                  select
                  label={t('identity.city')}
                  value={form.city}
                  onChange={(e) => handleCityChange(e.target.value)}
                  disabled={!selectedRegionId || locationsLoading}
                  fullWidth
                >
                  <MenuItem value="">{t('locations.selectCity')}</MenuItem>
                  {cities.map((city) => (
                    <MenuItem key={city.id} value={cityLabel(city)}>{cityLabel(city)}</MenuItem>
                  ))}
                </TextField>
              </Grid>
              <Grid size={{ xs: 12, sm: 4 }}>
                <TextField label={t('identity.country')} value={form.country} disabled fullWidth helperText={t('locations.countryFromRegion')} />
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField select label={t('identity.language')} value={form.preferredLanguage} onChange={(e) => setForm({ ...form, preferredLanguage: e.target.value })} fullWidth>
                  <MenuItem value="ar">العربية</MenuItem>
                  <MenuItem value="en">English</MenuItem>
                </TextField>
              </Grid>
              <Grid size={{ xs: 12, sm: 6 }}>
                <TextField label={t('identity.timezone')} value={form.timezone} onChange={(e) => setForm({ ...form, timezone: e.target.value })} fullWidth />
              </Grid>
              <Grid size={12}>
                <Box sx={{ display: 'flex', gap: 2, flexWrap: 'wrap' }}>
                  <Button type="submit" variant="contained" disabled={saving}>
                    {saving ? t('common.loading') : t('common.save')}
                  </Button>
                  <Button component={Link} to="/change-password" variant="outlined">{t('identity.changePassword')}</Button>
                  <Button component={Link} to="/sessions" variant="outlined">{t('identity.sessionsTitle')}</Button>
                  <Button component={Link} to="/addresses" variant="outlined">{t('addresses.title')}</Button>
                  <Button component={Link} to="/chat" variant="outlined">{t('chat.title')}</Button>
                </Box>
              </Grid>
            </Grid>
          </Box>
        </CardContent>
      </Card>

      {user && (
        <Typography variant="caption" color="text.secondary" sx={{ mt: 2, display: 'block' }}>
          {t('identity.accountRole')}: {user.role || user.primaryRole}
        </Typography>
      )}
    </Container>
  );
}
