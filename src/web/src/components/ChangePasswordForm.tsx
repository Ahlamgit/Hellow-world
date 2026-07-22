import { useState } from 'react';
import { Alert, Box, Button, TextField } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { authApi } from '../services/api';
import { getApiErrorMessage } from '../utils/apiError';
import { useAuth } from '../context/AuthContext';
import PasswordConfirmFields, { passwordsMatch } from './PasswordConfirmFields';

export default function ChangePasswordForm() {
  const { t } = useTranslation();
  const { logout } = useAuth();
  const [currentPassword, setCurrentPassword] = useState('');
  const [newPassword, setNewPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [error, setError] = useState('');
  const [success, setSuccess] = useState('');
  const [loading, setLoading] = useState(false);

  const canSubmit = currentPassword && passwordsMatch(newPassword, confirmPassword) && newPassword !== currentPassword;

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    setError('');
    setSuccess('');
    if (!passwordsMatch(newPassword, confirmPassword)) {
      setError(t('identity.passwordMismatch'));
      return;
    }
    if (newPassword === currentPassword) {
      setError(t('identity.newPasswordMustDiffer'));
      return;
    }
    setLoading(true);
    try {
      const { data } = await authApi.changePassword(currentPassword, newPassword, confirmPassword);
      setSuccess(data.data.message || t('identity.changePasswordSuccess'));
      setCurrentPassword('');
      setNewPassword('');
      setConfirmPassword('');
      setTimeout(() => logout(), 2000);
    } catch (err) {
      setError(getApiErrorMessage(err, t('common.error')));
    } finally {
      setLoading(false);
    }
  };

  return (
    <Box component="form" onSubmit={handleSubmit} sx={{ display: 'flex', flexDirection: 'column', gap: 2 }}>
      <TextField
        label={t('identity.currentPassword')}
        type="password"
        value={currentPassword}
        onChange={(e) => setCurrentPassword(e.target.value)}
        required
        fullWidth
        autoComplete="current-password"
      />
      <PasswordConfirmFields
        password={newPassword}
        confirmPassword={confirmPassword}
        onPasswordChange={setNewPassword}
        onConfirmPasswordChange={setConfirmPassword}
        passwordLabel={t('identity.newPassword')}
        confirmLabel={t('identity.confirmPassword')}
      />
      {error && <Alert severity="error">{error}</Alert>}
      {success && <Alert severity="success">{success}</Alert>}
      <Button type="submit" variant="contained" disabled={loading || !canSubmit}>
        {loading ? t('common.loading') : t('identity.changePassword')}
      </Button>
    </Box>
  );
}
