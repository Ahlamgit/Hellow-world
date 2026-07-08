import { useEffect, useState } from 'react';
import {
  AppBar, Toolbar, Typography, Button, IconButton, Container,
  Menu, MenuItem, Badge,
} from '@mui/material';
import { Brightness4, Brightness7, Language, Notifications } from '@mui/icons-material';
import { Link, useLocation } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../context/AuthContext';
import { hasAdminAccess } from '../utils/roles';
import { useThemeMode } from '../theme/ThemeContext';
import { notificationsApi } from '../services/api';

export default function Navbar() {
  const { t, i18n } = useTranslation();
  const location = useLocation();
  const { isAuthenticated, user, logout } = useAuth();
  const { mode, toggleMode } = useThemeMode();
  const [langAnchor, setLangAnchor] = useState<null | HTMLElement>(null);
  const [unreadCount, setUnreadCount] = useState(0);

  useEffect(() => {
    if (!isAuthenticated) {
      setUnreadCount(0);
      return;
    }
    notificationsApi.unreadCount()
      .then(setUnreadCount)
      .catch(() => setUnreadCount(0));
  }, [isAuthenticated, location.pathname]);

  const changeLanguage = (lng: string) => {
    i18n.changeLanguage(lng);
    setLangAnchor(null);
    window.location.reload();
  };

  return (
    <AppBar position="sticky" elevation={0} sx={{ bgcolor: 'background.paper', color: 'text.primary', borderBottom: 1, borderColor: 'divider' }}>
      <Container maxWidth="lg">
        <Toolbar disableGutters sx={{ gap: 2 }}>
          <Typography
            variant="h6"
            component={Link}
            to="/"
            sx={{ flexGrow: 1, fontWeight: 800, color: 'primary.main', textDecoration: 'none' }}
          >
            {t('app.name')}
          </Typography>

          <Button component={Link} to="/" color="inherit">{t('nav.home')}</Button>
          <Button component={Link} to="/services" color="inherit">{t('nav.services')}</Button>
          {isAuthenticated && (
            <Button component={Link} to="/bookings" color="inherit">{t('booking.myBookings')}</Button>
          )}
          {isAuthenticated && (user?.role === 'Craftsman' || user?.role === 'Store' || user?.primaryRole === 'StoreOwner') && (
            <Button component={Link} to="/subscriptions" color="inherit">{t('subscription.nav')}</Button>
          )}
          {isAuthenticated && hasAdminAccess(user) && (
            <Button component={Link} to="/admin" color="inherit">Admin</Button>
          )}

          <IconButton onClick={(e) => setLangAnchor(e.currentTarget)} color="inherit">
            <Language />
          </IconButton>
          <Menu anchorEl={langAnchor} open={!!langAnchor} onClose={() => setLangAnchor(null)}>
            <MenuItem onClick={() => changeLanguage('ar')}>العربية</MenuItem>
            <MenuItem onClick={() => changeLanguage('en')}>English</MenuItem>
          </Menu>

          <IconButton onClick={toggleMode} color="inherit">
            {mode === 'dark' ? <Brightness7 /> : <Brightness4 />}
          </IconButton>

          {isAuthenticated ? (
            <>
              <IconButton component={Link} to="/notifications" color="inherit" aria-label={t('notifications.title')}>
                <Badge badgeContent={unreadCount} color="error" max={99}>
                  <Notifications />
                </Badge>
              </IconButton>
              <Button component={Link} to="/dashboard" color="inherit">{t('nav.dashboard')}</Button>
              <Button component={Link} to="/profile" color="inherit">{t('nav.profile')}</Button>
              <Button onClick={logout} variant="outlined" size="small">{t('nav.logout')}</Button>
            </>
          ) : (
            <>
              <Button component={Link} to="/login" color="inherit">{t('nav.login')}</Button>
              <Button component={Link} to="/register" variant="contained" size="small">{t('nav.register')}</Button>
            </>
          )}
        </Toolbar>
      </Container>
    </AppBar>
  );
}
