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
import { useNavigate, useSearchParams } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { resolvePostLoginPath, sanitizeReturnUrl } from '../auth/redirects';

const DEFAULT_ADMIN_PHONE = '+96170000000';

export function AdminLoginPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [params] = useSearchParams();
  const { adminLogin } = useAuth();
  const [email, setEmail] = useState('');
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
      await adminLogin(email.trim(), password, otpCode.trim(), DEFAULT_ADMIN_PHONE);
      navigate(resolvePostLoginPath('admin', returnUrl), { replace: true });
    } catch (err) {
      const message = err instanceof Error ? err.message : 'LOGIN_FAILED';
      if (message === 'NOT_ADMIN') {
        setError(t('login.notAdminAccount'));
      } else if (message === 'INVALID_OTP') {
        setError(t('login.invalidOtp'));
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
        <Typography color="grey.400" sx={{ mb: 4 }}>
          {t('login.adminSubtitle')}
        </Typography>

        <Box component="form" onSubmit={onSubmit} sx={{ display: 'grid', gap: 2 }}>
          {error && <Alert severity="error">{error}</Alert>}
          <TextField
            label={t('login.email')}
            type="email"
            required
            value={email}
            onChange={(e) => setEmail(e.target.value)}
            fullWidth
            sx={{ '& .MuiInputBase-root': { bgcolor: 'grey.800' } }}
          />
          <TextField
            label={t('login.password')}
            type="password"
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
        </Box>
      </Container>
    </Box>
  );
}
