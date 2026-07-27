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
import { AdminAuthRequestError } from '../auth/adminAuthApi';
import { useAdminAuth } from '../auth/AdminAuthContext';
import {
  clearPendingAdminLogin,
  loadPendingAdminLogin,
  savePendingAdminLogin,
} from '../auth/pendingAdminLogin';
import { sanitizeReturnUrl } from '../auth/redirects';
import { PasswordField } from '../components/auth/PasswordField';

type Step = 'credentials' | 'mfa';

export function AdminLoginPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [params] = useSearchParams();
  const { login, verifyMfa } = useAdminAuth();

  const returnUrl = sanitizeReturnUrl(params.get('returnUrl'));
  const pending = loadPendingAdminLogin();

  const [step, setStep] = useState<Step>(pending ? 'mfa' : 'credentials');
  const [email, setEmail] = useState(pending?.email ?? '');
  const [password, setPassword] = useState('');
  const [otpCode, setOtpCode] = useState('');
  const [info, setInfo] = useState<string | null>(null);
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const onCredentialsSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError(null);
    setInfo(null);
    setSubmitting(true);
    try {
      const result = await login(email.trim(), password);
      savePendingAdminLogin({
        email: result.email,
        phoneE164: result.phoneE164,
        returnUrl: returnUrl ?? undefined,
      });
      setEmail(result.email);
      setInfo(t('login.adminMfaSent'));
      setStep('mfa');
    } catch (err) {
      if (err instanceof AdminAuthRequestError) {
        setError(err.message || t('login.invalidCredentials'));
      } else {
        setError(t('login.invalidCredentials'));
      }
    } finally {
      setSubmitting(false);
    }
  };

  const onMfaSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    const stored = loadPendingAdminLogin();
    if (!stored) {
      setStep('credentials');
      return;
    }
    setError(null);
    setSubmitting(true);
    try {
      await verifyMfa(stored.email, otpCode.trim());
      clearPendingAdminLogin();
      const target =
        returnUrl && returnUrl.startsWith('/admin') ? returnUrl : '/admin/dashboard';
      navigate(target, { replace: true });
    } catch (err) {
      if (err instanceof AdminAuthRequestError) {
        if (err.code === 'VERIFICATION_CODE_EXPIRED') {
          setError(t('auth.verificationExpired'));
        } else if (err.code === 'VERIFICATION_ATTEMPTS_EXCEEDED') {
          setError(t('auth.verificationAttemptsExceeded'));
        } else {
          setError(err.message || t('login.invalidOtp'));
        }
      } else {
        setError(t('login.invalidOtp'));
      }
    } finally {
      setSubmitting(false);
    }
  };

  const storedPending = loadPendingAdminLogin();

  return (
    <Box sx={{ minHeight: '100vh', bgcolor: 'grey.900', color: 'grey.100' }}>
      <Container maxWidth="sm" sx={{ py: 8 }}>
        <Typography variant="h4" component="h1" sx={{ fontWeight: 800, mb: 1 }}>
          {t('login.adminTitle')}
        </Typography>
        <Typography color="grey.400" sx={{ mb: 2 }}>
          {step === 'credentials' ? t('login.adminSubtitle') : t('login.adminMfaSubtitle')}
        </Typography>
        <Alert severity="info" sx={{ mb: 3, bgcolor: 'grey.800', color: 'grey.100' }}>
          {t('login.adminSteps')}
        </Alert>

        {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
        {info && <Alert severity="success" sx={{ mb: 2 }}>{info}</Alert>}

        {step === 'credentials' && (
          <Box component="form" onSubmit={onCredentialsSubmit} sx={{ display: 'grid', gap: 2 }}>
            <TextField
              label={t('login.email')}
              type="email"
              required
              value={email}
              onChange={(e) => setEmail(e.target.value)}
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
            <Button type="submit" variant="contained" size="large" disabled={submitting}>
              {submitting ? t('common.loading') : t('login.adminContinue')}
            </Button>
          </Box>
        )}

        {step === 'mfa' && storedPending && (
          <Box component="form" onSubmit={onMfaSubmit} sx={{ display: 'grid', gap: 2 }}>
            <Typography color="grey.400">{storedPending.phoneE164}</Typography>
            <TextField
              label={t('login.otp')}
              required
              value={otpCode}
              onChange={(e) => setOtpCode(e.target.value)}
              helperText={t('login.otpHint')}
              fullWidth
              inputProps={{ inputMode: 'numeric', autoComplete: 'one-time-code' }}
              sx={{ '& .MuiInputBase-root': { bgcolor: 'grey.800' } }}
            />
            <Button type="submit" variant="contained" size="large" disabled={submitting}>
              {submitting ? t('common.loading') : t('login.adminSubmit')}
            </Button>
            <Button
              type="button"
              color="inherit"
              onClick={() => {
                clearPendingAdminLogin();
                setStep('credentials');
                setOtpCode('');
              }}
            >
              {t('common.back')}
            </Button>
          </Box>
        )}

        <Button component={Link} to="/login" color="inherit" fullWidth sx={{ mt: 3 }}>
          {t('login.backToPublicLogin')}
        </Button>
      </Container>
    </Box>
  );
}
