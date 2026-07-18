import { useState } from 'react';
import { Box, Card, CardContent, TextField, Button, Typography, Alert, MenuItem } from '@mui/material';
import { Link, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../context/AuthContext';
import { canAccessAdmin } from '../utils/permissions';

export default function LoginPage() {
  const { t } = useTranslation();
  const { login } = useAuth();
  const navigate = useNavigate();
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await login(email, password);
      const stored = localStorage.getItem('user');
      const loggedInUser = stored ? JSON.parse(stored) as { permissions?: string[] } : user;
      navigate(canAccessAdmin(loggedInUser) ? '/admin' : '/dashboard');
    } catch {
      setError(t('common.error'));
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '70vh', px: 2 }}>
      <Card sx={{ width: '100%', maxWidth: 440 }}>
        <CardContent sx={{ p: 4 }}>
          <Typography variant="h5" sx={{ fontWeight: 700 }} gutterBottom>{t('auth.loginTitle')}</Typography>
          {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
          <Box component="form" onSubmit={handleSubmit} sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
            <TextField label={t('auth.email')} type="email" value={email} onChange={(e) => setEmail(e.target.value)} required fullWidth />
            <TextField label={t('auth.password')} type="password" value={password} onChange={(e) => setPassword(e.target.value)} required fullWidth />
            <Box sx={{ textAlign: 'right' }}>
              <Link to="/forgot-password">{t('identity.forgotPassword')}</Link>
            </Box>
            <Button type="submit" variant="contained" size="large" disabled={loading}>
              {loading ? t('common.loading') : t('nav.login')}
            </Button>
          </Box>
          <Typography variant="body2" sx={{ mt: 2, textAlign: 'center' }}>
            {t('auth.noAccount')} <Link to="/register">{t('nav.register')}</Link>
          </Typography>
        </CardContent>
      </Card>
    </Box>
  );
}

export function RegisterPage() {
  const { t } = useTranslation();
  const { register } = useAuth();
  const navigate = useNavigate();
  const [form, setForm] = useState({
    email: '', phone: '', password: '', firstName: '', lastName: '', role: 'Customer', preferredLanguage: 'ar',
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setLoading(true);
    try {
      await register(form);
      navigate('/dashboard');
    } catch {
      setError(t('common.error'));
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box sx={{ display: 'flex', justifyContent: 'center', alignItems: 'center', minHeight: '70vh', px: 2, py: 4 }}>
      <Card sx={{ width: '100%', maxWidth: 480 }}>
        <CardContent sx={{ p: 4 }}>
          <Typography variant="h5" sx={{ fontWeight: 700 }} gutterBottom>{t('auth.registerTitle')}</Typography>
          {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
          <Box component="form" onSubmit={handleSubmit} sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
            <TextField label={t('auth.firstName')} value={form.firstName} onChange={(e) => setForm({ ...form, firstName: e.target.value })} required fullWidth />
            <TextField label={t('auth.lastName')} value={form.lastName} onChange={(e) => setForm({ ...form, lastName: e.target.value })} required fullWidth />
            <TextField label={t('auth.email')} type="email" value={form.email} onChange={(e) => setForm({ ...form, email: e.target.value })} required fullWidth />
            <TextField label={t('auth.phone')} value={form.phone} onChange={(e) => setForm({ ...form, phone: e.target.value })} required fullWidth />
            <TextField label={t('auth.password')} type="password" value={form.password} onChange={(e) => setForm({ ...form, password: e.target.value })} required fullWidth />
            <TextField select label={t('auth.role')} value={form.role} onChange={(e) => setForm({ ...form, role: e.target.value })} fullWidth>
              <MenuItem value="Customer">{t('auth.customer')}</MenuItem>
              <MenuItem value="Craftsman">{t('auth.craftsman')}</MenuItem>
              <MenuItem value="Store">{t('auth.store')}</MenuItem>
            </TextField>
            <Button type="submit" variant="contained" size="large" disabled={loading}>
              {loading ? t('common.loading') : t('nav.register')}
            </Button>
          </Box>
          <Typography variant="body2" sx={{ mt: 2, textAlign: 'center' }}>
            {t('auth.hasAccount')} <Link to="/login">{t('nav.login')}</Link>
          </Typography>
        </CardContent>
      </Card>
    </Box>
  );
}
