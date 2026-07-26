import {
  AppBar,
  Box,
  Button,
  Drawer,
  IconButton,
  List,
  ListItemButton,
  ListItemText,
  Toolbar,
  Typography,
} from '@mui/material';
import LanguageIcon from '@mui/icons-material/Language';
import LogoutIcon from '@mui/icons-material/Logout';
import { Outlet, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import type { ActorType } from '../auth/actor';
import { clearActor } from '../auth/actor';

const drawerWidth = 240;

type NavItem = { key: string; path: string };

const navByActor: Record<ActorType, NavItem[]> = {
  customer: [
    { key: 'home', path: '' },
    { key: 'bookings', path: 'bookings' },
    { key: 'messages', path: 'messages' },
    { key: 'profile', path: 'profile' },
  ],
  craftsman: [
    { key: 'dashboard', path: '' },
    { key: 'bookings', path: 'bookings' },
    { key: 'messages', path: 'messages' },
    { key: 'profile', path: 'profile' },
  ],
  store: [
    { key: 'dashboard', path: '' },
    { key: 'services', path: 'services' },
    { key: 'analytics', path: 'analytics' },
    { key: 'settings', path: 'settings' },
  ],
  admin: [
    { key: 'dashboard', path: '' },
    { key: 'users', path: 'users' },
    { key: 'finance', path: 'finance' },
    { key: 'settings', path: 'settings' },
  ],
};

type ActorShellProps = {
  actor: ActorType;
};

export function ActorShell({ actor }: ActorShellProps) {
  const { t, i18n } = useTranslation();
  const navigate = useNavigate();
  const basePath = `/${actor}`;
  const navItems = navByActor[actor];

  const toggleLanguage = () => {
    const next = i18n.language === 'ar' ? 'en' : 'ar';
    void i18n.changeLanguage(next);
    document.documentElement.dir = next === 'ar' ? 'rtl' : 'ltr';
    document.documentElement.lang = next;
  };

  const signOut = () => {
    clearActor();
    navigate('/login', { replace: true });
  };

  return (
    <Box sx={{ display: 'flex' }}>
      <AppBar position="fixed" sx={{ zIndex: (t) => t.zIndex.drawer + 1 }}>
            <Toolbar>
              <Typography variant="h6" sx={{ flexGrow: 1 }}>
                {t('app.name')} — {t(`login.${actor}`)}
              </Typography>
              <IconButton color="inherit" onClick={toggleLanguage} aria-label={t('common.language')}>
                <LanguageIcon />
              </IconButton>
              <Button color="inherit" startIcon={<LogoutIcon />} onClick={signOut}>
                {t('common.signOut')}
              </Button>
            </Toolbar>
          </AppBar>
          <Drawer
            variant="permanent"
            sx={{
              width: drawerWidth,
              flexShrink: 0,
              [`& .MuiDrawer-paper`]: { width: drawerWidth, boxSizing: 'border-box' },
            }}
          >
            <Toolbar />
            <List>
              {navItems.map((item) => {
                const path = item.path ? `${basePath}/${item.path}` : basePath;
                return (
                  <ListItemButton key={item.key} onClick={() => navigate(path)}>
                    <ListItemText primary={t(`nav.${item.key}`)} />
                  </ListItemButton>
                );
              })}
            </List>
          </Drawer>
          <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
            <Toolbar />
            <Outlet />
          </Box>
    </Box>
  );
}
