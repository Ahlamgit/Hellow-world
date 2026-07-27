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
  completeLoginRequest,
  adminLoginRequest,
  isAdminApiRole,
  loginRequest,
  registerRequest,
  verifyOtpRequest,
} from './authApi';
import { verifyRegistrationOtpRequest } from './legalApi';
import { apiRoleToPortal, portalDashboardPath } from './redirects';
import { clearSession, isAccessTokenExpired, loadSession, saveSession } from './session';
import type { ApiRole, AuthSession, PortalRole, RegisterPayload, RegisterPendingResponse } from './types';

export type LoginFlowResult =
  | { status: 'SUCCESS'; portal: PortalRole }
  | { status: 'ROLE_SELECTION_REQUIRED'; roles: ApiRole[] };

type AuthContextValue = {
  session: AuthSession | null;
  portal: PortalRole | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (identifier: string, password: string) => Promise<LoginFlowResult>;
  completeLogin: (identifier: string, password: string, role: ApiRole) => Promise<PortalRole>;
  register: (payload: RegisterPayload) => Promise<RegisterPendingResponse>;
  verifyRegistrationOtp: (email: string, role: ApiRole, otpCode: string) => Promise<PortalRole>;
  adminLogin: (email: string, password: string, otpCode: string, phoneE164: string) => Promise<void>;
  logout: () => void;
  establishSession: (email: string, tokens: { accessToken: string; refreshToken: string; role: ApiRole }) => PortalRole;
};

const AuthContext = createContext<AuthContextValue | null>(null);

function subjectFromToken(accessToken: string): string {
  try {
    const payload = JSON.parse(atob(accessToken.split('.')[1])) as { sub?: string };
    return payload.sub ?? '';
  } catch {
    return '';
  }
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

  const establishSession = useCallback(
    (fallbackEmail: string, tokens: { accessToken: string; refreshToken: string; role: ApiRole }): PortalRole => {
      if (isAdminApiRole(tokens.role)) {
        throw new Error('ADMIN_PORTAL_REQUIRED');
      }
      const portal = apiRoleToPortal(tokens.role);
      if (!portal) {
        throw new Error('UNSUPPORTED_ROLE');
      }
      const sessionEmail = subjectFromToken(tokens.accessToken) || fallbackEmail.trim().toLowerCase();
      const nextSession: AuthSession = {
        email: sessionEmail,
        accessToken: tokens.accessToken,
        refreshToken: tokens.refreshToken,
        role: tokens.role,
      };
      saveSession(nextSession);
      setSession(nextSession);
      return portal;
    },
    [],
  );

  const logout = useCallback(() => {
    clearSession();
    setSession(null);
  }, []);

  const login = useCallback(async (identifier: string, password: string): Promise<LoginFlowResult> => {
    const response = await loginRequest(identifier, password);
    if (response.status === 'ROLE_SELECTION_REQUIRED') {
      return { status: 'ROLE_SELECTION_REQUIRED', roles: response.availableRoles };
    }
    if (!response.tokens) {
      throw new Error('LOGIN_FAILED');
    }
    const portal = establishSession(identifier.includes('@') ? identifier.trim().toLowerCase() : identifier.trim(), {
      accessToken: response.tokens.accessToken,
      refreshToken: response.tokens.refreshToken,
      role: response.tokens.role,
    });
    return { status: 'SUCCESS', portal };
  }, [establishSession]);

  const completeLogin = useCallback(
    async (identifier: string, password: string, role: ApiRole): Promise<PortalRole> => {
      const tokens = await completeLoginRequest(identifier, password, role);
      return establishSession(
        identifier.includes('@') ? identifier.trim().toLowerCase() : identifier.trim(),
        tokens,
      );
    },
    [establishSession],
  );

  const register = useCallback(async (payload: RegisterPayload): Promise<RegisterPendingResponse> => {
    return registerRequest(payload);
  }, []);

  const verifyRegistrationOtp = useCallback(
    async (email: string, role: ApiRole, otpCode: string): Promise<PortalRole> => {
      const tokens = await verifyRegistrationOtpRequest(email, role, otpCode);
      return establishSession(email.trim().toLowerCase(), tokens);
    },
    [establishSession],
  );

  const adminLogin = useCallback(async (email: string, password: string, otpCode: string, phoneE164: string) => {
    const tokens = await adminLoginRequest(email, password);
    if (!isAdminApiRole(tokens.role)) {
      throw new Error('NOT_ADMIN');
    }
    const valid = await verifyOtpRequest(phoneE164, otpCode);
    if (!valid) {
      throw new Error('INVALID_OTP');
    }
    const nextSession: AuthSession = {
      email: subjectFromToken(tokens.accessToken) || email.trim().toLowerCase(),
      accessToken: tokens.accessToken,
      refreshToken: tokens.refreshToken,
      role: tokens.role,
    };
    saveSession(nextSession);
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
      completeLogin,
      register,
      verifyRegistrationOtp,
      adminLogin,
      logout,
      establishSession,
    }),
    [session, portal, isLoading, login, completeLogin, register, verifyRegistrationOtp, adminLogin, logout, establishSession],
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
