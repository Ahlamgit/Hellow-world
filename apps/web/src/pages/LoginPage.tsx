import { Box, Button, Card, CardActionArea, CardContent, Container, Typography } from '@mui/material';
import PersonOutlinedIcon from '@mui/icons-material/PersonOutlined';
import HandymanOutlinedIcon from '@mui/icons-material/HandymanOutlined';
import StorefrontOutlinedIcon from '@mui/icons-material/StorefrontOutlined';
import AdminPanelSettingsOutlinedIcon from '@mui/icons-material/AdminPanelSettingsOutlined';
import ArrowBackIcon from '@mui/icons-material/ArrowBack';
import { useTranslation } from 'react-i18next';
import { Link, useNavigate } from 'react-router-dom';
import type { ActorType } from '../auth/actor';
import { saveActor } from '../auth/actor';
import { PublicHeader } from '../components/PublicHeader';

const actors: { type: ActorType; icon: React.ReactNode }[] = [
  { type: 'customer', icon: <PersonOutlinedIcon fontSize="large" color="primary" /> },
  { type: 'craftsman', icon: <HandymanOutlinedIcon fontSize="large" color="primary" /> },
  { type: 'store', icon: <StorefrontOutlinedIcon fontSize="large" color="primary" /> },
  { type: 'admin', icon: <AdminPanelSettingsOutlinedIcon fontSize="large" color="primary" /> },
];

export function LoginPage() {
  const { t } = useTranslation();
  const navigate = useNavigate();

  const selectActor = (actor: ActorType) => {
    saveActor(actor);
    navigate(`/${actor}`);
  };

  return (
    <Box sx={{ minHeight: '100vh', bgcolor: 'background.default' }}>
      <PublicHeader />
      <Container maxWidth="sm" sx={{ py: 6 }}>
        <Button component={Link} to="/" startIcon={<ArrowBackIcon />} sx={{ mb: 3 }}>
          {t('login.browseWithout')}
        </Button>
        <Typography variant="h4" component="h1" sx={{ fontWeight: 800 }} align="center">
          {t('login.title')}
        </Typography>
        <Typography color="text.secondary" align="center" sx={{ mb: 4 }}>
          {t('login.subtitle')}
        </Typography>
        <Box sx={{ display: 'grid', gap: 2 }}>
          {actors.map(({ type, icon }) => (
            <Card key={type} variant="outlined" sx={{ borderRadius: 3 }}>
              <CardActionArea onClick={() => selectActor(type)}>
                <CardContent sx={{ display: 'flex', alignItems: 'center', gap: 2, py: 2.5 }}>
                  <Box sx={{ bgcolor: 'primary.light', borderRadius: 2, p: 1.5, display: 'flex' }}>{icon}</Box>
                  <Box sx={{ flex: 1 }}>
                    <Typography variant="h6" sx={{ fontWeight: 700 }}>
                      {t(`login.${type}`)}
                    </Typography>
                    <Typography variant="body2" color="text.secondary">
                      {t(`login.${type}Desc`)}
                    </Typography>
                  </Box>
                  <Typography color="primary.main" sx={{ fontWeight: 700 }}>
                    {t('login.continue')} →
                  </Typography>
                </CardContent>
              </CardActionArea>
            </Card>
          ))}
        </Box>
      </Container>
    </Box>
  );
}
