import { Routes, Route } from 'react-router-dom';
import { Box } from '@mui/material';
import { AuthProvider } from './context/AuthContext';
import Navbar from './components/Navbar';
import { ProtectedRoute, PublicRoute } from './components/ProtectedRoute';
import HomePage from './pages/HomePage';
import ServicesPage from './pages/ServicesPage';
import DashboardPage from './pages/DashboardPage';
import LoginPage, { RegisterPage } from './pages/AuthPages';
import { BookingWizardPage, MyBookingsPage, BookingDetailPage, BookingPaymentPage } from './pages/BookingPages';

function App() {
  return (
    <AuthProvider>
      <Box sx={{ minHeight: '100vh', display: 'flex', flexDirection: 'column' }}>
        <Navbar />
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
          </Routes>
        </Box>
      </Box>
    </AuthProvider>
  );
}

export default App;
