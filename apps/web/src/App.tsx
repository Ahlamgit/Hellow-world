import type { ReactNode } from 'react';
import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import './i18n';
import { AppProviders } from './theme/AppProviders';
import { AuthProvider } from './auth/AuthContext';
import { AdminAuthProvider } from './auth/AdminAuthContext';
import { RequireRole, GuestOnly } from './auth/RequireRole';
import { AdminGuestOnly, RequireAdmin } from './auth/RequireAdmin';
import { PublicLayout } from './layouts/PublicLayout';
import { RoleShell } from './layouts/RoleShell';
import { HomePage } from './pages/HomePage';
import { ServicesPage } from './pages/ServicesPage';
import { ServiceDetailPage } from './pages/ServiceDetailPage';
import { CategoriesPage } from './pages/CategoriesPage';
import { HowItWorksPage } from './pages/HowItWorksPage';
import { LoginPage } from './pages/LoginPage';
import { SelectAccountPage } from './pages/SelectAccountPage';
import { RegisterPage } from './pages/RegisterPage';
import { RegisterVerifyPage } from './pages/RegisterVerifyPage';
import { ForgotPasswordPage } from './pages/ForgotPasswordPage';
import { LegalDocumentPage } from './pages/legal/LegalDocumentPage';
import { AdminLegalPage } from './pages/admin/AdminLegalPage';
import { AdminLoginPage } from './pages/AdminLoginPage';
import { BookServicePage } from './pages/BookServicePage';
import { LegacyCustomerRedirect } from './pages/LegacyCustomerRedirect';
import { ShellPage } from './pages/ShellPage';
import type { PortalRole } from './auth/types';

function portalRoutePath(portal: PortalRole): string {
  if (portal === 'customer') {
    return 'home';
  }
  return portal;
}

function portalRoutes(
  portal: PortalRole,
  pages: { path: string; titleKey: string; element?: ReactNode }[],
) {
  return (
    <Route key={portal} path={portalRoutePath(portal)}>
      <Route element={<RequireRole roles={[portal]} loginPath="/login" />}>
        <Route element={<RoleShell portal={portal} />}>
          {pages.map((page) => (
            <Route
              key={page.path || 'index'}
              index={page.path === ''}
              path={page.path || undefined}
              element={
                page.element ?? <ShellPage portal={portal} titleKey={page.titleKey} />
              }
            />
          ))}
        </Route>
      </Route>
    </Route>
  );
}

function App() {
  return (
    <AppProviders>
      <AuthProvider>
        <AdminAuthProvider>
          <BrowserRouter>
          <Routes>
            <Route element={<PublicLayout />}>
              <Route index element={<HomePage />} />
              <Route path="services" element={<ServicesPage />} />
              <Route path="services/:id" element={<ServiceDetailPage />} />
              <Route path="categories" element={<CategoriesPage />} />
              <Route path="how-it-works" element={<HowItWorksPage />} />
              <Route element={<RequireRole roles={['customer']} />}>
                <Route path="services/:id/book" element={<BookServicePage />} />
              </Route>
            </Route>

            <Route
              path="/login"
              element={
                <GuestOnly>
                  <LoginPage />
                </GuestOnly>
              }
            />
            <Route
              path="/login/select-account"
              element={
                <GuestOnly>
                  <SelectAccountPage />
                </GuestOnly>
              }
            />
            <Route
              path="/register"
              element={
                <GuestOnly>
                  <RegisterPage />
                </GuestOnly>
              }
            />
            <Route path="/register/verify" element={<RegisterVerifyPage />} />
            <Route
              path="/forgot-password"
              element={
                <GuestOnly>
                  <ForgotPasswordPage />
                </GuestOnly>
              }
            />
            <Route
              path="/admin/login"
              element={
                <AdminGuestOnly>
                  <AdminLoginPage />
                </AdminGuestOnly>
              }
            />

            <Route path="/legal/terms" element={<LegalDocumentPage type="terms" />} />
            <Route path="/legal/privacy" element={<LegalDocumentPage type="privacy" />} />

            {portalRoutes('customer', [
              { path: '', titleKey: 'nav.home' },
              { path: 'bookings', titleKey: 'nav.bookings' },
              { path: 'messages', titleKey: 'nav.messages' },
              { path: 'profile', titleKey: 'nav.profile' },
            ])}
            {portalRoutes('provider', [
              { path: '', titleKey: 'nav.dashboard' },
              { path: 'bookings', titleKey: 'nav.bookings' },
              { path: 'messages', titleKey: 'nav.messages' },
              { path: 'profile', titleKey: 'nav.profile' },
            ])}
            {portalRoutes('store', [
              { path: '', titleKey: 'nav.dashboard' },
              { path: 'services', titleKey: 'nav.services' },
              { path: 'analytics', titleKey: 'nav.analytics' },
              { path: 'settings', titleKey: 'nav.settings' },
            ])}

            <Route path="/admin" element={<RequireAdmin />}>
              <Route element={<RoleShell portal="admin" />}>
                <Route index element={<Navigate to="dashboard" replace />} />
                <Route path="dashboard" element={<ShellPage portal="admin" titleKey="nav.dashboard" />} />
                <Route path="legal" element={<AdminLegalPage />} />
                <Route path="users" element={<ShellPage portal="admin" titleKey="nav.users" />} />
                <Route path="finance" element={<ShellPage portal="admin" titleKey="nav.finance" />} />
                <Route path="settings" element={<ShellPage portal="admin" titleKey="nav.settings" />} />
              </Route>
            </Route>

            <Route path="/customer/*" element={<LegacyCustomerRedirect />} />

            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
          </BrowserRouter>
        </AdminAuthProvider>
      </AuthProvider>
    </AppProviders>
  );
}

export default App;
