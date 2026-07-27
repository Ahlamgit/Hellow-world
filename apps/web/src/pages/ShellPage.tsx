import { Paper, Typography } from '@mui/material';
import { useTranslation } from 'react-i18next';
import type { PortalRole } from '../auth/types';

type ShellPageProps = {
  portal: PortalRole;
  titleKey: string;
};

export function ShellPage({ portal, titleKey }: ShellPageProps) {
  const { t } = useTranslation();
  return (
    <Paper sx={{ p: 3 }}>
      <Typography variant="h5">{t(titleKey)}</Typography>
      <Typography color="text.secondary">
        {t('app.tagline')} — {t(`roles.${portal}`)}
      </Typography>
      <Typography variant="caption" sx={{ mt: 2, display: 'block' }}>
        {t('login.shellNote')}
      </Typography>
    </Paper>
  );
}
