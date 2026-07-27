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
import { useAuth } from '../auth/AuthContext';
import { LegalComplianceGate } from '../components/legal/LegalComplianceGate';
import { portalDashboardPath } from '../auth/redirects';
import type { PortalRole } from '../auth/types';

const drawerWidth = 240;

type NavItem = { key: string; path: string };

const navByPortal: Record<PortalRole, NavItem[]> = {
  customer: [
    { key: 'home', path: '' },
    { key: 'bookings', path: 'bookings' },
    { key: 'messages', path: 'messages' },
    { key: 'profile', path: 'profile' },
  ],
  provider: [
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
    { key: 'legal', path: 'legal' },
    { key: 'users', path: 'users' },
    { key: 'finance', path: 'finance' },
    { key: 'settings', path: 'settings' },
  ],
};

type RoleShellProps = {
  portal: PortalRole;
};

export function RoleShell({ portal }: RoleShellProps) {
  const { t, i18n } = useTranslation();
  const navigate = useNavigate();
  const { logout } = useAuth();
  const basePath = portalDashboardPath(portal);
  const navItems = navByPortal[portal];

  const toggleLanguage = () => {
    const next = i18n.language === 'ar' ? 'en' : 'ar';
    void i18n.changeLanguage(next);
    document.documentElement.dir = next === 'ar' ? 'rtl' : 'ltr';
    document.documentElement.lang = next;
  };

  const signOut = () => {
    logout();
    navigate('/login', { replace: true });
  };

  return (
    <Box sx={{ display: 'flex' }}>
      <AppBar position="fixed" sx={{ zIndex: (theme) => theme.zIndex.drawer + 1 }}>
        <Toolbar>
          <Typography variant="h6" sx={{ flexGrow: 1 }}>
            {t('app.name')} — {t(`roles.${portal}`)}
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
        {portal !== 'admin' && <LegalComplianceGate portal={portal} />}
        <Outlet />
      </Box>
    </Box>
  );
}
