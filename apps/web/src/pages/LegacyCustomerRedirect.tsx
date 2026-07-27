import { Navigate, useLocation } from 'react-router-dom';

export function LegacyCustomerRedirect() {
  const location = useLocation();
  const suffix = location.pathname.replace(/^\/customer/, '');
  return <Navigate to={`/home${suffix}${location.search}`} replace />;
}
