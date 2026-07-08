import { Routes, Route, Navigate } from 'react-router-dom';
import AdminLayout from './AdminLayout';
import AdminDashboardPage from './AdminDashboardPage';
import AdminModulePage from './AdminModulePage';
import {
  AdminAnalyticsPage, AdminSystemHealthPage, AdminRestorePage, AdminReportsPage,
} from './AdminSpecialPages';

export default function AdminRoutes() {
  return (
    <Routes>
      <Route element={<AdminLayout />}>
        <Route index element={<AdminDashboardPage />} />
        <Route path="analytics" element={<AdminAnalyticsPage />} />
        <Route path="reports" element={<AdminReportsPage />} />
        <Route path="system-health" element={<AdminSystemHealthPage />} />
        <Route path="restore" element={<AdminRestorePage />} />
        <Route path=":moduleKey" element={<AdminModulePage />} />
        <Route path="*" element={<Navigate to="/admin" replace />} />
      </Route>
    </Routes>
  );
}
