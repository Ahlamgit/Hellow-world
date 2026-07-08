import { Container, Typography, Button, Card, CardContent, Stack } from '@mui/material';
import Grid from '@mui/material/Grid';
import { Build, ElectricBolt, AcUnit, FormatPaint, CleaningServices } from '@mui/icons-material';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

const categoryIcons = [Build, ElectricBolt, AcUnit, FormatPaint, CleaningServices];

export default function HomePage() {
  const { t, i18n } = useTranslation();
  const isAr = i18n.language === 'ar';

  const categories = [
    { name: isAr ? 'سباكة' : 'Plumbing' },
    { name: isAr ? 'كهرباء' : 'Electrical' },
    { name: isAr ? 'تكييف' : 'HVAC' },
    { name: isAr ? 'دهان' : 'Painting' },
    { name: isAr ? 'تنظيف' : 'Cleaning' },
  ];

  return (
    <Stack>
      <Stack sx={{ bgcolor: 'primary.main', color: 'white', py: { xs: 8, md: 12 } }}>
        <Container maxWidth="lg">
          <Typography variant="h2" sx={{ fontWeight: 800, fontSize: { xs: '2rem', md: '3rem' } }} gutterBottom>
            {t('home.hero')}
          </Typography>
          <Typography variant="h6" sx={{ opacity: 0.9, mb: 4, maxWidth: 600 }}>
            {t('home.subtitle')}
          </Typography>
          <Stack direction="row" spacing={2}>
            <Button component={Link} to="/services" variant="contained" color="secondary" size="large">
              {t('home.browseServices')}
            </Button>
            <Button component={Link} to="/register" variant="outlined" size="large" sx={{ color: 'white', borderColor: 'white' }}>
              {t('home.getStarted')}
            </Button>
          </Stack>
        </Container>
      </Stack>

      <Container maxWidth="lg" sx={{ py: 8 }}>
        <Typography variant="h4" sx={{ fontWeight: 700, textAlign: 'center' }} gutterBottom>
          {t('home.categories')}
        </Typography>
        <Grid container spacing={3} sx={{ mt: 2 }}>
          {categories.map((cat, i) => {
            const Icon = categoryIcons[i];
            return (
              <Grid key={cat.name} size={{ xs: 6, sm: 4, md: 2.4 }}>
                <Card sx={{ textAlign: 'center', cursor: 'pointer', '&:hover': { transform: 'translateY(-4px)', transition: '0.2s' } }}>
                  <CardContent>
                    <Icon sx={{ fontSize: 48, color: 'primary.main', mb: 1 }} />
                    <Typography variant="body1" sx={{ fontWeight: 600 }}>{cat.name}</Typography>
                  </CardContent>
                </Card>
              </Grid>
            );
          })}
        </Grid>
      </Container>

      <Stack sx={{ bgcolor: 'background.paper', py: 8 }}>
        <Container maxWidth="lg">
          <Typography variant="h4" sx={{ fontWeight: 700, textAlign: 'center' }} gutterBottom>
            {t('home.howItWorks')}
          </Typography>
          <Grid container spacing={4} sx={{ mt: 2 }}>
            {[t('home.step1'), t('home.step2'), t('home.step3')].map((step, i) => (
              <Grid key={step} size={{ xs: 12, md: 4 }}>
                <Card>
                  <CardContent sx={{ textAlign: 'center', py: 4 }}>
                    <Typography variant="h3" color="primary" sx={{ fontWeight: 800 }}>{i + 1}</Typography>
                    <Typography variant="h6" sx={{ mt: 2 }}>{step}</Typography>
                  </CardContent>
                </Card>
              </Grid>
            ))}
          </Grid>
        </Container>
      </Stack>
    </Stack>
  );
}
