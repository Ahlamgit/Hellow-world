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

export function AuthProvider({ children }: { children: ReactNode }) {
  const [user, setUser] = useState<UserDto | null>(null);
  const [isLoading, setIsLoading] = useState(true);

  useEffect(() => {
    const stored = localStorage.getItem('user');
    if (stored) setUser(JSON.parse(stored));
    setIsLoading(false);
  }, []);

  const login = async (email: string, password: string, rememberMe = false) => {
    const { data } = await authApi.login(email, password, rememberMe);
    persistAuth(data.data);
    setUser(data.data.user);
  };

  const register = async (formData: Record<string, string>) => {
    const { data } = await authApi.register(formData);
    persistAuth(data.data);
    setUser(data.data.user);
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
