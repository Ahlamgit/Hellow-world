import { api, type ApiResponse } from '../services/api';
import type { AdminAnalytics, AdminDashboard, AdminListQuery, AdminListResult, AdminSystemHealth } from './moduleConfig';

export const adminApi = {
  getDashboard: () => api.get<ApiResponse<AdminDashboard>>('/admin/dashboard'),
  listModule: (module: string, query: AdminListQuery) =>
    api.get<ApiResponse<AdminListResult>>(`/admin/${module}`, { params: query }),
  bulkAction: (module: string, action: string, ids: string[], reason?: string) =>
    api.post<ApiResponse<{ affectedCount: number; message: string }>>(`/admin/${module}/bulk`, { action, ids, reason }),
  exportModule: async (module: string, format: 'xlsx' | 'pdf', query: AdminListQuery) => {
    const response = await api.get(`/admin/${module}/export`, {
      params: { ...query, format },
      responseType: 'blob',
    });
    const url = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement('a');
    link.href = url;
    link.download = `khadamati-${module}.${format === 'pdf' ? 'pdf' : 'xlsx'}`;
    link.click();
    window.URL.revokeObjectURL(url);
  },
  getAnalytics: () => api.get<ApiResponse<AdminAnalytics>>('/admin/analytics/data'),
  getReports: () => api.get<ApiResponse<Array<{ id: string; name: string; type: string; status: string; generatedAt: string }>>>('/admin/reports/list'),
  generateReport: async (reportId: string, format: 'xlsx' | 'pdf', query: AdminListQuery = {}) => {
    const response = await api.post('/admin/reports/generate', { reportId, format, query }, { responseType: 'blob' });
    const url = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement('a');
    link.href = url;
    link.download = `khadamati-report-${reportId}.${format === 'pdf' ? 'pdf' : 'xlsx'}`;
    link.click();
    window.URL.revokeObjectURL(url);
  },
  getSystemHealth: () => api.get<ApiResponse<AdminSystemHealth>>('/admin/system/health'),
  createBackup: () => api.post<ApiResponse<{ id: string; name: string; status: string; filePath?: string; errorMessage?: string }>>('/admin/backup/create'),
  restoreBackup: (backupId: string) =>
    api.post<ApiResponse<{ message: string }>>('/admin/backup/restore', { backupId, confirm: true }),
  downloadBackup: async (backupId: string) => {
    const response = await api.get(`/admin/backup/${backupId}/download`, { responseType: 'blob' });
    const url = window.URL.createObjectURL(new Blob([response.data]));
    const link = document.createElement('a');
    link.href = url;
    link.download = `khadamati-backup-${backupId}.json.gz`;
    link.click();
    window.URL.revokeObjectURL(url);
  },
};
