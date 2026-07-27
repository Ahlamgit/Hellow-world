import {
  AppBar,
  Box,
  Button,
  Container,
  IconButton,
  Toolbar,
  Typography,
} from '@mui/material';
import LanguageIcon from '@mui/icons-material/Language';
import { Link as RouterLink, useLocation } from 'react-router-dom';
import { useTranslation } from 'react-i18next';

const navLinks = [
  { to: '/', key: 'home' },
  { to: '/services', key: 'browse' },
  { to: '/categories', key: 'categories' },
  { to: '/how-it-works', key: 'howItWorks' },
] as const;

export function PublicHeader() {
  const { t, i18n } = useTranslation();
  const location = useLocation();

  const toggleLanguage = () => {
    const next = i18n.language === 'ar' ? 'en' : 'ar';
    void i18n.changeLanguage(next);
    document.documentElement.dir = next === 'ar' ? 'rtl' : 'ltr';
    document.documentElement.lang = next;
  };

  return (
    <AppBar position="sticky" elevation={0} sx={{ bgcolor: 'background.paper', color: 'text.primary', borderBottom: 1, borderColor: 'divider' }}>
      <Container maxWidth="lg">
        <Toolbar disableGutters sx={{ gap: 2, py: 1 }}>
          <Box component={RouterLink} to="/" sx={{ display: 'flex', alignItems: 'center', gap: 1, textDecoration: 'none', color: 'inherit', flexGrow: { xs: 1, md: 0 } }}>
            <Box
              sx={{
                width: 40,
                height: 40,
                borderRadius: 2,
                bgcolor: 'primary.main',
                display: 'flex',
                alignItems: 'center',
                justifyContent: 'center',
                color: 'primary.contrastText',
                fontWeight: 800,
                fontSize: 18,
              }}
            >
              خ
            </Box>
            <Box>
              <Typography variant="subtitle1" sx={{ fontWeight: 800, lineHeight: 1.1, color: 'primary.main' }}>
                {i18n.language === 'ar' ? t('app.nameAr') : t('app.name')}
              </Typography>
              <Typography variant="caption" color="text.secondary" sx={{ display: { xs: 'none', sm: 'block' } }}>
                {i18n.language === 'ar' ? t('app.taglineAr') : t('app.tagline')}
              </Typography>
            </Box>
          </Box>

          <Box sx={{ display: { xs: 'none', md: 'flex' }, flexGrow: 1, justifyContent: 'center', gap: 1 }}>
            {navLinks.map((link) => (
              <Button
                key={link.to}
                component={RouterLink}
                to={link.to}
                color={location.pathname === link.to ? 'primary' : 'inherit'}
                sx={{ fontWeight: location.pathname === link.to ? 700 : 500 }}
              >
                {t(`nav.${link.key}`)}
              </Button>
            ))}
          </Box>

          <Box sx={{ display: 'flex', flexDirection: 'row', gap: 1, alignItems: 'center' }}>
            <IconButton onClick={toggleLanguage} aria-label={t('common.language')} color="inherit">
              <LanguageIcon />
            </IconButton>
            <Button component={RouterLink} to="/login" variant="contained" color="primary">
              {t('nav.signIn')}
            </Button>
          </Box>
        </Toolbar>
      </Container>
    </AppBar>
  );
}
