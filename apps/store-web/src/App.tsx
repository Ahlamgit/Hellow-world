import { BrowserRouter, Navigate, Route, Routes } from 'react-router-dom';
import './i18n';
import { AppLayout } from './layouts/AppLayout';
import { ShellPage } from './pages/ShellPage';

function App() {
  return (
    <BrowserRouter>
      <Routes>
        <Route element={<AppLayout />}>
          <Route index element={<ShellPage titleKey="nav.dashboard" />} />
          <Route path="services" element={<ShellPage titleKey="nav.services" />} />
          <Route path="analytics" element={<ShellPage titleKey="nav.analytics" />} />
          <Route path="settings" element={<ShellPage titleKey="nav.settings" />} />
          <Route path="*" element={<Navigate to="/" replace />} />
        </Route>
      </Routes>
    </BrowserRouter>
  );
}

export default App;
