import type { ApiRole } from './types';

const PENDING_LOGIN_KEY = 'khadamati_pending_login';

export type PendingLogin = {
  identifier: string;
  password: string;
  roles: ApiRole[];
  returnUrl?: string;
};

export function savePendingLogin(pending: PendingLogin): void {
  sessionStorage.setItem(PENDING_LOGIN_KEY, JSON.stringify(pending));
}

export function loadPendingLogin(): PendingLogin | null {
  const raw = sessionStorage.getItem(PENDING_LOGIN_KEY);
  if (!raw) {
    return null;
  }
  try {
    return JSON.parse(raw) as PendingLogin;
  } catch {
    return null;
  }
}

export function clearPendingLogin(): void {
  sessionStorage.removeItem(PENDING_LOGIN_KEY);
}
