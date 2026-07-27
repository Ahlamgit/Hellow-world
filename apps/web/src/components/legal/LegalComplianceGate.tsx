import { useCallback, useEffect, useState } from 'react';
import {
  Alert,
  Box,
  Button,
  Checkbox,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  FormControlLabel,
  Typography,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import { Link } from 'react-router-dom';
import { portalDashboardPath } from '../../auth/redirects';
import { useAuth } from '../../auth/AuthContext';
import {
  acceptLegalDocuments,
  fetchLegalCompliance,
  fetchLegalDocument,
  type LegalCompliance,
} from '../../auth/legalApi';

type LegalComplianceGateProps = {
  portal: 'customer' | 'provider' | 'store';
};

export function LegalComplianceGate({ portal }: LegalComplianceGateProps) {
  const { t, i18n } = useTranslation();
  const { session } = useAuth();
  const lang = i18n.language.startsWith('ar') ? 'ar' : 'en';

  const [compliance, setCompliance] = useState<LegalCompliance | null>(null);
  const [termsVersion, setTermsVersion] = useState('');
  const [privacyVersion, setPrivacyVersion] = useState('');
  const [acceptTerms, setAcceptTerms] = useState(false);
  const [acceptPrivacy, setAcceptPrivacy] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const loadCompliance = useCallback(async () => {
    if (!session?.accessToken) return;
    try {
      const result = await fetchLegalCompliance(session.accessToken, lang);
      setCompliance(result);
      if (!result.compliant) {
        const terms = await fetchLegalDocument('terms', lang);
        const privacy = await fetchLegalDocument('privacy', lang);
        setTermsVersion(terms.version);
        setPrivacyVersion(privacy.version);
      }
    } catch {
      setError(t('legal.complianceCheckError'));
    }
  }, [session?.accessToken, lang, t]);

  useEffect(() => {
    void loadCompliance();
  }, [loadCompliance]);

  const termsPending = compliance?.pending.some((p) => p.documentType === 'TERMS') ?? false;
  const privacyPending = compliance?.pending.some((p) => p.documentType === 'PRIVACY') ?? false;
  const canSubmit =
    compliance &&
    !compliance.compliant &&
    (!termsPending || acceptTerms) &&
    (!privacyPending || acceptPrivacy) &&
    termsVersion &&
    privacyVersion;

  const onAccept = async () => {
    if (!session?.accessToken || !canSubmit) return;
    setError(null);
    setSubmitting(true);
    try {
      const result = await acceptLegalDocuments(
        session.accessToken,
        termsVersion,
        privacyVersion,
        lang,
      );
      setCompliance(result);
      setAcceptTerms(false);
      setAcceptPrivacy(false);
    } catch (err) {
      setError(err instanceof Error ? err.message : t('common.error'));
    } finally {
      setSubmitting(false);
    }
  };

  if (!compliance || compliance.compliant) {
    return null;
  }

  const returnUrl = portalDashboardPath(portal);

  return (
    <Dialog open fullScreen>
      <DialogTitle sx={{ fontWeight: 800 }}>{t('legal.updateRequiredTitle')}</DialogTitle>
      <DialogContent>
        <Typography color="text.secondary" sx={{ mb: 2 }}>
          {t('legal.updateRequiredBody')}
        </Typography>

        {error && (
          <Alert severity="error" sx={{ mb: 2 }}>
            {error}
          </Alert>
        )}

        <Box sx={{ display: 'flex', flexDirection: { xs: 'column', sm: 'row' }, gap: 1, mb: 2 }}>
          {termsPending && (
            <Button
              component={Link}
              to={`/legal/terms?returnUrl=${encodeURIComponent(returnUrl)}`}
              variant="outlined"
              fullWidth
            >
              {t('legal.viewTerms')}
            </Button>
          )}
          {privacyPending && (
            <Button
              component={Link}
              to={`/legal/privacy?returnUrl=${encodeURIComponent(returnUrl)}`}
              variant="outlined"
              fullWidth
            >
              {t('legal.viewPrivacy')}
            </Button>
          )}
        </Box>

        {termsPending && (
          <FormControlLabel
            control={<Checkbox checked={acceptTerms} onChange={(e) => setAcceptTerms(e.target.checked)} />}
            label={t('legal.acceptTermsLabel')}
          />
        )}
        {privacyPending && (
          <FormControlLabel
            control={<Checkbox checked={acceptPrivacy} onChange={(e) => setAcceptPrivacy(e.target.checked)} />}
            label={t('legal.acceptPrivacyLabel')}
          />
        )}
      </DialogContent>
      <DialogActions sx={{ px: 3, pb: 3 }}>
        <Button variant="contained" size="large" fullWidth disabled={!canSubmit || submitting} onClick={onAccept}>
          {submitting ? t('common.loading') : t('legal.continueAfterAccept')}
        </Button>
      </DialogActions>
    </Dialog>
  );
}
