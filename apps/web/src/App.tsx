import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import './i18n';
import { AppProviders } from './theme/AppProviders';
import { PublicLayout } from './layouts/PublicLayout';
import { ActorShell } from './layouts/ActorShell';
import { HomePage } from './pages/HomePage';
import { ServicesPage } from './pages/ServicesPage';
import { ServiceDetailPage } from './pages/ServiceDetailPage';
import { CategoriesPage } from './pages/CategoriesPage';
import { HowItWorksPage } from './pages/HowItWorksPage';
import { LoginPage } from './pages/LoginPage';
import { ShellPage } from './pages/ShellPage';
import { RequireActor } from './auth/RequireActor';
import type { ActorType } from './auth/actor';

function actorRoutes(actor: ActorType, pages: { path: string; titleKey: string }[]) {
  return (
    <Route element={<RequireActor actor={actor} />}>
      <Route element={<ActorShell actor={actor} />}>
        {pages.map((page) => (
          <Route
            key={page.path || 'index'}
            index={page.path === ''}
            path={page.path || undefined}
            element={<ShellPage actor={actor} titleKey={page.titleKey} />}
          />
        ))}
      </Route>
    </Route>
  );
}

function App() {
  return (
    <AppProviders>
      <BrowserRouter>
        <Routes>
          <Route element={<PublicLayout />}>
            <Route index element={<HomePage />} />
            <Route path="services" element={<ServicesPage />} />
            <Route path="services/:id" element={<ServiceDetailPage />} />
            <Route path="categories" element={<CategoriesPage />} />
            <Route path="how-it-works" element={<HowItWorksPage />} />
          </Route>
          <Route path="/login" element={<LoginPage />} />
          {actorRoutes('customer', [
            { path: '', titleKey: 'nav.home' },
            { path: 'bookings', titleKey: 'nav.bookings' },
            { path: 'messages', titleKey: 'nav.messages' },
            { path: 'profile', titleKey: 'nav.profile' },
          ])}
          {actorRoutes('craftsman', [
            { path: '', titleKey: 'nav.dashboard' },
            { path: 'bookings', titleKey: 'nav.bookings' },
            { path: 'messages', titleKey: 'nav.messages' },
            { path: 'profile', titleKey: 'nav.profile' },
          ])}
          {actorRoutes('store', [
            { path: '', titleKey: 'nav.dashboard' },
            { path: 'services', titleKey: 'nav.services' },
            { path: 'analytics', titleKey: 'nav.analytics' },
            { path: 'settings', titleKey: 'nav.settings' },
          ])}
          {actorRoutes('admin', [
            { path: '', titleKey: 'nav.dashboard' },
            { path: 'users', titleKey: 'nav.users' },
            { path: 'finance', titleKey: 'nav.finance' },
            { path: 'settings', titleKey: 'nav.settings' },
          ])}
          <Route path="*" element={<Navigate to="/" replace />} />
        </Routes>
      </BrowserRouter>
    </AppProviders>
  );
}

export default App;
