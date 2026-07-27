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
