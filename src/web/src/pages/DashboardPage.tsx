import { Container, Typography, Card, CardContent, Grid, Avatar, Chip } from '@mui/material';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../context/AuthContext';

export default function DashboardPage() {
  const { t } = useTranslation();
  const { user } = useAuth();

  return (
    <Container maxWidth="lg" sx={{ py: 6 }}>
      <Typography variant="h4" sx={{ fontWeight: 700 }} gutterBottom>{t('nav.dashboard')}</Typography>
      <Grid container spacing={3} sx={{ mt: 1 }}>
        <Grid size={{ xs: 12, md: 4 }}>
          <Card>
            <CardContent sx={{ display: 'flex', alignItems: 'center', gap: 2 }}>
              <Avatar sx={{ width: 64, height: 64, bgcolor: 'primary.main', fontSize: 24 }}>
                {user?.firstName?.[0]}{user?.lastName?.[0]}
              </Avatar>
              <div>
                <Typography variant="h6">{user?.firstName} {user?.lastName}</Typography>
                <Typography variant="body2" color="text.secondary">{user?.email}</Typography>
                <Chip label={user?.role} size="small" sx={{ mt: 1 }} />
              </div>
            </CardContent>
          </Card>
        </Grid>
        <Grid size={{ xs: 12, md: 8 }}>
          <Card>
            <CardContent>
              <Typography variant="h6" gutterBottom>{t('nav.profile')}</Typography>
              <Typography variant="body2" color="text.secondary">{t('auth.phone')}: {user?.phone}</Typography>
              <Typography variant="body2" color="text.secondary" sx={{ mt: 1 }}>
                Status: {user?.status} | Verification: {user?.verificationStatus}
              </Typography>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Container>
  );
}
