import { Routes, Route, useLocation } from 'react-router-dom';
import { Box } from '@mui/material';
import { AuthProvider } from './context/AuthContext';
import Navbar from './components/Navbar';
import { ProtectedRoute, PublicRoute } from './components/ProtectedRoute';
import { AdminRoute } from './admin/AdminRoute';
import AdminRoutes from './admin/AdminRoutes';
import HomePage from './pages/HomePage';
import ServicesPage from './pages/ServicesPage';
import DashboardPage from './pages/DashboardPage';
import LoginPage, { RegisterPage } from './pages/AuthPages';
import { BookingWizardPage, MyBookingsPage, BookingDetailPage, BookingPaymentPage } from './pages/BookingPages';
import { SubscriptionPlansPage, SubscribePage, MySubscriptionPage } from './pages/SubscriptionPages';
import {
  ForgotPasswordPage, ResetPasswordPage, VerifyEmailPage, ChangePasswordPage, SessionsPage,
} from './pages/IdentityPages';
import ProfilePage from './pages/ProfilePage';

function AppContent() {
  const location = useLocation();
  const isAdmin = location.pathname.startsWith('/admin');

  return (
    <Box sx={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
      {!isAdmin && <Navbar />}
      <Box component="main" sx={{ flexGrow: 1 }}>
        <Routes>
          <Route path="/" element={<HomePage />} />
          <Route path="/services" element={<ServicesPage />} />
          <Route element={<PublicRoute />}>
            <Route path="/login" element={<LoginPage />} />
            <Route path="/register" element={<RegisterPage />} />
            <Route path="/forgot-password" element={<ForgotPasswordPage />} />
            <Route path="/reset-password" element={<ResetPasswordPage />} />
            <Route path="/verify-email" element={<VerifyEmailPage />} />
          </Route>
          <Route element={<ProtectedRoute />}>
            <Route path="/dashboard" element={<DashboardPage />} />
            <Route path="/profile" element={<ProfilePage />} />
            <Route path="/change-password" element={<ChangePasswordPage />} />
            <Route path="/sessions" element={<SessionsPage />} />
            <Route path="/bookings" element={<MyBookingsPage />} />
            <Route path="/bookings/new" element={<BookingWizardPage />} />
            <Route path="/bookings/:id" element={<BookingDetailPage />} />
            <Route path="/bookings/:id/payment" element={<BookingPaymentPage />} />
            <Route path="/subscriptions" element={<SubscriptionPlansPage />} />
            <Route path="/subscriptions/:planId" element={<SubscribePage />} />
            <Route path="/subscription" element={<MySubscriptionPage />} />
          </Route>
          <Route element={<AdminRoute />}>
            <Route path="/admin/*" element={<AdminRoutes />} />
          </Route>
        </Routes>
      </Box>
    </Box>
  );
}

function App() {
  return (
    <AuthProvider>
      <AppContent />
    </AuthProvider>
  );
}

export default App;
