export type AdminAuthStatus = 'INITIALIZING' | 'AUTHENTICATED' | 'UNAUTHENTICATED';

export type AdminSession = {
  email: string;
  accessToken: string;
  refreshToken: string;
  role: 'ADMIN';
  accountStatus: 'ACTIVE';
  firstName: string;
  lastName: string;
  phoneE164: string;
  mfaVerified: boolean;
};

export type AdminLoginPendingResponse = {
  status: string;
  email: string;
  phoneE164: string;
  message: string;
};

export type AdminSessionInfo = {
  email: string;
  role: 'ADMIN';
  accountStatus: 'ACTIVE' | 'PENDING_VERIFICATION' | 'DISABLED';
  firstName: string;
  lastName: string;
  phoneE164: string;
  mfaVerified: boolean;
};

export type AdminAuthTokensResponse = {
  accessToken: string;
  refreshToken: string;
  tokenType: string;
  role: 'ADMIN';
};

export type AdminAuthErrorBody = {
  error?: string;
  code?: string;
};
