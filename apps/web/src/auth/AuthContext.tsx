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
  AuthRequestError,
  completeLoginRequest,
  isAdminApiRole,
  loginRequest,
  registerRequest,
  sessionRequest,
  verifyRegistrationOtpRequest,
} from './authApi';
import { apiRoleToPortal, portalDashboardPath } from './redirects';
import { clearSession, isAccessTokenExpired, loadSession, saveSession } from './session';
import type {
  ApiRole,
  AuthSession,
  AuthStatus,
  PortalRole,
  RegisterPayload,
  RegisterPendingResponse,
  SessionInfo,
} from './types';

export type LoginFlowResult =
  | { status: 'SUCCESS'; portal: PortalRole }
  | { status: 'ROLE_SELECTION_REQUIRED'; roles: ApiRole[] };

type AuthContextValue = {
  authStatus: AuthStatus;
  session: AuthSession | null;
  portal: PortalRole | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (identifier: string, password: string) => Promise<LoginFlowResult>;
  completeLogin: (identifier: string, password: string, role: ApiRole) => Promise<PortalRole>;
  register: (payload: RegisterPayload) => Promise<RegisterPendingResponse>;
  verifyRegistrationOtp: (email: string, role: ApiRole, otpCode: string) => Promise<PortalRole>;
  logout: () => void;
  refreshSession: () => Promise<boolean>;
};

const AuthContext = createContext<AuthContextValue | null>(null);

function sessionFromTokens(
  tokens: { accessToken: string; refreshToken: string; role: ApiRole },
  sessionInfo: SessionInfo,
): AuthSession {
  return {
    email: sessionInfo.email,
    accessToken: tokens.accessToken,
    refreshToken: tokens.refreshToken,
    role: sessionInfo.role,
    accountStatus: sessionInfo.accountStatus,
    firstName: sessionInfo.firstName,
    lastName: sessionInfo.lastName,
    phoneE164: sessionInfo.phoneE164,
  };
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [session, setSession] = useState<AuthSession | null>(null);
  const [authStatus, setAuthStatus] = useState<AuthStatus>('INITIALIZING');

  const logout = useCallback(() => {
    clearSession();
    setSession(null);
    setAuthStatus('UNAUTHENTICATED');
  }, []);

  const refreshSession = useCallback(async (): Promise<boolean> => {
    const stored = loadSession();
    if (!stored || isAccessTokenExpired(stored.accessToken) || isAdminApiRole(stored.role)) {
      logout();
      return false;
    }
    try {
      const sessionInfo = await sessionRequest(stored.accessToken);
      if (sessionInfo.accountStatus !== 'ACTIVE') {
        logout();
        return false;
      }
      const nextSession: AuthSession = {
        ...stored,
        email: sessionInfo.email,
        role: sessionInfo.role,
        accountStatus: sessionInfo.accountStatus,
        firstName: sessionInfo.firstName,
        lastName: sessionInfo.lastName,
        phoneE164: sessionInfo.phoneE164,
      };
      saveSession(nextSession);
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
      if (loadSession() === null) {
        setAuthStatus('UNAUTHENTICATED');
      }
    });
  }, [refreshSession]);

  const establishSession = useCallback(
    async (tokens: { accessToken: string; refreshToken: string; role: ApiRole }): Promise<PortalRole> => {
      if (isAdminApiRole(tokens.role)) {
        throw new Error('ADMIN_PORTAL_REQUIRED');
      }
      const portal = apiRoleToPortal(tokens.role);
      if (!portal) {
        throw new Error('UNSUPPORTED_ROLE');
      }
      const sessionInfo = await sessionRequest(tokens.accessToken);
      if (sessionInfo.accountStatus !== 'ACTIVE') {
        throw new AuthRequestError({
          error: 'Your account needs verification.',
          code: 'ACCOUNT_PENDING_VERIFICATION',
          email: sessionInfo.email,
          role: sessionInfo.role,
          phoneE164: sessionInfo.phoneE164,
        });
      }
      const nextSession = sessionFromTokens(tokens, sessionInfo);
      saveSession(nextSession);
      setSession(nextSession);
      setAuthStatus('AUTHENTICATED');
      return portal;
    },
    [],
  );

  const login = useCallback(async (identifier: string, password: string): Promise<LoginFlowResult> => {
    const response = await loginRequest(identifier, password);
    if (response.status === 'ROLE_SELECTION_REQUIRED') {
      return { status: 'ROLE_SELECTION_REQUIRED', roles: response.availableRoles };
    }
    if (!response.tokens) {
      throw new Error('LOGIN_FAILED');
    }
    const portal = await establishSession({
      accessToken: response.tokens.accessToken,
      refreshToken: response.tokens.refreshToken,
      role: response.tokens.role,
    });
    return { status: 'SUCCESS', portal };
  }, [establishSession]);

  const completeLogin = useCallback(
    async (identifier: string, password: string, role: ApiRole): Promise<PortalRole> => {
      const tokens = await completeLoginRequest(identifier, password, role);
      return establishSession(tokens);
    },
    [establishSession],
  );

  const register = useCallback(async (payload: RegisterPayload): Promise<RegisterPendingResponse> => {
    return registerRequest(payload);
  }, []);

  const verifyRegistrationOtp = useCallback(
    async (email: string, role: ApiRole, otpCode: string): Promise<PortalRole> => {
      const tokens = await verifyRegistrationOtpRequest(email, role, otpCode);
      return establishSession(tokens);
    },
    [establishSession],
  );

  const portal = session ? apiRoleToPortal(session.role) : null;
  const isLoading = authStatus === 'INITIALIZING';
  const isAuthenticated = authStatus === 'AUTHENTICATED' && session !== null && portal !== null;

  const value = useMemo<AuthContextValue>(
    () => ({
      authStatus,
      session,
      portal,
      isAuthenticated,
      isLoading,
      login,
      completeLogin,
      register,
      verifyRegistrationOtp,
      logout,
      refreshSession,
    }),
    [
      authStatus,
      session,
      portal,
      isAuthenticated,
      isLoading,
      login,
      completeLogin,
      register,
      verifyRegistrationOtp,
      logout,
      refreshSession,
    ],
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
