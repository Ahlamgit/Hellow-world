import { TextField } from '@mui/material';
import { useTranslation } from 'react-i18next';

interface PasswordConfirmFieldsProps {
  password: string;
  confirmPassword: string;
  onPasswordChange: (value: string) => void;
  onConfirmPasswordChange: (value: string) => void;
  passwordLabel?: string;
  confirmLabel?: string;
  autoComplete?: 'new-password' | 'current-password';
}

export function passwordsMatch(password: string, confirmPassword: string): boolean {
  return password.length > 0 && password === confirmPassword;
}

export default function PasswordConfirmFields({
  password,
  confirmPassword,
  onPasswordChange,
  onConfirmPasswordChange,
  passwordLabel,
  confirmLabel,
  autoComplete = 'new-password',
}: PasswordConfirmFieldsProps) {
  const { t } = useTranslation();
  const mismatch = !!confirmPassword && password !== confirmPassword;

  return (
    <>
      <TextField
        label={passwordLabel ?? t('auth.password')}
        type="password"
        value={password}
        onChange={(e) => onPasswordChange(e.target.value)}
        required
        fullWidth
        autoComplete={autoComplete}
      />
      <TextField
        label={confirmLabel ?? t('auth.confirmPassword')}
        type="password"
        value={confirmPassword}
        onChange={(e) => onConfirmPasswordChange(e.target.value)}
        required
        fullWidth
        autoComplete="new-password"
        error={mismatch}
        helperText={mismatch ? t('identity.passwordMismatch') : ' '}
      />
    </>
  );
}
