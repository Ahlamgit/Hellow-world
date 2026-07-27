import { useCallback, useEffect, useState } from 'react';
import {
  Alert,
  Box,
  Button,
  FormControl,
  InputLabel,
  MenuItem,
  Paper,
  Select,
  Table,
  TableBody,
  TableCell,
  TableHead,
  TableRow,
  TextField,
  Typography,
} from '@mui/material';
import { useTranslation } from 'react-i18next';
import { useAdminAuth } from '../../auth/AdminAuthContext';
import {
  listAdminLegalDocuments,
  publishLegalDocument,
  type LegalDocument,
  type LegalDocumentType,
} from '../../auth/legalApi';

export function AdminLegalPage() {
  const { t } = useTranslation();
  const { session } = useAdminAuth();

  const [documents, setDocuments] = useState<LegalDocument[]>([]);
  const [documentType, setDocumentType] = useState<LegalDocumentType>('TERMS');
  const [version, setVersion] = useState('');
  const [title, setTitle] = useState('');
  const [content, setContent] = useState('');
  const [language, setLanguage] = useState('en');
  const [error, setError] = useState<string | null>(null);
  const [success, setSuccess] = useState<string | null>(null);
  const [submitting, setSubmitting] = useState(false);

  const loadDocuments = useCallback(async () => {
    if (!session?.accessToken) return;
    try {
      const list = await listAdminLegalDocuments(session.accessToken);
      setDocuments(list);
    } catch {
      setError(t('legal.adminLoadError'));
    }
  }, [session?.accessToken, t]);

  useEffect(() => {
    void loadDocuments();
  }, [loadDocuments]);

  const onPublish = async (event: React.FormEvent) => {
    event.preventDefault();
    if (!session?.accessToken) return;
    setError(null);
    setSuccess(null);
    setSubmitting(true);
    try {
      await publishLegalDocument(session.accessToken, {
        documentType,
        version: version.trim(),
        title: title.trim(),
        content,
        language,
      });
      setSuccess(t('legal.adminPublishSuccess'));
      setVersion('');
      setTitle('');
      setContent('');
      await loadDocuments();
    } catch (err) {
      setError(err instanceof Error ? err.message : t('common.error'));
    } finally {
      setSubmitting(false);
    }
  };

  return (
    <Box sx={{ display: 'grid', gap: 3 }}>
      <Typography variant="h5" sx={{ fontWeight: 800 }}>
        {t('legal.adminTitle')}
      </Typography>

      {error && <Alert severity="error">{error}</Alert>}
      {success && <Alert severity="success">{success}</Alert>}

      <Paper sx={{ p: 3 }}>
        <Typography variant="h6" sx={{ fontWeight: 700, mb: 2 }}>
          {t('legal.adminPublishTitle')}
        </Typography>
        <Box
          component="form"
          onSubmit={onPublish}
          sx={{ display: 'grid', gap: 2, maxWidth: 720 }}
        >
          <FormControl fullWidth>
            <InputLabel>{t('legal.adminDocumentType')}</InputLabel>
            <Select
              value={documentType}
              label={t('legal.adminDocumentType')}
              onChange={(e) => setDocumentType(e.target.value as LegalDocumentType)}
            >
              <MenuItem value="TERMS">{t('legal.adminTerms')}</MenuItem>
              <MenuItem value="PRIVACY">{t('legal.adminPrivacy')}</MenuItem>
            </Select>
          </FormControl>
          <Box sx={{ display: 'grid', gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' }, gap: 2 }}>
            <TextField
              label={t('legal.adminVersion')}
              required
              value={version}
              onChange={(e) => setVersion(e.target.value)}
              placeholder="2.0"
            />
            <FormControl fullWidth>
              <InputLabel>{t('legal.adminLanguage')}</InputLabel>
              <Select
                value={language}
                label={t('legal.adminLanguage')}
                onChange={(e) => setLanguage(e.target.value)}
              >
                <MenuItem value="en">English</MenuItem>
                <MenuItem value="ar">العربية</MenuItem>
              </Select>
            </FormControl>
          </Box>
          <TextField
            label={t('legal.adminDocTitle')}
            required
            value={title}
            onChange={(e) => setTitle(e.target.value)}
            fullWidth
          />
          <TextField
            label={t('legal.adminContent')}
            required
            value={content}
            onChange={(e) => setContent(e.target.value)}
            multiline
            minRows={6}
            fullWidth
            helperText={t('legal.adminContentHint')}
          />
          <Button type="submit" variant="contained" disabled={submitting}>
            {submitting ? t('common.loading') : t('legal.adminPublish')}
          </Button>
        </Box>
      </Paper>

      <Paper sx={{ p: 3 }}>
        <Typography variant="h6" sx={{ fontWeight: 700, mb: 2 }}>
          {t('legal.adminHistoryTitle')}
        </Typography>
        <Table size="small">
          <TableHead>
            <TableRow>
              <TableCell>{t('legal.adminDocumentType')}</TableCell>
              <TableCell>{t('legal.adminVersion')}</TableCell>
              <TableCell>{t('legal.adminLanguage')}</TableCell>
              <TableCell>{t('legal.adminDocTitle')}</TableCell>
              <TableCell>{t('legal.adminActive')}</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {documents.map((doc) => (
              <TableRow key={doc.id}>
                <TableCell>{doc.documentType}</TableCell>
                <TableCell>{doc.version}</TableCell>
                <TableCell>{doc.language}</TableCell>
                <TableCell>{doc.title}</TableCell>
                <TableCell>{doc.active ? t('legal.adminActiveYes') : ''}</TableCell>
              </TableRow>
            ))}
          </TableBody>
        </Table>
      </Paper>
    </Box>
  );
}
