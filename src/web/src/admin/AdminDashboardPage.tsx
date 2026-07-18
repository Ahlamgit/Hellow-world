import { useEffect, useState } from 'react';
import { Link as RouterLink } from 'react-router-dom';
import {
  Box, Button, Card, CardActionArea, CardContent, Grid, Typography, CircularProgress, Alert, LinearProgress, Stack,
} from '@mui/material';
import {
  People, Person, Engineering, Store, EventNote, CardMembership, AttachMoney,
  ReportProblem, SupportAgent, Notifications, Add,
} from '@mui/icons-material';
import { adminApi } from './adminApi';
import type { AdminDashboard } from './moduleConfig';

interface KpiCardProps {
  title: string;
  value: number | string;
  icon: React.ReactNode;
  color: string;
  subtitle?: string;
  to?: string;
}

function KpiCard({ title, value, icon, color, subtitle, to }: KpiCardProps) {
  const content = (
    <CardContent>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'flex-start' }}>
        <Box>
          <Typography variant="body2" color="text.secondary">{title}</Typography>
          <Typography variant="h4" sx={{ fontWeight: 700, mt: 0.5 }}>{value}</Typography>
          {subtitle && <Typography variant="caption" color="text.secondary">{subtitle}</Typography>}
        </Box>
        <Box sx={{ bgcolor: `${color}.light`, color: `${color}.main`, p: 1, borderRadius: 2, display: 'flex' }}>
          {icon}
        </Box>
      </Box>
    </CardContent>
  );

  return (
    <Card elevation={0} sx={{ border: 1, borderColor: 'divider', height: '100%' }}>
      {to ? (
        <CardActionArea component={RouterLink} to={to} sx={{ height: '100%' }}>
          {content}
        </CardActionArea>
      ) : content}
    </Card>
  );
}

export default function AdminDashboardPage() {
  const [dashboard, setDashboard] = useState<AdminDashboard | null>(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    adminApi.getDashboard()
      .then(({ data }) => setDashboard(data.data))
      .catch(() => setError('Failed to load dashboard'))
      .finally(() => setLoading(false));
  }, []);

  if (loading) return <Box sx={{ display: 'flex', justifyContent: 'center', py: 8 }}><CircularProgress /></Box>;
  if (error) return <Alert severity="error">{error}</Alert>;
  if (!dashboard) return null;

  const kpis: KpiCardProps[] = [
    { title: 'Total Users', value: dashboard.totalUsers, icon: <People />, color: 'primary', to: '/admin/users' },
    { title: 'Customers', value: dashboard.totalCustomers, icon: <Person />, color: 'info', to: '/admin/customers' },
    { title: 'Craftsmen', value: dashboard.totalCraftsmen, icon: <Engineering />, color: 'secondary', to: '/admin/craftsmen' },
    { title: 'Stores', value: dashboard.totalStores, icon: <Store />, color: 'warning', to: '/admin/stores' },
    { title: 'Total Bookings', value: dashboard.totalBookings, icon: <EventNote />, color: 'primary', subtitle: `${dashboard.pendingBookings} pending`, to: '/admin/bookings' },
    { title: 'Active Subscriptions', value: dashboard.activeSubscriptions, icon: <CardMembership />, color: 'success', to: '/admin/user-subscriptions' },
    { title: 'Total Revenue', value: `${dashboard.totalRevenue.toLocaleString()} SAR`, icon: <AttachMoney />, color: 'success', to: '/admin/payments' },
    { title: 'Open Complaints', value: dashboard.openComplaints, icon: <ReportProblem />, color: 'error', to: '/admin/complaints' },
    { title: 'Open Tickets', value: dashboard.openTickets, icon: <SupportAgent />, color: 'warning', to: '/admin/support-tickets' },
    { title: 'Unread Notifications', value: dashboard.unreadNotifications, icon: <Notifications />, color: 'info', to: '/admin/notifications' },
  ];

  return (
    <Box>
      <Box sx={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', flexWrap: 'wrap', gap: 2, mb: 3 }}>
        <Typography variant="h4" sx={{ fontWeight: 700 }}>Dashboard</Typography>
        <Stack direction="row" spacing={1} flexWrap="wrap">
          <Button component={RouterLink} to="/admin/users" variant="outlined" startIcon={<People />}>
            Manage users
          </Button>
          <Button component={RouterLink} to="/admin/subscriptions" variant="contained" startIcon={<Add />}>
            Subscription plans
          </Button>
        </Stack>
      </Box>

      <Grid container spacing={2}>
        {kpis.map((kpi) => (
          <Grid key={kpi.title} size={{ xs: 12, sm: 6, md: 4, lg: 3 }}>
            <KpiCard {...kpi} />
          </Grid>
        ))}
      </Grid>

      <Card elevation={0} sx={{ border: 1, borderColor: 'divider', mt: 3 }}>
        <CardContent>
          <Typography variant="h6" gutterBottom>Platform Overview</Typography>
          <Typography variant="body2" color="text.secondary" sx={{ mb: 2 }}>
            Use the sidebar or quick actions above to manage users, subscription plans, bookings, and all platform modules.
            Click any KPI card to open that section.
          </Typography>
          <LinearProgress variant="determinate" value={dashboard.totalUsers > 0 ? Math.min(100, (dashboard.activeSubscriptions / dashboard.totalUsers) * 100) : 0} sx={{ mb: 1 }} />
          <Typography variant="caption" color="text.secondary">
            Subscription adoption rate across users
          </Typography>
        </CardContent>
      </Card>
    </Box>
  );
}
