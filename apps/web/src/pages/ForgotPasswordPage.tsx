import { useState } from 'react';
import { Alert, Box, Button, TextField, Typography } from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import { forgotPasswordRequest } from '../auth/authApi';
import { AuthPageLayout } from '../components/auth/AuthPageLayout';

export function ForgotPasswordPage() {
  const { t } = useTranslation();
  const [identifier, setIdentifier] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState(false);
  const [submitting, setSubmitting] = useState(false);

  const onSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError(null);
    setSubmitting(true);
    try {
      await forgotPasswordRequest(identifier);
      setSuccess(true);
    } catch {
      setError(t('auth.forgotPasswordNotFound'));
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <AuthPageLayout>
      <Button component={Link} to="/login" startIcon={<ArrowBackIcon />} sx={{ mb: 3 }}>
        {t('common.back')}
      </Button>

      <Typography variant="h4" component="h1" sx={{ fontWeight: 800, mb: 1 }} align="center">
        {t('auth.forgotPassword')}
      </Typography>
      <Typography color="text.secondary" align="center" sx={{ mb: 4 }}>
        {t('auth.forgotPasswordSubtitle')}
      </Typography>

      {success ? (
        <Alert severity="success">{t('auth.forgotPasswordSuccess')}</Alert>
      ) : (
        <Box
          component="form"
          onSubmit={onSubmit}
          sx={{ p: 3, borderRadius: 3, border: 1, borderColor: 'divider', bgcolor: 'background.paper', display: 'grid', gap: 2 }}
        >
          {error && <Alert severity="error">{error}</Alert>}
          <TextField
            label={t('auth.identifier')}
            required
            value={identifier}
            onChange={(e) => setIdentifier(e.target.value)}
            fullWidth
            helperText={t('auth.identifierHint')}
          />
          <Button type="submit" variant="contained" size="large" disabled={submitting}>
            {submitting ? t('common.loading') : t('auth.sendResetLink')}
          </Button>
        </Box>
      )}
    </AuthPageLayout>
  );
}
