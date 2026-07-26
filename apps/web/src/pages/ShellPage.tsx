import { Paper, Typography } from '@mui/material';
import { useTranslation } from 'react-i18next';
import type { ActorType } from '../auth/actor';

type ShellPageProps = {
  actor: ActorType;
  titleKey: string;
};

export function ShellPage({ actor, titleKey }: ShellPageProps) {
  const { t } = useTranslation();
  return (
    <Paper sx={{ p: 3 }}>
      <Typography variant="h5" gutterBottom>
        {t(titleKey)}
      </Typography>
      <Typography color="text.secondary">
        {t('app.tagline')} — {t(`login.${actor}`)}
      </Typography>
      <Typography variant="caption" sx={{ mt: 2, display: 'block' }}>
        {t('login.shellNote')}
      </Typography>
    </Paper>
  );
}
