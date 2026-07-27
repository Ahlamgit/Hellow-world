import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import './i18n';
import { AppProviders } from './theme/AppProviders';
import { AuthProvider } from './auth/AuthContext';
import { RequireRole, GuestOnly } from './auth/RequireRole';
import { PublicLayout } from './layouts/PublicLayout';
import { RoleShell } from './layouts/RoleShell';
import { HomePage } from './pages/HomePage';
import { ServicesPage } from './pages/ServicesPage';
import { ServiceDetailPage } from './pages/ServiceDetailPage';
import { CategoriesPage } from './pages/CategoriesPage';
import { HowItWorksPage } from './pages/HowItWorksPage';
import { LoginPage } from './pages/LoginPage';
import { AdminLoginPage } from './pages/AdminLoginPage';
import { BookServicePage } from './pages/BookServicePage';
import { ShellPage } from './pages/ShellPage';
import type { PortalRole } from './auth/types';

function portalRoutes(portal: PortalRole, pages: { path: string; titleKey: string }[]) {
  return (
    <Route key={portal} path={portal}>
      <Route element={<RequireRole roles={[portal]} loginPath={portal === 'admin' ? '/admin/login' : '/login'} />}>
        <Route element={<RoleShell portal={portal} />}>
          {pages.map((page) => (
            <Route
              key={page.path || 'index'}
              index={page.path === ''}
              path={page.path || undefined}
              element={<ShellPage portal={portal} titleKey={page.titleKey} />}
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
            <Route path="/admin/login" element={<AdminLoginPage />} />

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
            {portalRoutes('admin', [
              { path: '', titleKey: 'nav.dashboard' },
              { path: 'users', titleKey: 'nav.users' },
              { path: 'finance', titleKey: 'nav.finance' },
              { path: 'settings', titleKey: 'nav.settings' },
            ])}

            <Route path="*" element={<Navigate to="/" replace />} />
          </Routes>
        </BrowserRouter>
      </AuthProvider>
    </AppProviders>
  );
}

export default App;
