import {
  Card,
  CardActionArea,
  CardContent,
  Chip,
  Box,
  Typography,
} from '@mui/material';
import StarIcon from '@mui/icons-material/Star';
import VerifiedIcon from '@mui/icons-material/Verified';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import type { SampleService } from '../data/sampleServices';

type ServiceCardProps = {
  service: SampleService;
};

export function ServiceCard({ service }: ServiceCardProps) {
  const { i18n, t } = useTranslation();
  const isAr = i18n.language === 'ar';

  return (
    <Card elevation={0} sx={{ height: '100%', border: 1, borderColor: 'divider', borderRadius: 3 }}>
      <CardActionArea component={Link} to={`/services/${service.id}`} sx={{ height: '100%' }}>
        <CardContent sx={{ p: 2.5 }}>
          <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start', mb: 1 }}>
            <Chip
              size="small"
              label={t(`categories.${service.category}`)}
              sx={{ bgcolor: 'primary.light', color: 'primary.dark', fontWeight: 600 }}
            />
            {service.verified && (
              <VerifiedIcon color="primary" fontSize="small" titleAccess="Verified" />
            )}
          </Box>
          <Typography variant="h6" component="h3" sx={{ fontWeight: 700, mb: 0.5 }}>
            {isAr ? service.titleAr : service.titleEn}
          </Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 1 }}>
            {isAr ? service.providerAr : service.providerEn}
          </Typography>
          <Box sx={{ display: 'flex', alignItems: 'center', gap: 0.5, mt: 1 }}>
            <StarIcon sx={{ fontSize: 18, color: 'warning.main' }} />
            <Typography variant="body2">
              {t('services.rating', { rating: service.rating, count: service.reviewCount })}
            </Typography>
          </Box>
          <Typography variant="caption" color="primary.main" sx={{ display: 'block', mt: 2, fontWeight: 600 }}>
            {t('services.viewDetails')} →
          </Typography>
        </CardContent>
      </CardActionArea>
    </Card>
  );
}
