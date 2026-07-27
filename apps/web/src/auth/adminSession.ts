import type { AdminSession } from './adminTypes';

const ADMIN_SESSION_KEY = 'khadamati_admin_session';

export function saveAdminSession(session: AdminSession): void {
  sessionStorage.setItem(ADMIN_SESSION_KEY, JSON.stringify(session));
}

export function loadAdminSession(): AdminSession | null {
  const raw = sessionStorage.getItem(ADMIN_SESSION_KEY);
  if (!raw) {
    return null;
  }
  try {
    return JSON.parse(raw) as AdminSession;
  } catch {
    return null;
  }
}

export function clearAdminSession(): void {
  sessionStorage.removeItem(ADMIN_SESSION_KEY);
}
