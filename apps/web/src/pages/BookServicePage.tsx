import {
  Alert,
  Button,
  Container,
  Paper,
  Typography,
} from '@mui/material';
import { Link as RouterLink, useParams } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { getServiceById } from '../data/sampleServices';

export function BookServicePage() {
  const { id } = useParams();
  const { t, i18n } = useTranslation();
  const isAr = i18n.language === 'ar';
  const service = id ? getServiceById(id) : undefined;

  if (!service) {
    return (
      <Container sx={{ py: 6 }}>
        <Typography>{t('services.noResults')}</Typography>
        <Button component={RouterLink} to="/services" sx={{ mt: 2 }}>
          {t('common.back')}
        </Button>
      </Container>
    );
  }

  return (
    <Container maxWidth="sm" sx={{ py: 6 }}>
      <Paper sx={{ p: 4, borderRadius: 3 }}>
        <Typography variant="h5" sx={{ fontWeight: 800, mb: 1 }}>
          {t('booking.title')}
        </Typography>
        <Typography color="text.secondary" sx={{ mb: 3 }}>
          {isAr ? service.titleAr : service.titleEn}
        </Typography>
        <Alert severity="success" sx={{ mb: 3 }}>
          {t('booking.authenticated')}
        </Alert>
        <Typography variant="body2" color="text.secondary">
          {t('booking.placeholder')}
        </Typography>
        <Button component={RouterLink} to={`/services/${service.id}`} sx={{ mt: 3 }}>
          {t('common.back')}
        </Button>
      </Paper>
    </Container>
  );
}
