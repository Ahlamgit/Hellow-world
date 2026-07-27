import { useState } from 'react';
import {
  Alert,
  Box,
  Button,
  TextField,
  Typography,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { useTranslation } from 'react-i18next';
import { Link, Navigate, useNavigate } from 'react-router-dom';
import { AuthRequestError, resendRegistrationOtpRequest } from '../auth/authApi';
import { useAuth } from '../auth/AuthContext';
import {
  clearPendingRegistration,
  loadPendingRegistration,
} from '../auth/pendingRegistration';
import { resolvePostLoginPath } from '../auth/redirects';
import { AuthPageLayout } from '../components/auth/AuthPageLayout';

export function RegisterVerifyPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const { authStatus, verifyRegistrationOtp, isAuthenticated, portal } = useAuth();

  const pending = loadPendingRegistration();
  const [otpCode, setOtpCode] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [info, setInfo] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);
  const [resending, setResending] = useState(false);

  if (authStatus === 'INITIALIZING') {
    return null;
  }

  if (isAuthenticated && portal) {
    return <Navigate to={resolvePostLoginPath(portal, pending?.returnUrl ?? null)} replace />;
  }

  if (!pending) {
    return <Navigate to="/register" replace />;
  }

  const onSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError(null);
    setInfo(null);
    setSubmitting(true);
    try {
      const targetPortal = await verifyRegistrationOtp(pending.email, pending.role, otpCode.trim());
      clearPendingRegistration();
      navigate(resolvePostLoginPath(targetPortal, pending.returnUrl ?? null), { replace: true });
    } catch (err) {
      if (err instanceof AuthRequestError) {
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

  const onResend = async () => {
    setError(null);
    setInfo(null);
    setResending(true);
    try {
      await resendRegistrationOtpRequest(pending.email, pending.role);
      setInfo(t('auth.verificationResent'));
    } catch (err) {
      setError(err instanceof Error ? err.message : t('common.error'));
    } finally {
      setResending(false);
    }
  };

  return (
    <AuthPageLayout maxWidth="sm">
      <Button component={Link} to="/register" startIcon={<ArrowBackIcon />} sx={{ mb: 3 }}>
        {t('common.back')}
      </Button>

      <Typography variant="h4" component="h1" sx={{ fontWeight: 800, mb: 1 }} align="center">
        {t('auth.verifyAccountTitle')}
      </Typography>
      <Typography color="text.secondary" align="center" sx={{ mb: 2 }}>
        {t('auth.verificationSent')}
      </Typography>
      <Typography color="text.secondary" align="center" sx={{ mb: 4 }}>
        {pending.phoneE164}
      </Typography>

      {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
      {info && <Alert severity="success" sx={{ mb: 2 }}>{info}</Alert>}

      <Box
        component="form"
        onSubmit={onSubmit}
        sx={{
          p: 3,
          borderRadius: 3,
          border: 1,
          borderColor: 'divider',
          bgcolor: 'background.paper',
          display: 'grid',
          gap: 2,
        }}
      >
        <TextField
          label={t('login.otp')}
          required
          value={otpCode}
          onChange={(e) => setOtpCode(e.target.value)}
          helperText={t('login.otpHint')}
          fullWidth
          inputProps={{ inputMode: 'numeric', autoComplete: 'one-time-code' }}
        />
        <Button type="submit" variant="contained" size="large" disabled={submitting}>
          {submitting ? t('common.loading') : t('auth.activateAccount')}
        </Button>
        <Button type="button" variant="text" disabled={resending} onClick={onResend}>
          {resending ? t('common.loading') : t('auth.resendVerification')}
        </Button>
      </Box>
    </AuthPageLayout>
  );
}
