import { useState } from 'react';
import { Link, Outlet, useLocation, useNavigate } from 'react-router-dom';
import {
  AppBar, Box, Drawer, IconButton, List, ListItemButton, ListItemIcon, ListItemText,
  Toolbar, Typography, Divider, useMediaQuery, useTheme, Button,
} from '@mui/material';
import {
  Menu as MenuIcon, Dashboard, People, Person, Engineering, Store,
  Category, Handyman, EventNote, CardMembership, Campaign, LocalOffer,
  Notifications, Payment, Assessment, Analytics, ReportProblem, SupportAgent,
  LocationCity, Map, Settings, Security, VpnKey, History, Timeline,
  Backup, Restore, MonitorHeart, Logout, Home, VerifiedUser,
} from '@mui/icons-material';
import { useTranslation } from 'react-i18next';
import { useAuth } from '../context/AuthContext';
import { canAccessAdminPath } from '../utils/permissions';

const DRAWER_WIDTH = 280;

interface NavItem {
  path: string;
  label: string;
  labelAr: string;
  icon: React.ReactNode;
}

const NAV_SECTIONS: { title: string; titleAr: string; items: NavItem[] }[] = [
  {
    title: 'Overview',
    titleAr: 'نظرة عامة',
    items: [
      { path: '/admin', label: 'Dashboard', labelAr: 'لوحة التحكم', icon: <Dashboard /> },
      { path: '/admin/analytics', label: 'Analytics', labelAr: 'التحليلات', icon: <Analytics /> },
      { path: '/admin/reports', label: 'Reports', labelAr: 'التقارير', icon: <Assessment /> },
      { path: '/admin/system-health', label: 'System Health', labelAr: 'صحة النظام', icon: <MonitorHeart /> },
    ],
  },
  {
    title: 'Users & Accounts',
    titleAr: 'المستخدمون',
    items: [
      { path: '/admin/users', label: 'Users', labelAr: 'المستخدمون', icon: <People /> },
      { path: '/admin/customers', label: 'Customers', labelAr: 'العملاء', icon: <Person /> },
      { path: '/admin/craftsmen', label: 'Craftsmen', labelAr: 'الحرفيون', icon: <Engineering /> },
      { path: '/admin/stores', label: 'Stores', labelAr: 'المتاجر', icon: <Store /> },
      { path: '/admin/roles', label: 'Role Permissions', labelAr: 'صلاحيات الأدوار', icon: <Security /> },
      { path: '/admin/permissions', label: 'Permissions', labelAr: 'الصلاحيات', icon: <VpnKey /> },
    ],
  },
  {
    title: 'Catalog & Services',
    titleAr: 'الخدمات',
    items: [
      { path: '/admin/categories', label: 'Categories', labelAr: 'الفئات', icon: <Category /> },
      { path: '/admin/services', label: 'Services', labelAr: 'الخدمات', icon: <Handyman /> },
      { path: '/admin/bookings', label: 'Bookings', labelAr: 'الحجوزات', icon: <EventNote /> },
      { path: '/admin/subscriptions', label: 'Plans', labelAr: 'الباقات', icon: <CardMembership /> },
      { path: '/admin/user-subscriptions', label: 'User Subscriptions', labelAr: 'اشتراكات المستخدمين', icon: <CardMembership /> },
    ],
  },
  {
    title: 'Marketing & Finance',
    titleAr: 'التسويق والمالية',
    items: [
      { path: '/admin/advertisements', label: 'Advertisements', labelAr: 'الإعلانات', icon: <Campaign /> },
      { path: '/admin/coupons', label: 'Coupons', labelAr: 'الكوبونات', icon: <LocalOffer /> },
      { path: '/admin/notifications', label: 'Notifications', labelAr: 'الإشعارات', icon: <Notifications /> },
      { path: '/admin/payments', label: 'Payments', labelAr: 'المدفوعات', icon: <Payment /> },
    ],
  },
  {
    title: 'Support & Locations',
    titleAr: 'الدعم والمواقع',
    items: [
      { path: '/admin/complaints', label: 'Complaints', labelAr: 'الشكاوى', icon: <ReportProblem /> },
      { path: '/admin/verification-documents', label: 'Verification', labelAr: 'التحقق', icon: <VerifiedUser /> },
      { path: '/admin/support-tickets', label: 'Support Tickets', labelAr: 'تذاكر الدعم', icon: <SupportAgent /> },
      { path: '/admin/cities', label: 'Cities', labelAr: 'المدن', icon: <LocationCity /> },
      { path: '/admin/regions', label: 'Regions', labelAr: 'المناطق', icon: <Map /> },
    ],
  },
  {
    title: 'System',
    titleAr: 'النظام',
    items: [
      { path: '/admin/settings', label: 'Settings', labelAr: 'الإعدادات', icon: <Settings /> },
      { path: '/admin/audit-logs', label: 'Audit Logs', labelAr: 'سجلات التدقيق', icon: <History /> },
      { path: '/admin/activity-logs', label: 'Activity Logs', labelAr: 'سجلات النشاط', icon: <Timeline /> },
      { path: '/admin/backup', label: 'Backup', labelAr: 'النسخ الاحتياطي', icon: <Backup /> },
      { path: '/admin/restore', label: 'Restore', labelAr: 'الاستعادة', icon: <Restore /> },
    ],
  },
];

export default function AdminLayout() {
  const theme = useTheme();
  const isMobile = useMediaQuery(theme.breakpoints.down('md'));
  const [mobileOpen, setMobileOpen] = useState(false);
  const location = useLocation();
  const navigate = useNavigate();
  const { i18n } = useTranslation();
  const { user, logout } = useAuth();
  const isAr = i18n.language === 'ar';

  const drawer = (
    <Box sx={{ height: '100%', display: 'flex', flexDirection: 'column' }}>
      <Toolbar sx={{ px: 2 }}>
        <Typography variant="h6" sx={{ fontWeight: 800, color: 'primary.main' }}>
          Khadamati Admin
        </Typography>
      </Toolbar>
      <Divider />
      <Box sx={{ flexGrow: 1, overflow: 'auto', py: 1 }}>
        {NAV_SECTIONS.map((section) => (
          <Box key={section.title} sx={{ mb: 1 }}>
            <Typography variant="caption" sx={{ px: 2, py: 1, display: 'block', color: 'text.secondary', fontWeight: 600 }}>
              {isAr ? section.titleAr : section.title}
            </Typography>
            <List dense disablePadding>
              {section.items.filter((item) => canAccessAdminPath(user, item.path)).map((item) => {
                const selected = location.pathname === item.path ||
                  (item.path !== '/admin' && location.pathname.startsWith(item.path));
                return (
                  <ListItemButton
                    key={item.path}
                    component={Link}
                    to={item.path}
                    selected={selected}
                    onClick={() => isMobile && setMobileOpen(false)}
                    sx={{ mx: 1, borderRadius: 1 }}
                  >
                    <ListItemIcon sx={{ minWidth: 36 }}>{item.icon}</ListItemIcon>
                    <ListItemText primary={isAr ? item.labelAr : item.label} />
                  </ListItemButton>
                );
              })}
            </List>
          </Box>
        ))}
      </Box>
      <Divider />
      <Box sx={{ p: 2 }}>
        <Typography variant="body2" color="text.secondary" noWrap>{user?.email}</Typography>
        <Box sx={{ display: 'flex', gap: 1, mt: 1 }}>
          <Button size="small" startIcon={<Home />} onClick={() => navigate('/')}>Site</Button>
          <Button size="small" startIcon={<Logout />} onClick={logout}>Logout</Button>
        </Box>
      </Box>
    </Box>
  );

  return (
    <Box sx={{ display: 'flex', minHeight: '100vh' }}>
      <AppBar
        position="fixed"
        elevation={0}
        sx={{
          width: { md: `calc(100% - ${DRAWER_WIDTH}px)` },
          ml: { md: `${DRAWER_WIDTH}px` },
          bgcolor: 'background.paper',
          color: 'text.primary',
          borderBottom: 1,
          borderColor: 'divider',
        }}
      >
        <Toolbar>
          {isMobile && (
            <IconButton edge="start" onClick={() => setMobileOpen(!mobileOpen)} sx={{ mr: 1 }}>
              <MenuIcon />
            </IconButton>
          )}
          <Typography variant="h6" sx={{ fontWeight: 600 }}>
            Enterprise Administration
          </Typography>
        </Toolbar>
      </AppBar>

      <Box component="nav" sx={{ width: { md: DRAWER_WIDTH }, flexShrink: { md: 0 } }}>
        <Drawer
          variant="temporary"
          open={mobileOpen}
          onClose={() => setMobileOpen(false)}
          ModalProps={{ keepMounted: true }}
          sx={{ display: { xs: 'block', md: 'none' }, '& .MuiDrawer-paper': { width: DRAWER_WIDTH } }}
        >
          {drawer}
        </Drawer>
        <Drawer
          variant="permanent"
          sx={{ display: { xs: 'none', md: 'block' }, '& .MuiDrawer-paper': { width: DRAWER_WIDTH, boxSizing: 'border-box' } }}
          open
        >
          {drawer}
        </Drawer>
      </Box>

      <Box
        component="main"
        sx={{
          flexGrow: 1,
          p: 3,
          width: { md: `calc(100% - ${DRAWER_WIDTH}px)` },
          mt: '64px',
          bgcolor: 'background.default',
          minHeight: 'calc(100vh - 64px)',
        }}
      >
        <Outlet />
      </Box>
    </Box>
  );
}
