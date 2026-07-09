import { api, type ApiResponse, type PagedResult } from '../services/api';

export interface VerificationDocument {
  id: string;
  userId: string;
  userEmail: string;
  userRole: string;
  documentType: string;
  documentUrl: string;
  status: string;
  rejectionReason?: string;
  createdAt: string;
  reviewedAt?: string;
}

export interface VerificationDocumentListQuery {
  status?: string;
  page?: number;
  pageSize?: number;
}

export const adminVerificationApi = {
  list: (query: VerificationDocumentListQuery) =>
    api.get<ApiResponse<PagedResult<VerificationDocument>>>('/admin/verification-documents', { params: query }),
  getById: (id: string) =>
    api.get<ApiResponse<VerificationDocument>>(`/admin/verification-documents/${id}`),
  approve: (id: string) =>
    api.post<ApiResponse<VerificationDocument>>(`/admin/verification-documents/${id}/approve`, {}),
  reject: (id: string, reason: string) =>
    api.post<ApiResponse<VerificationDocument>>(`/admin/verification-documents/${id}/reject`, { reason }),
};

export const VERIFICATION_STATUSES = ['PendingReview', 'Verified', 'Rejected', 'Unverified'] as const;
