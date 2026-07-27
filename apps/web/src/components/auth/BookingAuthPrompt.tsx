import {
  Button,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Typography,
} from '@mui/material';
import { Link as RouterLink } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { withReturnUrl } from '../../auth/redirects';

type BookingAuthPromptProps = {
  open: boolean;
  onClose: () => void;
  returnUrl: string;
};

export function BookingAuthPrompt({ open, onClose, returnUrl }: BookingAuthPromptProps) {
  const { t } = useTranslation();
  const loginUrl = withReturnUrl('/login', returnUrl);
  const registerUrl = withReturnUrl('/register', returnUrl);

  return (
    <Dialog open={open} onClose={onClose} fullWidth maxWidth="xs">
      <DialogTitle sx={{ fontWeight: 800 }}>{t('auth.bookingTitle')}</DialogTitle>
      <DialogContent>
        <Typography color="text.secondary">{t('auth.bookingMessage')}</Typography>
      </DialogContent>
      <DialogActions sx={{ flexDirection: 'column', alignItems: 'stretch', px: 3, pb: 3, gap: 1 }}>
        <Button component={RouterLink} to={loginUrl} variant="contained" size="large" fullWidth onClick={onClose}>
          {t('nav.signIn')}
        </Button>
        <Typography variant="body2" color="text.secondary" align="center" sx={{ pt: 1 }}>
          {t('auth.noAccountYet')}
        </Typography>
        <Button component={RouterLink} to={registerUrl} variant="outlined" size="large" fullWidth onClick={onClose}>
          {t('auth.createAccount')}
        </Button>
        <Button onClick={onClose} fullWidth>{t('common.back')}</Button>
      </DialogActions>
    </Dialog>
  );
}
