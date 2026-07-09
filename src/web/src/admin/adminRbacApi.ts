import { api, type ApiResponse } from '../services/api';

export interface AdminRole {
  id: string;
  name: string;
  nameAr: string;
  description?: string;
  isSystemRole: boolean;
  permissionCount: number;
}

export interface AdminPermission {
  id: string;
  code: string;
  nameEn: string;
  module: string;
}

export interface RolePermissionMatrix {
  roleId: string;
  roleName: string;
  allPermissions: AdminPermission[];
  assignedPermissionIds: string[];
}

export const adminRbacApi = {
  listRoles: () => api.get<ApiResponse<AdminRole[]>>('/admin/rbac/roles'),
  getRolePermissions: (roleId: string) =>
    api.get<ApiResponse<RolePermissionMatrix>>(`/admin/rbac/roles/${roleId}/permissions`),
  updateRolePermissions: (roleId: string, permissionIds: string[]) =>
    api.put<ApiResponse<RolePermissionMatrix>>(`/admin/rbac/roles/${roleId}/permissions`, { permissionIds }),
};
