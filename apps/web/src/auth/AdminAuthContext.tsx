import {
  createContext,
  useCallback,
  useContext,
  useEffect,
  useMemo,
  useState,
  type ReactNode,
} from 'react';
import {
  adminLoginRequest,
  adminSessionRequest,
  adminVerifyMfaRequest,
} from './adminAuthApi';
import { clearAdminSession, loadAdminSession, saveAdminSession } from './adminSession';
import type { AdminAuthStatus, AdminSession } from './adminTypes';
import { isAccessTokenExpired } from './session';

type AdminAuthContextValue = {
  authStatus: AdminAuthStatus;
  session: AdminSession | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (email: string, password: string) => Promise<{ email: string; phoneE164: string }>;
  verifyMfa: (email: string, otpCode: string) => Promise<void>;
  logout: () => void;
  refreshSession: () => Promise<boolean>;
};

const AdminAuthContext = createContext<AdminAuthContextValue | null>(null);

function sessionFromInfo(
  tokens: { accessToken: string; refreshToken: string },
  info: Awaited<ReturnType<typeof adminSessionRequest>>,
): AdminSession {
  return {
    email: info.email,
    accessToken: tokens.accessToken,
    refreshToken: tokens.refreshToken,
    role: 'ADMIN',
    accountStatus: 'ACTIVE',
    firstName: info.firstName,
    lastName: info.lastName,
    phoneE164: info.phoneE164,
    mfaVerified: info.mfaVerified,
  };
}

export function AdminAuthProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<AdminSession | null>(null);
  const [authStatus, setAuthStatus] = useState<AdminAuthStatus>('INITIALIZING');

  const logout = useCallback(() => {
    clearAdminSession();
    setSession(null);
    setAuthStatus('UNAUTHENTICATED');
  }, []);

  const refreshSession = useCallback(async (): Promise<boolean> => {
    const stored = loadAdminSession();
    if (!stored || isAccessTokenExpired(stored.accessToken)) {
      logout();
      return false;
    }
    try {
      const info = await adminSessionRequest(stored.accessToken);
      if (!info.mfaVerified || info.accountStatus !== 'ACTIVE') {
        logout();
        return false;
      }
      const nextSession = sessionFromInfo(
        { accessToken: stored.accessToken, refreshToken: stored.refreshToken },
        info,
      );
      saveAdminSession(nextSession);
      setSession(nextSession);
      setAuthStatus('AUTHENTICATED');
      return true;
    } catch {
      logout();
      return false;
    }
  }, [logout]);

  useEffect(() => {
    void refreshSession().finally(() => {
      if (loadAdminSession() === null) {
        setAuthStatus('UNAUTHENTICATED');
      }
    });
  }, [refreshSession]);

  const login = useCallback(async (email: string, password: string) => {
    const pending = await adminLoginRequest(email, password);
    return { email: pending.email, phoneE164: pending.phoneE164 };
  }, []);

  const verifyMfa = useCallback(async (email: string, otpCode: string) => {
    const tokens = await adminVerifyMfaRequest(email, otpCode);
    const info = await adminSessionRequest(tokens.accessToken);
    if (!info.mfaVerified) {
      throw new Error('MFA_REQUIRED');
    }
    const nextSession = sessionFromInfo(tokens, info);
    saveAdminSession(nextSession);
    setSession(nextSession);
    setAuthStatus('AUTHENTICATED');
  }, []);

  const isLoading = authStatus === 'INITIALIZING';
  const isAuthenticated = authStatus === 'AUTHENTICATED' && session !== null && session.mfaVerified;

  const value = useMemo<AdminAuthContextValue>(
    () => ({
      authStatus,
      session,
      isAuthenticated,
      isLoading,
      login,
      verifyMfa,
      logout,
      refreshSession,
    }),
    [authStatus, session, isAuthenticated, isLoading, login, verifyMfa, logout, refreshSession],
  );

  return <AdminAuthContext.Provider value={value}>{children}</AdminAuthContext.Provider>;
}

export function useAdminAuth(): AdminAuthContextValue {
  const context = useContext(AdminAuthContext);
  if (!context) {
    throw new Error('useAdminAuth must be used within AdminAuthProvider');
  }
  return context;
}
