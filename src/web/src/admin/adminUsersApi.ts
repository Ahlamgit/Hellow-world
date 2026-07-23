import { api, type ApiResponse, type PagedResult } from '../services/api';

export const PLATFORM_ROLES = [
  'SuperAdmin', 'Admin', 'SupportAgent', 'Moderator', 'Customer',
  'Craftsman', 'StoreOwner', 'StoreEmployee', 'Accountant',
] as const;

export const USER_STATUSES = ['Pending', 'Active', 'Suspended', 'Banned'] as const;

export interface AdminUserListQuery {
  search?: string;
  status?: string;
  role?: string;
  fromDate?: string;
  toDate?: string;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
  page?: number;
  pageSize?: number;
}

export interface AdminUserListItem {
  id: string;
  email: string;
  phone: string;
  firstName: string;
  lastName: string;
  primaryRole: string;
  roles: string[];
  status: string;
  verificationStatus: string;
  emailVerified: boolean;
  phoneVerified: boolean;
  createdAt: string;
  lastLoginAt?: string;
}

export interface AdminUserDetail extends AdminUserListItem {
  subscriptionStatus: string;
  subscriptionExpiresAt?: string;
  emailVerifiedAt?: string;
  phoneVerifiedAt?: string;
  preferredLanguage: string;
  permissions: string[];
}

export interface CreateAdminUserRequest {
  email: string;
  phone: string;
  password: string;
  confirmPassword: string;
  firstName: string;
  lastName: string;
  role: string;
  preferredLanguage: string;
  status: string;
  sendVerificationEmail: boolean;
}

export interface UpdateAdminUserRequest {
  phone: string;
  firstName: string;
  lastName: string;
  preferredLanguage: string;
  status: string;
}

export interface AssignUserRolesRequest {
  roles: string[];
  primaryRole: string;
}

export interface UserPermissionEntry {
  permissionId: string;
  code: string;
  nameEn: string;
  module: string;
  fromRole: boolean;
  override: boolean | null;
  effective: boolean;
}

export interface UserPermissionMatrix {
  userId: string;
  permissions: UserPermissionEntry[];
}

export interface UserPermissionOverride {
  permissionId: string;
  isGranted: boolean;
}

export const adminUsersApi = {
  list: (query: AdminUserListQuery) =>
    api.get<ApiResponse<PagedResult<AdminUserListItem>>>('/admin/users', { params: query }),

  getById: (id: string) =>
    api.get<ApiResponse<AdminUserDetail>>(`/admin/users/${id}`),

  create: (data: CreateAdminUserRequest) =>
    api.post<ApiResponse<AdminUserDetail>>('/admin/users', data),

  update: (id: string, data: UpdateAdminUserRequest) =>
    api.put<ApiResponse<AdminUserDetail>>(`/admin/users/${id}`, data),

  delete: (id: string) =>
    api.delete<ApiResponse<object>>(`/admin/users/${id}`),

  suspend: (id: string, reason?: string) =>
    api.post<ApiResponse<{ userId: string; status: string; message: string }>>(`/admin/users/${id}/suspend`, { reason }),

  activate: (id: string) =>
    api.post<ApiResponse<{ userId: string; status: string; message: string }>>(`/admin/users/${id}/activate`),

  assignRoles: (id: string, data: AssignUserRolesRequest) =>
    api.put<ApiResponse<AdminUserDetail>>(`/admin/users/${id}/roles`, data),

  verifyEmail: (id: string) =>
    api.post<ApiResponse<{ message: string }>>(`/admin/users/${id}/verify-email`),

  getPermissions: (id: string) =>
    api.get<ApiResponse<UserPermissionMatrix>>(`/admin/users/${id}/permissions`),

  updatePermissions: (id: string, overrides: UserPermissionOverride[]) =>
    api.put<ApiResponse<UserPermissionMatrix>>(`/admin/users/${id}/permissions`, { overrides }),
};
