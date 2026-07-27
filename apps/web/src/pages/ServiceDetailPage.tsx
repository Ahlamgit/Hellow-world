import { useState } from 'react';
import {
  Box,
  Breadcrumbs,
  Button,
  Chip,
  Container,
  Dialog,
  DialogActions,
  DialogContent,
  DialogTitle,
  Link,
  Typography,
} from '@mui/material';
import StarIcon from '@mui/icons-material/Star';
import VerifiedIcon from '@mui/icons-material/Verified';
import { Link as RouterLink, useNavigate, useParams } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../auth/AuthContext';
import { getServiceById } from '../data/sampleServices';

export function ServiceDetailPage() {
  const { id } = useParams();
  const { t, i18n } = useTranslation();
  const navigate = useNavigate();
  const { isAuthenticated, portal } = useAuth();
  const [authDialogOpen, setAuthDialogOpen] = useState(false);
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

  const bookPath = `/services/${service.id}/book`;

  const handleBook = () => {
    if (isAuthenticated && portal === 'customer') {
      navigate(bookPath);
      return;
    }
    if (isAuthenticated && portal !== 'customer') {
      return;
    }
    setAuthDialogOpen(true);
  };

  const continueToLogin = () => {
    setAuthDialogOpen(false);
    navigate(`/login?returnUrl=${encodeURIComponent(bookPath)}&intent=book`);
  };

  return (
    <Container maxWidth="md" sx={{ py: 5 }}>
      <Breadcrumbs sx={{ mb: 3 }}>
        <Link component={RouterLink} to="/" underline="hover" color="inherit">
          {t('nav.home')}
        </Link>
        <Link component={RouterLink} to="/services" underline="hover" color="inherit">
          {t('nav.browse')}
        </Link>
        <Typography color="text.primary">{isAr ? service.titleAr : service.titleEn}</Typography>
      </Breadcrumbs>

      <Box sx={{ p: { xs: 2, md: 4 }, borderRadius: 3, border: 1, borderColor: 'divider', bgcolor: 'background.paper' }}>
        <Box sx={{ display: 'flex', flexDirection: 'row', gap: 1, alignItems: 'center', mb: 2 }}>
          <Chip label={t(`categories.${service.category}`)} color="primary" variant="outlined" />
          {service.verified && <VerifiedIcon color="primary" />}
        </Box>
        <Typography variant="h4" sx={{ fontWeight: 800 }}>
          {isAr ? service.titleAr : service.titleEn}
        </Typography>
        <Typography variant="subtitle1" color="text.secondary">
          {isAr ? service.providerAr : service.providerEn}
        </Typography>
        <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5, mb: 3 }}>
          <StarIcon sx={{ color: 'warning.main' }} />
          <Typography>{t('services.rating', { rating: service.rating, count: service.reviewCount })}</Typography>
        </Box>
        <Typography variant="body1" sx={{ mb: 2, lineHeight: 1.8 }}>
          {isAr ? service.descriptionAr : service.descriptionEn}
        </Typography>
        <Typography variant="body2" color="text.secondary" sx={{ mb: 3 }}>
          {t('services.fromPrice')}
        </Typography>

        {isAuthenticated && portal !== 'customer' ? (
          <Typography color="text.secondary">{t('booking.customerAccountRequired')}</Typography>
        ) : (
          <Button onClick={handleBook} variant="contained" size="large">
            {isAuthenticated ? t('services.bookService') : t('services.signInToBook')}
          </Button>
        )}
      </Box>

      <Dialog open={authDialogOpen} onClose={() => setAuthDialogOpen(false)}>
        <DialogTitle>{t('login.authRequiredTitle')}</DialogTitle>
        <DialogContent>
          <Typography>{t('login.authRequiredForBooking')}</Typography>
        </DialogContent>
        <DialogActions>
          <Button onClick={() => setAuthDialogOpen(false)}>{t('common.back')}</Button>
          <Button variant="contained" onClick={continueToLogin}>
            {t('login.continueToSignIn')}
          </Button>
        </DialogActions>
      </Dialog>
    </Container>
  );
}
