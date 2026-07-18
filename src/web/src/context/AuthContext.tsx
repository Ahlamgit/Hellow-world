import { createContext, useContext, useState, useEffect, type ReactNode } from 'react';
import { type UserDto, authApi, identityApi } from '../services/api';

interface AuthContextType {
  user: UserDto | null;
  isAuthenticated: boolean;
  isLoading: boolean;
  login: (email: string, password: string, rememberMe?: boolean) => Promise<void>;
  register: (data: Record<string, string>) => Promise<void>;
  logout: () => void;
  refreshUser: () => Promise<void>;
}

const AuthContext = createContext<AuthContextType | null>(null);

function persistAuth(data: { accessToken: string; refreshToken: string; sessionId: string; user: UserDto }) {
  localStorage.setItem('accessToken', data.accessToken);
  localStorage.setItem('refreshToken', data.refreshToken);
  localStorage.setItem('sessionId', data.sessionId);
  localStorage.setItem('user', JSON.stringify(data.user));
  localStorage.setItem('userRole', data.user.role || data.user.primaryRole || '');
}

async function loadPermissions(user: UserDto): Promise<UserDto> {
  try {
    const { data } = await authApi.getPermissions();
    return { ...user, permissions: data.data };
  } catch {
    try {
      const { data } = await authApi.getMe();
      return { ...user, permissions: data.data.permissions };
    } catch {
      return user;
    }
  }
}

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<UserDto | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const init = async () => {
      const stored = localStorage.getItem('user');
      const token = localStorage.getItem('accessToken');
      if (stored && token) {
        const parsed: UserDto = JSON.parse(stored);
        const withPermissions = await loadPermissions(parsed);
        setUser(withPermissions);
        localStorage.setItem('user', JSON.stringify(withPermissions));
      }
      setIsLoading(false);
    };
    init();
  }, []);

  const login = async (email: string, password: string, rememberMe = false) => {
    const { data } = await authApi.login(email, password, rememberMe);
    const withPermissions = await loadPermissions(data.data.user);
    persistAuth({ ...data.data, user: withPermissions });
    setUser(withPermissions);
  };

  const register = async (formData: Record<string, string>) => {
    const { data } = await authApi.register(formData);
    const withPermissions = await loadPermissions(data.data.user);
    persistAuth({ ...data.data, user: withPermissions });
    setUser(withPermissions);
  };

  const logout = () => {
    authApi.logout().catch(() => {});
    localStorage.clear();
    setUser(null);
  };

  const refreshUser = async () => {
    const { data } = await identityApi.getProfile();
    const profile = data.data;
    setUser((current) => {
      if (!current) return current;
      const updated: UserDto = {
        ...current,
        firstName: profile.firstName,
        lastName: profile.lastName,
        preferredLanguage: profile.preferredLanguage,
        emailVerified: profile.emailVerified,
        phoneVerified: profile.phoneVerified,
      };
      localStorage.setItem('user', JSON.stringify(updated));
      loadPermissions(updated).then((withPermissions) => {
        setUser(withPermissions);
        localStorage.setItem('user', JSON.stringify(withPermissions));
      });
      return updated;
    });
  };

  return (
    <AuthContext.Provider value={{ user, isAuthenticated: !!user, isLoading, login, register, logout, refreshUser }}>
      {children}
    </AuthContext.Provider>
  );
}

export const useAuth = () => {
  const ctx = useContext(AuthContext);
  if (!ctx) throw new Error('useAuth must be used within AuthProvider');
  return ctx;
};
