import {
  Box,
  Button,
  Container,
  Divider,
  Paper,
  Typography,
} from '@mui/material';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { useTranslation } from 'react-i18next';
import { Link as RouterLink } from 'react-router-dom';
import type { LegalDocument } from '../../auth/legalApi';

type LegalDocumentViewProps = {
  document: LegalDocument;
  backTo?: string;
};

export function LegalDocumentView({ document, backTo = '/' }: LegalDocumentViewProps) {
  const { t, i18n } = useTranslation();
  const updated = new Date(document.updatedAt).toLocaleDateString(i18n.language);

  return (
    <Container maxWidth="md" sx={{ py: { xs: 4, md: 6 } }}>
      <Button component={RouterLink} to={backTo} startIcon={<ArrowBackIcon />} sx={{ mb: 3 }}>
        {t('common.back')}
      </Button>

      <Paper sx={{ p: { xs: 2, md: 4 }, borderRadius: 3 }}>
        <Typography variant="h4" component="h1" sx={{ fontWeight: 800, mb: 1 }}>
          {document.title}
        </Typography>
        <Box sx={{ display: 'flex', flexWrap: 'wrap', gap: 2, mb: 3 }}>
          <Typography variant="body2" color="text.secondary">
            {t('legal.version', { version: document.version })}
          </Typography>
          <Typography variant="body2" color="text.secondary">
            {t('legal.lastUpdated', { date: updated })}
          </Typography>
        </Box>
        <Divider sx={{ mb: 3 }} />
        <Box
          sx={{
            '& h2': { fontWeight: 700, mt: 3, mb: 1, fontSize: '1.1rem' },
            '& p': { mb: 1.5, lineHeight: 1.8 },
            whiteSpace: 'pre-wrap',
          }}
        >
          {document.content.split('\n').map((line, index) => {
            if (line.startsWith('## ')) {
              return (
                <Typography key={index} component="h2" variant="subtitle1" sx={{ fontWeight: 700, mt: 2 }}>
                  {line.replace(/^##\s*/, '')}
                </Typography>
              );
            }
            return (
              <Typography key={index} component="p" variant="body1" color="text.secondary">
                {line}
              </Typography>
            );
          })}
        </Box>
      </Paper>
    </Container>
  );
}
