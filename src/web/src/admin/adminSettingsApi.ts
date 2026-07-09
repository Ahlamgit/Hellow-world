import { api, type ApiResponse } from '../services/api';

export interface SystemSetting {
  id: string;
  key: string;
  value: string;
  category: string;
  description?: string;
  isEncrypted: boolean;
}

export const adminSettingsApi = {
  list: () => api.get<ApiResponse<SystemSetting[]>>('/admin/settings'),
  update: (id: string, value: string) =>
    api.put<ApiResponse<SystemSetting>>(`/admin/settings/${id}`, { value }),
};
