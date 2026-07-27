import { Paper, Typography } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../auth/AuthContext';
import type { PortalRole } from '../auth/types';

type ShellPageProps = {
  portal: PortalRole;
  titleKey: string;
};

export function ShellPage({ portal, titleKey }: ShellPageProps) {
  const { t } = useTranslation();
  const { session } = useAuth();
  const displayName = session ? `${session.firstName} ${session.lastName}`.trim() : '';

  return (
    <Paper sx={{ p: 3 }}>
      <Typography variant="h5" sx={{ fontWeight: 800 }}>
        {t(titleKey)}
      </Typography>
      {displayName && (
        <Typography color="text.secondary" sx={{ mt: 1 }}>
          {t('auth.welcomeUser', { name: displayName })}
        </Typography>
      )}
      <Typography color="text.secondary" sx={{ mt: 2 }}>
        {t('app.tagline')} — {t(`roles.${portal}`)}
      </Typography>
    </Paper>
  );
}
