import { useMemo } from 'react';
import { Box, Container, Typography } from '@mui/material';
import { useSearchParams } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { ServiceCard } from '../components/ServiceCard';
import { filterServices, type ServiceCategory } from '../data/sampleServices';

export function ServicesPage() {
  const { t } = useTranslation();
  const [params] = useSearchParams();
  const query = params.get('q') ?? '';
  const category = (params.get('category') ?? 'all') as ServiceCategory | 'all';

  const services = useMemo(() => filterServices(query, category), [query, category]);

  return (
    <Container maxWidth="lg" sx={{ py: 5 }}>
      <Typography variant="h4" sx={{ fontWeight: 800 }}>
        {t('services.title')}
      </Typography>
      <Typography color="text.secondary" sx={{ mb: 4 }}>
        {t('services.subtitle')}
      </Typography>
      {services.length === 0 ? (
        <Typography color="text.secondary">{t('services.noResults')}</Typography>
      ) : (
        <Box
          sx={{
            display: 'grid',
            gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr', lg: '1fr 1fr 1fr' },
            gap: 2,
          }}
        >
          {services.map((service) => (
            <ServiceCard key={service.id} service={service} />
          ))}
        </Box>
      )}
    </Container>
  );
}
