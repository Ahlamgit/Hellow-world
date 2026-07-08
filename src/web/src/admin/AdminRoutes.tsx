import { Routes, Route, Navigate } from 'react-router-dom';
import AdminLayout from './AdminLayout';
import AdminDashboardPage from './AdminDashboardPage';
import AdminModulePage from './AdminModulePage';
import AdminPageGuard from './AdminPageGuard';
import {
  AdminAnalyticsPage, AdminSystemHealthPage, AdminRestorePage, AdminReportsPage,
} from './AdminSpecialPages';

export default function AdminRoutes() {
  return (
    <Routes>
      <Route element={<AdminLayout />}>
        <Route index element={<AdminPageGuard path="/admin"><AdminDashboardPage /></AdminPageGuard>} />
        <Route path="analytics" element={<AdminPageGuard path="/admin/analytics"><AdminAnalyticsPage /></AdminPageGuard>} />
        <Route path="reports" element={<AdminPageGuard path="/admin/reports"><AdminReportsPage /></AdminPageGuard>} />
        <Route path="system-health" element={<AdminPageGuard path="/admin/system-health"><AdminSystemHealthPage /></AdminPageGuard>} />
        <Route path="restore" element={<AdminPageGuard module="restore"><AdminRestorePage /></AdminPageGuard>} />
        <Route path=":moduleKey" element={<AdminModulePage />} />
        <Route path="*" element={<Navigate to="/admin" replace />} />
      </Route>
    </Routes>
  );
}
