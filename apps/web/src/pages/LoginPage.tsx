import {
  Box,
  Button,
  Card,
  CardActionArea,
  CardContent,
  Container,
  IconButton,
  Typography,
} from '@mui/material';
import PersonOutlinedIcon from '@mui/icons-material/PersonOutlined';
import HandymanOutlinedIcon from '@mui/icons-material/HandymanOutlined';
import StorefrontOutlinedIcon from '@mui/icons-material/StorefrontOutlined';
import AdminPanelSettingsOutlinedIcon from '@mui/icons-material/AdminPanelSettingsOutlined';
import LanguageIcon from '@mui/icons-material/Language';
import { useTranslation } from 'react-i18next';
import { useNavigate } from 'react-router-dom';
import type { ActorType } from '../auth/actor';
import { saveActor } from '../auth/actor';

const actors: { type: ActorType; icon: React.ReactNode }[] = [
  { type: 'customer', icon: <PersonOutlinedIcon fontSize="large" color="primary" /> },
  { type: 'craftsman', icon: <HandymanOutlinedIcon fontSize="large" color="primary" /> },
  { type: 'store', icon: <StorefrontOutlinedIcon fontSize="large" color="primary" /> },
  { type: 'admin', icon: <AdminPanelSettingsOutlinedIcon fontSize="large" color="primary" /> },
];

export function LoginPage() {
  const { t, i18n } = useTranslation();
  const navigate = useNavigate();

  const selectActor = (actor: ActorType) => {
    saveActor(actor);
    navigate(`/${actor}`);
  };

  const toggleLanguage = () => {
    const next = i18n.language === 'ar' ? 'en' : 'ar';
    void i18n.changeLanguage(next);
    document.documentElement.dir = next === 'ar' ? 'rtl' : 'ltr';
    document.documentElement.lang = next;
  };

  return (
    <Container maxWidth="md" sx={{ py: 6 }}>
      <Box sx={{ display: 'flex', justifyContent: 'flex-end', mb: 2 }}>
        <IconButton onClick={toggleLanguage} aria-label={t('common.language')}>
          <LanguageIcon />
        </IconButton>
      </Box>
      <Typography variant="h4" component="h1" gutterBottom align="center">
        {t('login.title')}
      </Typography>
      <Typography color="text.secondary" align="center" sx={{ mb: 4 }}>
        {t('login.subtitle')}
      </Typography>
      <Box
        sx={{
          display: 'grid',
          gridTemplateColumns: { xs: '1fr', sm: '1fr 1fr' },
          gap: 2,
        }}
      >
        {actors.map(({ type, icon }) => (
          <Card key={type} variant="outlined">
            <CardActionArea onClick={() => selectActor(type)}>
              <CardContent sx={{ textAlign: 'center', py: 3 }}>
                <Box sx={{ mb: 1 }}>{icon}</Box>
                <Typography variant="h6">{t(`login.${type}`)}</Typography>
                <Typography variant="body2" color="text.secondary">
                  {t(`login.${type}Desc`)}
                </Typography>
                <Button sx={{ mt: 2 }} variant="contained" size="small">
                  {t('login.continue')}
                </Button>
              </CardContent>
            </CardActionArea>
          </Card>
        ))}
      </Box>
    </Container>
  );
}
