import type {
  ApiRole,
  AuthTokensResponse,
  LoginApiResponse,
  RegisterPayload,
} from './types';

const API_BASE = import.meta.env.VITE_API_URL ?? '/api/v1';

async function parseError(response: Response): Promise<string> {
  try {
    const body = (await response.json()) as { error?: string };
    return body.error ?? 'Request failed';
  } catch {
    return 'Request failed';
  }
}

export async function loginRequest(identifier: string, password: string): Promise<LoginApiResponse> {
  const response = await fetch(`${API_BASE}/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ identifier: identifier.trim(), password }),
  });
  if (!response.ok) {
    throw new Error(await parseError(response));
  }
  return (await response.json()) as LoginApiResponse;
}

export async function completeLoginRequest(
  identifier: string,
  password: string,
  role: ApiRole,
): Promise<AuthTokensResponse> {
  const response = await fetch(`${API_BASE}/auth/login/complete`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ identifier: identifier.trim(), password, role }),
  });
  if (!response.ok) {
    throw new Error(await parseError(response));
  }
  return (await response.json()) as AuthTokensResponse;
}

export async function registerRequest(payload: RegisterPayload): Promise<AuthTokensResponse> {
  const response = await fetch(`${API_BASE}/auth/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });
  if (!response.ok) {
    throw new Error(await parseError(response));
  }
  return (await response.json()) as AuthTokensResponse;
}

export async function forgotPasswordRequest(identifier: string): Promise<void> {
  const response = await fetch(`${API_BASE}/auth/password/forgot`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ identifier: identifier.trim() }),
  });
  if (!response.ok) {
    throw new Error(await parseError(response));
  }
}

export async function adminLoginRequest(identifier: string, password: string): Promise<AuthTokensResponse> {
  const response = await fetch(`${API_BASE}/auth/admin/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ identifier: identifier.trim(), password }),
  });
  if (!response.ok) {
    throw new Error(await parseError(response));
  }
  return (await response.json()) as AuthTokensResponse;
}

export async function verifyOtpRequest(phoneE164: string, otpCode: string): Promise<boolean> {
  const response = await fetch(`${API_BASE}/auth/otp/verify`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ phoneE164, otpCode }),
  });
  if (!response.ok) {
    throw new Error(await parseError(response));
  }
  const body = (await response.json()) as { key: string; value: boolean };
  return body.value;
}

export function isAdminApiRole(role: ApiRole): boolean {
  return role === 'ADMIN' || role === 'FINANCE_ADMIN' || role === 'SUPER_ADMIN';
}
