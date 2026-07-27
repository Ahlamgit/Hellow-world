import type { ApiRole } from './types';

const PENDING_REGISTRATION_KEY = 'khadamati_pending_registration';

export type PendingRegistration = {
  email: string;
  role: ApiRole;
  phoneE164: string;
  returnUrl?: string;
};

export function savePendingRegistration(pending: PendingRegistration): void {
  sessionStorage.setItem(PENDING_REGISTRATION_KEY, JSON.stringify(pending));
}

export function loadPendingRegistration(): PendingRegistration | null {
  const raw = sessionStorage.getItem(PENDING_REGISTRATION_KEY);
  if (!raw) return null;
  try {
    return JSON.parse(raw) as PendingRegistration;
  } catch {
    return null;
  }
}

export function clearPendingRegistration(): void {
  sessionStorage.removeItem(PENDING_REGISTRATION_KEY);
}
