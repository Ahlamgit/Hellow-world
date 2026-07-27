import type {
  AdminAuthErrorBody,
  AdminAuthTokensResponse,
  AdminLoginPendingResponse,
  AdminSessionInfo,
} from './adminTypes';

const ADMIN_API_BASE = '/api/admin';

export class AdminAuthRequestError extends Error {
  readonly code?: string;
  readonly body: AdminAuthErrorBody;

  constructor(body: AdminAuthErrorBody) {
    super(body.error ?? 'Request failed');
    this.code = body.code;
    this.body = body;
  }
}

async function parseAdminAuthError(response: Response): Promise<AdminAuthRequestError> {
  try {
    const body = (await response.json()) as AdminAuthErrorBody;
    return new AdminAuthRequestError(body);
  } catch {
    return new AdminAuthRequestError({ error: 'Request failed' });
  }
}

function authHeaders(accessToken: string): HeadersInit {
  return {
    'Content-Type': 'application/json',
    Authorization: `Bearer ${accessToken}`,
  };
}

export async function adminLoginRequest(email: string, password: string): Promise<AdminLoginPendingResponse> {
  const response = await fetch(`${ADMIN_API_BASE}/login`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ identifier: email.trim(), password }),
  });
  if (!response.ok) {
    throw await parseAdminAuthError(response);
  }
  return (await response.json()) as AdminLoginPendingResponse;
}

export async function adminVerifyMfaRequest(email: string, otpCode: string): Promise<AdminAuthTokensResponse> {
  const response = await fetch(`${ADMIN_API_BASE}/verify-mfa`, {
    method: 'POST',
    headers: { 'Content-Type': 'application/json' },
    body: JSON.stringify({ email: email.trim().toLowerCase(), otpCode }),
  });
  if (!response.ok) {
    throw await parseAdminAuthError(response);
  }
  return (await response.json()) as AdminAuthTokensResponse;
}

export async function adminSessionRequest(accessToken: string): Promise<AdminSessionInfo> {
  const response = await fetch(`${ADMIN_API_BASE}/session`, {
    headers: authHeaders(accessToken),
  });
  if (!response.ok) {
    throw await parseAdminAuthError(response);
  }
  return (await response.json()) as AdminSessionInfo;
}
