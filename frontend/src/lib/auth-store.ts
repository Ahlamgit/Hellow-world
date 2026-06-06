'use client';

import Cookies from 'js-cookie';
import { create } from 'zustand';
import { api } from './api';

interface User {
  id: string;
  email: string;
  role: string;
  name?: string;
}

interface AuthState {
  user: User | null;
  loading: boolean;
  login: (email: string, password: string) => Promise<void>;
  register: (name: string, email: string, password: string) => Promise<void>;
  logout: () => void;
  hydrate: () => void;
}

export const useAuthStore = create<AuthState>((set) => ({
  user: null,
  loading: false,

  hydrate: () => {
    const raw = Cookies.get('user');
    if (raw) {
      try {
        set({ user: JSON.parse(raw) });
      } catch {
        Cookies.remove('user');
      }
    }
  },

  login: async (email, password) => {
    set({ loading: true });
    try {
      const { data } = await api.post('/auth/login', { email, password });
      Cookies.set('accessToken', data.accessToken, { expires: 1 });
      Cookies.set('refreshToken', data.refreshToken, { expires: 7 });
      Cookies.set('user', JSON.stringify(data.user), { expires: 7 });
      set({ user: data.user });
    } finally {
      set({ loading: false });
    }
  },

  register: async (name, email, password) => {
    set({ loading: true });
    try {
      await api.post('/auth/register', { name, email, password });
    } finally {
      set({ loading: false });
    }
  },

  logout: () => {
    const refreshToken = Cookies.get('refreshToken');
    if (refreshToken) {
      api.post('/auth/logout', { refreshToken }).catch(() => undefined);
    }
    Cookies.remove('accessToken');
    Cookies.remove('refreshToken');
    Cookies.remove('user');
    set({ user: null });
  },
}));
