export type ApiRole = 'CUSTOMER' | 'CRAFTSMAN' | 'STORE' | 'ADMIN' | 'FINANCE_ADMIN' | 'SUPER_ADMIN';

export type PortalRole = 'customer' | 'provider' | 'store' | 'admin';

export type AuthSession = {
  accessToken: string;
  refreshToken: string;
  role: ApiRole;
  email: string;
};

export type AuthTokensResponse = {
  accessToken: string;
  refreshToken: string;
  tokenType: string;
  role: ApiRole;
};

export type LoginApiResponse = {
  status: 'SUCCESS' | 'ROLE_SELECTION_REQUIRED';
  tokens: AuthTokensResponse | null;
  availableRoles: ApiRole[];
};

export type RegisterPayload = {
  firstName: string;
  lastName: string;
  phoneE164: string;
  email: string;
  password: string;
  confirmPassword: string;
  role: 'CUSTOMER' | 'CRAFTSMAN' | 'STORE';
  acceptTerms: boolean;
  acceptPrivacy: boolean;
  termsVersion: string;
  privacyVersion: string;
  language: string;
};

export type RegisterPendingResponse = {
  status: string;
  email: string;
  role: ApiRole;
  phoneE164: string;
  message: string;
};
