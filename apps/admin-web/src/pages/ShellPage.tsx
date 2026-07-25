import { Typography, Paper } from '@mui/material';
import { useTranslation } from 'react-i18next';

type ShellPageProps = {
  titleKey: string;
};

export function ShellPage({ titleKey }: ShellPageProps) {
  const { t } = useTranslation();
  return (
    <Paper sx={{ p: 3 }}>
      <Typography variant="h5" gutterBottom>
        {t(titleKey)}
      </Typography>
      <Typography color="text.secondary">{t('app.tagline')}</Typography>
      <Typography variant="caption" sx={{ mt: 2, display: 'block' }}>
        Sprint 0 shell — placeholder route
      </Typography>
    </Paper>
  );
}
