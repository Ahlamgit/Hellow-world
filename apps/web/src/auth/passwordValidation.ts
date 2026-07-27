import type { ApiRole } from './types';

export type RegisterableRole = 'CUSTOMER' | 'CRAFTSMAN' | 'STORE';

export type RegisterAccountType = 'customer' | 'provider' | 'store';

export function registerTypeToApiRole(type: RegisterAccountType): RegisterableRole {
  switch (type) {
    case 'customer':
      return 'CUSTOMER';
    case 'provider':
      return 'CRAFTSMAN';
    case 'store':
      return 'STORE';
  }
}

export function apiRoleToRegisterType(role: ApiRole): RegisterAccountType | null {
  switch (role) {
    case 'CUSTOMER':
      return 'customer';
    case 'CRAFTSMAN':
      return 'provider';
    case 'STORE':
      return 'store';
    default:
      return null;
  }
}

export type PasswordValidation = {
  valid: boolean;
  errors: string[];
};

export function validatePassword(password: string): PasswordValidation {
  const errors: string[] = [];
  if (!password) {
    errors.push('required');
  } else {
    if (password.length < 8) {
      errors.push('minLength');
    }
    if (!/[a-zA-Z]/.test(password)) {
      errors.push('letter');
    }
    if (!/\d/.test(password)) {
      errors.push('digit');
    }
  }
  return { valid: errors.length === 0, errors };
}

export function validateConfirmPassword(password: string, confirm: string): boolean {
  return password.length > 0 && password === confirm;
}
