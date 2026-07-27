import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react';
import { isAdminApiRole, loginRequest, verifyOtpRequest } from './authApi';
import { apiRoleToPortal, portalDashboardPath } from './redirects';
import { clearSession, isAccessTokenExpired, loadSession, saveSession } from './session';
import type { AuthSession, PortalRole } from './types';

type AuthContextValue = {
  session: AuthSession | null;
  portal: PortalRole | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (email: string, password: string) => Promise<PortalRole>;
  adminLogin: (email: string, password: string, otpCode: string, phoneE164: string) => Promise<void>;
  logout: () => void;
};

const AuthContext = createContext<AuthContextValue | null>(null);

function completeSession(email: string, tokens: { accessToken: string; refreshToken: string; role: AuthSession['role'] }): AuthSession {
  const session: AuthSession = {
    email,
    accessToken: tokens.accessToken,
    refreshToken: tokens.refreshToken,
    role: tokens.role,
  };
  saveSession(session);
  return session;
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<AuthSession | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const stored = loadSession();
    if (stored && !isAccessTokenExpired(stored.accessToken)) {
      setSession(stored);
    } else if (stored) {
      clearSession();
    }
    setIsLoading(false);
  }, []);

  const logout = useCallback(() => {
    clearSession();
    setSession(null);
  }, []);

  const login = useCallback(async (email: string, password: string) => {
    const tokens = await loginRequest(email, password);
    if (isAdminApiRole(tokens.role)) {
      throw new Error('ADMIN_PORTAL_REQUIRED');
    }
    const portal = apiRoleToPortal(tokens.role);
    if (!portal) {
      throw new Error('UNSUPPORTED_ROLE');
    }
    const nextSession = completeSession(email, tokens);
    setSession(nextSession);
    return portal;
  }, []);

  const adminLogin = useCallback(async (email: string, password: string, otpCode: string, phoneE164: string) => {
    const tokens = await loginRequest(email, password);
    if (!isAdminApiRole(tokens.role)) {
      throw new Error('NOT_ADMIN');
    }
    const valid = await verifyOtpRequest(phoneE164, otpCode);
    if (!valid) {
      throw new Error('INVALID_OTP');
    }
    const nextSession = completeSession(email, tokens);
    setSession(nextSession);
  }, []);

  const portal = session ? apiRoleToPortal(session.role) : null;

  const value = useMemo<AuthContextValue>(
    () => ({
      session,
      portal,
      isAuthenticated: session !== null && portal !== null,
      isLoading,
      login,
      adminLogin,
      logout,
    }),
    [session, portal, isLoading, login, adminLogin, logout],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth(): AuthContextValue {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within AuthProvider');
  }
  return context;
}

export function useDashboardPath(): string {
  const { portal } = useAuth();
  return portal ? portalDashboardPath(portal) : '/';
}
