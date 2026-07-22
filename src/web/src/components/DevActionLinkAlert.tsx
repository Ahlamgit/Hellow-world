import { Alert, Link as MuiLink } from '@mui/material';
import { useTranslation } from 'react-i18next';

export function DevActionLinkAlert({ link, label }: { link?: string | null; label?: string }) {
  const { t } = useTranslation();
  if (!link) return null;
  return (
    <Alert severity="info" sx={{ mb: 2 }}>
      {label ?? t('identity.devActionLinkHint')}{' '}
      <MuiLink href={link} target="_blank" rel="noopener noreferrer" sx={{ wordBreak: 'break-all' }}>{link}</MuiLink>
    </Alert>
  );
}
