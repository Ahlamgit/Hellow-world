import { useEffect, useState } from 'react';
import { Alert, Box, CircularProgress, Container } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { useSearchParams } from 'react-router-dom';
import { fetchLegalDocument, type LegalDocument } from '../../auth/legalApi';
import { PublicHeader } from '../../components/PublicHeader';
import { LegalDocumentView } from '../../components/legal/LegalDocumentView';

type LegalDocumentPageProps = {
  type: 'terms' | 'privacy';
};

export function LegalDocumentPage({ type }: LegalDocumentPageProps) {
  const { t, i18n } = useTranslation();
  const [params] = useSearchParams();
  const [document, setDocument] = useState<LegalDocument | null>(null);
  const [error, setError] = useState<string | null>(null);

  const backTo = params.get('returnUrl') ?? '/';

  useEffect(() => {
    let cancelled = false;
    setError(null);
    void fetchLegalDocument(type, i18n.language.startsWith('ar') ? 'ar' : 'en')
      .then((doc) => {
        if (!cancelled) setDocument(doc);
      })
      .catch(() => {
        if (!cancelled) setError(t('legal.loadError'));
      });
    return () => {
      cancelled = true;
    };
  }, [type, i18n.language, t]);

  return (
    <Box sx={{ minHeight: '100vh', bgcolor: 'background.default' }}>
      <PublicHeader />
      {error && (
        <Container maxWidth="md" sx={{ pt: 4 }}>
          <Alert severity="error">{error}</Alert>
        </Container>
      )}
      {!document && !error && (
        <Box sx={{ display: 'flex', justifyContent: 'center', py: 8 }}>
          <CircularProgress />
        </Box>
      )}
      {document && <LegalDocumentView document={document} backTo={backTo} />}
    </Box>
  );
}
