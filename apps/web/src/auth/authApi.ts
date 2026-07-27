import type {
  ApiRole,
  AuthErrorBody,
  AuthTokensResponse,
  LoginApiResponse,
  RegisterPayload,
  RegisterPendingResponse,
  SessionInfo,
} from './types';

const API_BASE = import.meta.env.VITE_API_URL ?? '/api/v1';

export class AuthRequestError extends Error {
  readonly code?: string;
  readonly body: AuthErrorBody;

  constructor(body: AuthErrorBody) {
    super(body.error ?? 'Request failed');
    this.code = body.code;
    this.body = body;
  }
}

async function parseAuthError(response: Response): Promise<AuthRequestError> {
  try {
    const body = (await response.json()) as AuthErrorBody;
    return new AuthRequestError(body);
  } catch {
    return new AuthRequestError({ error: 'Request failed' });
  }
}

function authHeaders(accessToken: string): HeadersInit {
  return {
    'Content-Type': 'application/json',
    Authorization: `Bearer ${accessToken}`,
  };
}

export async function sessionRequest(accessToken: string): Promise<SessionInfo> {
  const response = await fetch(`${API_BASE}/auth/session`, {
    headers: authHeaders(accessToken),
  });
  if (!response.ok) {
    throw await parseAuthError(response);
  }
  return (await response.json()) as SessionInfo;
}

export async function loginRequest(identifier: string, password: string): Promise<LoginApiResponse> {
  const response = await fetch(`${API_BASE}/auth/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ identifier: identifier.trim(), password }),
  });
  if (!response.ok) {
    const err = await parseAuthError(response);
    if (err.message === 'ADMIN_PORTAL_REQUIRED' || err.code === 'ADMIN_PORTAL_REQUIRED') {
      throw new AuthRequestError({ error: 'ADMIN_PORTAL_REQUIRED', code: 'ADMIN_PORTAL_REQUIRED' });
    }
    throw err;
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
    throw await parseAuthError(response);
  }
  return (await response.json()) as AuthTokensResponse;
}

export async function registerRequest(payload: RegisterPayload): Promise<RegisterPendingResponse> {
  const response = await fetch(`${API_BASE}/auth/register`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify(payload),
  });
  if (!response.ok) {
    throw await parseAuthError(response);
  }
  return (await response.json()) as RegisterPendingResponse;
}

export async function resendRegistrationOtpRequest(email: string, role: ApiRole): Promise<void> {
  const response = await fetch(`${API_BASE}/auth/register/resend-otp`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, role }),
  });
  if (!response.ok) {
    throw await parseAuthError(response);
  }
}

export async function verifyRegistrationOtpRequest(
  email: string,
  role: ApiRole,
  otpCode: string,
): Promise<AuthTokensResponse> {
  const response = await fetch(`${API_BASE}/auth/register/verify-otp`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email, role, otpCode }),
  });
  if (!response.ok) {
    throw await parseAuthError(response);
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
    throw await parseAuthError(response);
  }
}

export async function adminLoginRequest(identifier: string, password: string): Promise<AuthTokensResponse> {
  const response = await fetch(`${API_BASE}/auth/admin/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ identifier: identifier.trim(), password }),
  });
  if (!response.ok) {
    throw await parseAuthError(response);
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
    throw await parseAuthError(response);
  }
  const body = (await response.json()) as { key: string; value: boolean };
  return body.value;
}

export function isAdminApiRole(role: ApiRole): boolean {
  return role === 'ADMIN' || role === 'FINANCE_ADMIN' || role === 'SUPER_ADMIN';
}
