import { Container, Typography, Card, CardContent, Grid, Avatar, Chip, Button, Alert, Box } from '@mui/material';
import { Link } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../context/AuthContext';

export default function DashboardPage() {
  const { t } = useTranslation();
  const { user } = useAuth();
  const needsVerification = user?.requiresEmailVerification || user?.emailVerified === false;

  return (
    <Container maxWidth="lg" sx={{ py: 6 }}>
      <Typography variant="h4" sx={{ fontWeight: 700 }} gutterBottom>{t('nav.dashboard')}</Typography>

      {needsVerification && (
        <Alert severity="warning" sx={{ mb: 3 }}>
          {t('identity.verifyEmailPrompt')}{' '}
          <Link to="/verify-email">{t('identity.verifyEmail')}</Link>
        </Alert>
      )}

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
                <Chip label={user?.role || user?.primaryRole} size="small" sx={{ mt: 1 }} />
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
              <Box sx={{ display: 'flex', gap: 1, flexWrap: 'wrap', mt: 2 }}>
                <Button component={Link} to="/profile" variant="contained" size="small">{t('identity.editProfile')}</Button>
                <Button component={Link} to="/sessions" variant="outlined" size="small">{t('identity.sessionsTitle')}</Button>
                <Button component={Link} to="/change-password" variant="outlined" size="small">{t('identity.changePassword')}</Button>
              </Box>
            </CardContent>
          </Card>
        </Grid>
      </Grid>
    </Container>
  );
}
