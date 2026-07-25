import { useState } from 'react';
import {
  AppBar,
  Box,
  CssBaseline,
  Drawer,
  IconButton,
  List,
  ListItemButton,
  ListItemIcon,
  ListItemText,
  Toolbar,
  Typography,
} from '@mui/material';
import DashboardIcon from '@mui/icons-material/Dashboard';
import PeopleIcon from '@mui/icons-material/People';
import SettingsIcon from '@mui/icons-material/Settings';
import AccountBalanceIcon from '@mui/icons-material/AccountBalance';
import LanguageIcon from '@mui/icons-material/Language';
import { Outlet, useNavigate } from 'react-router-dom';
import { useTranslation } from 'react-i18next';
import { ThemeProvider } from '@mui/material/styles';
import { CacheProvider } from '@emotion/react';
import createCache from '@emotion/cache';
import { prefixer } from 'stylis';
import rtlPlugin from 'stylis-plugin-rtl';
import { khadamatiTheme, khadamatiThemeRtl } from '../theme/khadamatiTheme';

const drawerWidth = 240;

const navItems = [
  { key: 'dashboard', icon: <DashboardIcon />, path: '/' },
  { key: 'users', icon: <PeopleIcon />, path: '/users' },
  { key: 'finance', icon: <AccountBalanceIcon />, path: '/finance' },
  { key: 'settings', icon: <SettingsIcon />, path: '/settings' },
] as const;

export function AppLayout() {
  const { t, i18n } = useTranslation();
  const navigate = useNavigate();
  const isRtl = i18n.language === 'ar';
  const theme = isRtl ? khadamatiThemeRtl : khadamatiTheme;
  const [cache] = useState(() =>
    createCache({
      key: isRtl ? 'muirtl' : 'muiltr',
      stylisPlugins: isRtl ? [prefixer, rtlPlugin] : [prefixer],
    }),
  );

  const toggleLanguage = () => {
    const next = i18n.language === 'ar' ? 'en' : 'ar';
    void i18n.changeLanguage(next);
    document.documentElement.dir = next === 'ar' ? 'rtl' : 'ltr';
    document.documentElement.lang = next;
  };

  return (
    <CacheProvider value={cache}>
      <ThemeProvider theme={theme}>
        <Box sx={{ display: 'flex' }}>
          <CssBaseline />
          <AppBar position="fixed" sx={{ zIndex: (t) => t.zIndex.drawer + 1 }}>
            <Toolbar>
              <Typography variant="h6" sx={{ flexGrow: 1 }}>
                {t('app.name')} — Admin
              </Typography>
              <IconButton color="inherit" onClick={toggleLanguage} aria-label={t('common.language')}>
                <LanguageIcon />
              </IconButton>
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
              {navItems.map((item) => (
                <ListItemButton key={item.key} onClick={() => navigate(item.path)}>
                  <ListItemIcon>{item.icon}</ListItemIcon>
                  <ListItemText primary={t(`nav.${item.key}`)} />
                </ListItemButton>
              ))}
            </List>
          </Drawer>
          <Box component="main" sx={{ flexGrow: 1, p: 3 }}>
            <Toolbar />
            <Outlet />
          </Box>
        </Box>
      </ThemeProvider>
    </CacheProvider>
  );
}
