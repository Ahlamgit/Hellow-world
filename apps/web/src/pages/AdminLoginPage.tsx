import { useState } from 'react';
import {
  Alert,
  Box,
  Button,
  Container,
  TextField,
  Typography,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { resolvePostLoginPath, sanitizeReturnUrl } from '../auth/redirects';
import { AuthPageLayout } from '../components/auth/AuthPageLayout';
import { PasswordField } from '../components/auth/PasswordField';

const DEFAULT_ADMIN_PHONE = '+96170000000';

export function AdminLoginPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [params] = useSearchParams();
  const { adminLogin } = useAuth();
  const [identifier, setIdentifier] = useState('');
  const [password, setPassword] = useState('');
  const [otpCode, setOtpCode] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const returnUrl = sanitizeReturnUrl(params.get('returnUrl'));

  const onSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError(null);
    setSubmitting(true);
    try {
      await adminLogin(identifier.trim(), password, otpCode.trim(), DEFAULT_ADMIN_PHONE);
      navigate(resolvePostLoginPath('admin', returnUrl), { replace: true });
    } catch (err) {
      const message = err instanceof Error ? err.message : 'LOGIN_FAILED';
      if (message === 'NOT_ADMIN') {
        setError(t('login.notAdminAccount'));
      } else if (message === 'INVALID_OTP') {
        setError(t('login.invalidOtp'));
      } else if (message === 'Request failed' || message.includes('fetch')) {
        setError(t('auth.apiUnavailable'));
      } else {
        setError(t('login.invalidCredentials'));
      }
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Box sx={{ minHeight: '100vh', bgcolor: 'grey.900', color: 'grey.100' }}>
      <Container maxWidth="sm" sx={{ py: 8 }}>
        <Typography variant="h4" component="h1" sx={{ fontWeight: 800, mb: 1 }}>
          {t('login.adminTitle')}
        </Typography>
        <Typography color="grey.400" sx={{ mb: 2 }}>
          {t('login.adminSubtitle')}
        </Typography>
        <Alert severity="info" sx={{ mb: 3, bgcolor: 'grey.800', color: 'grey.100' }}>
          {t('login.adminSteps')}
        </Alert>

        <Box component="form" onSubmit={onSubmit} sx={{ display: 'grid', gap: 2 }}>
          {error && <Alert severity="error">{error}</Alert>}
          <TextField
            label={t('auth.identifier')}
            required
            value={identifier}
            onChange={(e) => setIdentifier(e.target.value)}
            fullWidth
            helperText={t('login.adminEmailHint')}
            sx={{ '& .MuiInputBase-root': { bgcolor: 'grey.800' } }}
          />
          <PasswordField
            label={t('login.password')}
            required
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            fullWidth
            sx={{ '& .MuiInputBase-root': { bgcolor: 'grey.800' } }}
          />
          <TextField
            label={t('login.otp')}
            required
            value={otpCode}
            onChange={(e) => setOtpCode(e.target.value)}
            helperText={t('login.otpHint')}
            fullWidth
            sx={{ '& .MuiInputBase-root': { bgcolor: 'grey.800' } }}
          />
          <Button type="submit" variant="contained" size="large" disabled={submitting}>
            {submitting ? t('common.loading') : t('login.adminSubmit')}
          </Button>
          <Button component={Link} to="/login" color="inherit" fullWidth>
            {t('login.backToPublicLogin')}
          </Button>
        </Box>
      </Container>
    </Box>
  );
}
