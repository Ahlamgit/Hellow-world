import { useParams } from 'react-router-dom';
import { Alert, Box } from '@mui/material';
import AdminDataTable from './AdminDataTable';
import AdminUsersPage from './AdminUsersPage';
import AdminUserSubscriptionsPage from './AdminUserSubscriptionsPage';
import AdminSubscriptionPlansPage from './AdminSubscriptionPlansPage';
import AdminSettingsPage from './AdminSettingsPage';
import AdminRbacMatrixPage from './AdminRbacMatrixPage';
import AdminCategoriesPage from './AdminCategoriesPage';
import AdminServicesPage from './AdminServicesPage';
import AdminRegionsPage from './AdminRegionsPage';
import AdminCitiesPage from './AdminCitiesPage';
import AdminCouponsPage from './AdminCouponsPage';
import AdminAdvertisementsPage from './AdminAdvertisementsPage';
import AdminBookingsPage from './AdminBookingsPage';
import AdminComplaintsPage from './AdminComplaintsPage';
import AdminSupportTicketsPage from './AdminSupportTicketsPage';
import AdminPaymentsPage from './AdminPaymentsPage';
import AdminVerificationDocumentsPage from './AdminVerificationDocumentsPage';
import AdminPageGuard from './AdminPageGuard';
import { getModuleConfig } from './moduleConfig';
import AdminBackupPage from './AdminSpecialPages';

export default function AdminModulePage() {
  const { moduleKey } = useParams<{ moduleKey: string }>();
  const config = moduleKey ? getModuleConfig(moduleKey) : undefined;

  if (moduleKey === 'users') {
    return (
      <AdminPageGuard key="users" module="users">
        <AdminUsersPage />
      </AdminPageGuard>
    );
  }

  if (moduleKey === 'customers') {
    return (
      <AdminPageGuard key="customers" module="customers">
        <AdminUsersPage title="Customers" defaultRoleFilter="Customer" lockRoleFilter defaultCreateRole="Customer" />
      </AdminPageGuard>
    );
  }

  if (moduleKey === 'craftsmen') {
    return (
      <AdminPageGuard key="craftsmen" module="craftsmen">
        <AdminUsersPage title="Craftsmen" defaultRoleFilter="Craftsman" lockRoleFilter defaultCreateRole="Craftsman" />
      </AdminPageGuard>
    );
  }

  if (moduleKey === 'stores') {
    return (
      <AdminPageGuard key="stores" module="stores">
        <AdminUsersPage title="Stores" defaultRoleFilter="StoreOwner" lockRoleFilter defaultCreateRole="StoreOwner" />
      </AdminPageGuard>
    );
  }

  if (moduleKey === 'user-subscriptions') {
    return (
      <AdminPageGuard module="user-subscriptions">
        <AdminUserSubscriptionsPage />
      </AdminPageGuard>
    );
  }

  if (moduleKey === 'subscriptions') {
    return (
      <AdminPageGuard module="subscriptions">
        <AdminSubscriptionPlansPage />
      </AdminPageGuard>
    );
  }

  if (moduleKey === 'settings') {
    return (
      <AdminPageGuard module="settings">
        <AdminSettingsPage />
      </AdminPageGuard>
    );
  }

  if (moduleKey === 'roles') {
    return (
      <AdminPageGuard module="roles">
        <AdminRbacMatrixPage />
      </AdminPageGuard>
    );
  }

  if (moduleKey === 'categories') {
    return (
      <AdminPageGuard module="categories">
        <AdminCategoriesPage />
      </AdminPageGuard>
    );
  }

  if (moduleKey === 'services') {
    return (
      <AdminPageGuard module="services">
        <AdminServicesPage />
      </AdminPageGuard>
    );
  }

  if (moduleKey === 'regions') {
    return (
      <AdminPageGuard module="regions">
        <AdminRegionsPage />
      </AdminPageGuard>
    );
  }

  if (moduleKey === 'cities') {
    return (
      <AdminPageGuard module="cities">
        <AdminCitiesPage />
      </AdminPageGuard>
    );
  }

  if (moduleKey === 'coupons') {
    return (
      <AdminPageGuard module="coupons">
        <AdminCouponsPage />
      </AdminPageGuard>
    );
  }

  if (moduleKey === 'advertisements') {
    return (
      <AdminPageGuard module="advertisements">
        <AdminAdvertisementsPage />
      </AdminPageGuard>
    );
  }

  if (moduleKey === 'bookings') {
    return (
      <AdminPageGuard module="bookings">
        <AdminBookingsPage />
      </AdminPageGuard>
    );
  }

  if (moduleKey === 'complaints') {
    return (
      <AdminPageGuard module="complaints">
        <AdminComplaintsPage />
      </AdminPageGuard>
    );
  }

  if (moduleKey === 'support-tickets') {
    return (
      <AdminPageGuard module="support-tickets">
        <AdminSupportTicketsPage />
      </AdminPageGuard>
    );
  }

  if (moduleKey === 'payments') {
    return (
      <AdminPageGuard module="payments">
        <AdminPaymentsPage />
      </AdminPageGuard>
    );
  }

  if (moduleKey === 'verification-documents') {
    return (
      <AdminPageGuard module="verification-documents">
        <AdminVerificationDocumentsPage />
      </AdminPageGuard>
    );
  }

  if (!config) {
    return <Alert severity="error">Module not found: {moduleKey}</Alert>;
  }

  if (moduleKey === 'backup') {
    return (
      <AdminPageGuard module="backup">
        <AdminBackupPage />
      </AdminPageGuard>
    );
  }

  return (
    <AdminPageGuard module={moduleKey!}>
      <Box>
        <AdminDataTable config={config} />
      </Box>
    </AdminPageGuard>
  );
}
