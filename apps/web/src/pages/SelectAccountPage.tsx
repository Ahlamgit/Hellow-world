import { useEffect, useState } from 'react';
import {
  Alert,
  Box,
  Button,
  FormControlLabel,
  Radio,
  RadioGroup,
  Typography,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import { useNavigate, useSearchParams } from 'react-router-dom';
import { useAuth } from '../auth/AuthContext';
import { apiRoleToRegisterType } from '../auth/passwordValidation';
import { clearPendingLogin, loadPendingLogin } from '../auth/pendingLogin';
import { resolvePostLoginPath, sanitizeReturnUrl } from '../auth/redirects';
import type { ApiRole } from '../auth/types';
import { AuthPageLayout } from '../components/auth/AuthPageLayout';

export function SelectAccountPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();
  const [params] = useSearchParams();
  const { completeLogin } = useAuth();
  const [selectedRole, setSelectedRole] = useState<ApiRole | ''>('');
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const pending = loadPendingLogin();
  const returnUrl = sanitizeReturnUrl(params.get('returnUrl')) ?? pending?.returnUrl ?? null;

  useEffect(() => {
    if (!pending) {
      navigate('/login', { replace: true });
    }
  }, [pending, navigate]);

  if (!pending) {
    return null;
  }

  const onContinue = async () => {
    if (!selectedRole) {
      setError(t('auth.selectAccountType'));
      return;
    }
    setError(null);
    setSubmitting(true);
    try {
      const portal = await completeLogin(pending.identifier, pending.password, selectedRole);
      clearPendingLogin();
      navigate(resolvePostLoginPath(portal, returnUrl), { replace: true });
    } catch {
      setError(t('login.invalidCredentials'));
      clearPendingLogin();
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <AuthPageLayout>
      <Typography variant="h4" component="h1" sx={{ fontWeight: 800, mb: 1 }} align="center">
        {t('auth.selectAccountTitle')}
      </Typography>
      <Typography color="text.secondary" align="center" sx={{ mb: 4 }}>
        {t('auth.selectAccountSubtitle')}
      </Typography>

      <Box sx={{ p: 3, borderRadius: 3, border: 1, borderColor: 'divider', bgcolor: 'background.paper' }}>
        {error && <Alert severity="error" sx={{ mb: 2 }}>{error}</Alert>}
        <RadioGroup
          value={selectedRole}
          onChange={(e) => setSelectedRole(e.target.value as ApiRole)}
          sx={{ gap: 1 }}
        >
          {pending.roles.map((role) => {
            const type = apiRoleToRegisterType(role);
            if (!type) return null;
            return (
              <FormControlLabel
                key={role}
                value={role}
                control={<Radio />}
                label={t(`roles.${type}`)}
                sx={{
                  mx: 0,
                  px: 2,
                  py: 1,
                  borderRadius: 2,
                  border: 1,
                  borderColor: 'divider',
                  width: '100%',
                }}
              />
            );
          })}
        </RadioGroup>
        <Button
          variant="contained"
          size="large"
          fullWidth
          sx={{ mt: 3 }}
          onClick={onContinue}
          disabled={submitting}
        >
          {submitting ? t('common.loading') : t('auth.continue')}
        </Button>
      </Box>
    </AuthPageLayout>
  );
}
