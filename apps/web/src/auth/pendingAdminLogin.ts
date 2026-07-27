export type PendingAdminLogin = {
  email: string;
  phoneE164: string;
  returnUrl?: string;
};

const PENDING_ADMIN_LOGIN_KEY = 'khadamati_pending_admin_login';

export function savePendingAdminLogin(pending: PendingAdminLogin): void {
  sessionStorage.setItem(PENDING_ADMIN_LOGIN_KEY, JSON.stringify(pending));
}

export function loadPendingAdminLogin(): PendingAdminLogin | null {
  const raw = sessionStorage.getItem(PENDING_ADMIN_LOGIN_KEY);
  if (!raw) return null;
  try {
    return JSON.parse(raw) as PendingAdminLogin;
  } catch {
    return null;
  }
}

export function clearPendingAdminLogin(): void {
  sessionStorage.removeItem(PENDING_ADMIN_LOGIN_KEY);
}
