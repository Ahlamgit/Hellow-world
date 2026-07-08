import { useEffect, useState } from 'react';
import { Container, Typography, Card, CardContent, CardActions, Button, Chip, CircularProgress, Box } from '@mui/material';
import Grid from '@mui/material/Grid';
import { useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../context/AuthContext';
import { servicesApi, type Service } from '../services/api';

export default function ServicesPage() {
  const { t, i18n } = useTranslation();
  const isAr = i18n.language === 'ar';
  const navigate = useNavigate();
  const { isAuthenticated } = useAuth();
  const [services, setServices] = useState<Service[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    servicesApi.getServices()
      .then((res) => setServices(res.data.data))
      .catch(() => setServices([]))
      .finally(() => setLoading(false));
  }, []);

  if (loading) {
    return (
      <Box sx={{ display: 'flex', justifyContent: 'center', py: 8 }}>
        <CircularProgress />
      </Box>
    );
  }

  return (
    <Container maxWidth="lg" sx={{ py: 6 }}>
      <Typography variant="h4" sx={{ fontWeight: 700 }} gutterBottom>{t('services.title')}</Typography>
      <Grid container spacing={3} sx={{ mt: 1 }}>
        {services.map((service) => (
          <Grid key={service.id} size={{ xs: 12, sm: 6, md: 4 }}>
            <Card sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
              <CardContent sx={{ flexGrow: 1 }}>
                <Typography variant="h6" sx={{ fontWeight: 600 }}>
                  {isAr ? service.nameAr : service.nameEn}
                </Typography>
                <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                  {isAr ? service.descriptionAr : service.descriptionEn}
                </Typography>
                <Box sx={{ mt: 2, display: 'flex', gap: 1 }}>
                  <Chip label={`${t('services.price')}: ${service.basePrice} SAR`} color="primary" size="small" />
                  <Chip label={`${service.estimatedDurationMinutes} ${t('common.minutes')}`} size="small" />
                </Box>
              </CardContent>
              <CardActions sx={{ p: 2, pt: 0 }}>
                <Button variant="contained" fullWidth onClick={() => {
                  if (!isAuthenticated) { navigate('/login'); return; }
                  navigate(`/bookings/new?serviceId=${service.id}`);
                }}>{t('services.request')}</Button>
              </CardActions>
            </Card>
          </Grid>
        ))}
      </Grid>
    </Container>
  );
}
