import { useState } from 'react';
import {
  Alert,
  Box,
  Button,
  Link as MuiLink,
  TextField,
  Typography,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate, useSearchParams } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { savePendingLogin } from '../auth/pendingLogin';
import { resolvePostLoginPath, sanitizeReturnUrl, withReturnUrl } from '../auth/redirects';
import { AuthPageLayout } from '../components/auth/AuthPageLayout';
import { PasswordField } from '../components/auth/PasswordField';

export function LoginPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [params] = useSearchParams();
  const { login } = useAuth();
  const [identifier, setIdentifier] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const returnUrl = sanitizeReturnUrl(params.get('returnUrl'));

  const onSubmit = async (event: React.FormEvent) => {
    event.preventDefault();
    setError(null);
    setSubmitting(true);
    try {
      const result = await login(identifier, password);
      if (result.status === 'ROLE_SELECTION_REQUIRED') {
        savePendingLogin({
          identifier: identifier.trim(),
          password,
          roles: result.roles,
          returnUrl: returnUrl ?? undefined,
        });
        navigate(withReturnUrl('/login/select-account', returnUrl));
        return;
      }
      navigate(resolvePostLoginPath(result.portal, returnUrl), { replace: true });
    } catch (err) {
      const message = err instanceof Error ? err.message : 'LOGIN_FAILED';
      if (message === 'ADMIN_PORTAL_REQUIRED') {
        setError(t('login.useAdminPortal'));
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
    <AuthPageLayout>
      <Button component={Link} to="/" startIcon={<ArrowBackIcon />} sx={{ mb: 3 }}>
        {t('login.browseWithout')}
      </Button>

      <Typography variant="h4" component="h1" sx={{ fontWeight: 800, mb: 1 }} align="center">
        {t('login.title')}
      </Typography>
      <Typography color="text.secondary" align="center" sx={{ mb: 4 }}>
        {t('login.subtitle')}
      </Typography>

      <Box
        component="form"
        onSubmit={onSubmit}
        sx={{
          display: 'grid',
          gap: 2,
          p: { xs: 2, md: 3 },
          borderRadius: 3,
          border: 1,
          borderColor: 'divider',
          bgcolor: 'background.paper',
        }}
      >
        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
            {error === t('login.useAdminPortal') && (
              <Button component={Link} to="/admin/login" size="small" sx={{ mt: 1 }}>
                {t('login.goToAdminLogin')}
              </Button>
            )}
          </Alert>
        )}
        <TextField
          label={t('auth.identifier')}
          autoComplete="username"
          required
          value={identifier}
          onChange={(e) => setIdentifier(e.target.value)}
          fullWidth
          helperText={t('auth.identifierHint')}
        />
        <PasswordField
          label={t('login.password')}
          autoComplete="current-password"
          required
          value={password}
          onChange={(e) => setPassword(e.target.value)}
          fullWidth
        />
        <Box sx={{ display: 'flex', justifyContent: 'flex-end' }}>
          <MuiLink component={Link} to={withReturnUrl('/forgot-password', returnUrl)} variant="body2">
            {t('auth.forgotPassword')}
          </MuiLink>
        </Box>
        <Button type="submit" variant="contained" size="large" disabled={submitting}>
          {submitting ? t('common.loading') : t('login.submit')}
        </Button>
      </Box>

      <Box sx={{ mt: 4, textAlign: 'center' }}>
        <Typography color="text.secondary" sx={{ mb: 1 }}>{t('auth.noAccountYet')}</Typography>
        <Button component={Link} to={withReturnUrl('/register', returnUrl)} variant="outlined" size="large" fullWidth>
          {t('auth.createAccount')}
        </Button>
      </Box>
    </AuthPageLayout>
  );
}
