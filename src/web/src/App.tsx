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
          </Route>
          <Route element={<ProtectedRoute />}>
            <Route path="/dashboard" element={<DashboardPage />} />
            <Route path="/bookings" element={<MyBookingsPage />} />
            <Route path="/bookings/new" element={<BookingWizardPage />} />
            <Route path="/bookings/:id" element={<BookingDetailPage />} />
            <Route path="/bookings/:id/payment" element={<BookingPaymentPage />} />
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
