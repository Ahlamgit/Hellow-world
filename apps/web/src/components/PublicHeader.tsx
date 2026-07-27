import { useState } from 'react';
import {
  AppBar,
  Box,
  Button,
  Container,
  Divider,
  Drawer,
  IconButton,
  List,
  ListItemButton,
  ListItemText,
  Toolbar,
  Typography,
} from '@mui/material';
import LanguageIcon from '@mui/icons-material/Language';
import MenuIcon from '@mui/icons-material/Menu';
import CloseIcon from '@mui/icons-material/Close';
import { Link as RouterLink, useLocation, useNavigate } from 'react-router-dom';
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
  const navigate = useNavigate();
  const [mobileOpen, setMobileOpen] = useState(false);

  const toggleLanguage = () => {
    const next = i18n.language === 'ar' ? 'en' : 'ar';
    void i18n.changeLanguage(next);
    document.documentElement.dir = next === 'ar' ? 'rtl' : 'ltr';
    document.documentElement.lang = next;
  };

  const closeMobile = () => setMobileOpen(false);

  const goTo = (path: string) => {
    closeMobile();
    navigate(path);
  };

  const isActive = (path: string) =>
    path === '/' ? location.pathname === '/' : location.pathname.startsWith(path);

  return (
    <>
      <AppBar
        position="sticky"
        elevation={0}
        sx={{ bgcolor: 'background.paper', color: 'text.primary', borderBottom: 1, borderColor: 'divider' }}
      >
        <Container maxWidth="lg">
          <Toolbar disableGutters sx={{ gap: 2, py: 1 }}>
            <IconButton
              edge="start"
              onClick={() => setMobileOpen(true)}
              aria-label={t('nav.menu')}
              sx={{ display: { md: 'none' } }}
            >
              <MenuIcon />
            </IconButton>

            <Box
              component={RouterLink}
              to="/"
              sx={{
                display: 'flex',
                alignItems: 'center',
                gap: 1.5,
                textDecoration: 'none',
                color: 'inherit',
                flexGrow: { xs: 1, md: 0 },
              }}
            >
              <Box
                component="img"
                src="/khadamatiLogo.jpg"
                alt={t('app.name')}
                sx={{ width: 44, height: 44, borderRadius: 2, objectFit: 'cover' }}
              />
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
                  color={isActive(link.to) ? 'primary' : 'inherit'}
                  sx={{ fontWeight: isActive(link.to) ? 700 : 500 }}
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

      <Drawer
        anchor={i18n.language === 'ar' ? 'right' : 'left'}
        open={mobileOpen}
        onClose={closeMobile}
        sx={{ display: { md: 'none' } }}
      >
        <Box sx={{ width: 280, pt: 1 }} role="presentation">
          <Box sx={{ display: 'flex', alignItems: 'center', justifyContent: 'space-between', px: 2, py: 1 }}>
            <Box sx={{ display: 'flex', alignItems: 'center', gap: 1.5 }}>
              <Box
                component="img"
                src="/khadamatiLogo.jpg"
                alt={t('app.name')}
                sx={{ width: 36, height: 36, borderRadius: 1.5, objectFit: 'cover' }}
              />
              <Typography variant="subtitle1" sx={{ fontWeight: 800, color: 'primary.main' }}>
                {i18n.language === 'ar' ? t('app.nameAr') : t('app.name')}
              </Typography>
            </Box>
            <IconButton onClick={closeMobile} aria-label={t('common.close')}>
              <CloseIcon />
            </IconButton>
          </Box>
          <Divider />
          <List>
            {navLinks.map((link) => (
              <ListItemButton key={link.to} selected={isActive(link.to)} onClick={() => goTo(link.to)}>
                <ListItemText primary={t(`nav.${link.key}`)} />
              </ListItemButton>
            ))}
          </List>
          <Divider />
          <Box sx={{ p: 2 }}>
            <Button fullWidth variant="contained" onClick={() => goTo('/login')}>
              {t('nav.signIn')}
            </Button>
          </Box>
        </Box>
      </Drawer>
    </>
  );
}
