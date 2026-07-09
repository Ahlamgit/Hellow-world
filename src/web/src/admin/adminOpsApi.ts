import { api, type ApiResponse, type Booking, type PagedResult } from '../services/api';
import { adminApi } from './adminApi';
import type { AdminListQuery, AdminListResult } from './moduleConfig';

export interface BookingListQuery {
  status?: string;
  fromDate?: string;
  toDate?: string;
  page?: number;
  pageSize?: number;
}

export interface AdminBookingStats {
  totalBookings: number;
  pendingPayment: number;
  awaitingConfirmation: number;
  confirmed: number;
  completed: number;
  cancelled: number;
  rejected: number;
}

export interface AdminComplaintDetail {
  id: string;
  subject: string;
  description: string;
  status: string;
  priority: string;
  complainantEmail: string;
  resolution?: string;
  createdAt: string;
  resolvedAt?: string;
}

export interface AdminSupportTicketDetail {
  id: string;
  ticketNumber: string;
  subject: string;
  description: string;
  status: string;
  priority: string;
  category: string;
  userEmail: string;
  createdAt: string;
  closedAt?: string;
}

export interface AdminPaymentDetail {
  id: string;
  bookingId: string;
  bookingReference?: string;
  payerEmail: string;
  payeeEmail: string;
  amount: number;
  currency: string;
  status: string;
  paymentMethod: string;
  transactionReference?: string;
  paidAt?: string;
  failureReason?: string;
  createdAt: string;
}

export const adminBookingsApi = {
  list: (query: BookingListQuery) =>
    api.get<ApiResponse<PagedResult<Booking>>>('/admin/bookings', { params: query }),
  getById: (id: string) => api.get<ApiResponse<Booking>>(`/admin/bookings/${id}`),
  stats: () => api.get<ApiResponse<AdminBookingStats>>('/admin/bookings/stats'),
};

export const adminComplaintsApi = {
  list: (query: AdminListQuery) => adminApi.listModule('complaints', query) as Promise<{ data: ApiResponse<AdminListResult> }>,
  getById: (id: string) => api.get<ApiResponse<AdminComplaintDetail>>(`/admin/complaints/${id}`),
  resolve: (id: string, resolution: string) =>
    api.post<ApiResponse<AdminComplaintDetail>>(`/admin/complaints/${id}/resolve`, { resolution }),
};

export const adminSupportTicketsApi = {
  list: (query: AdminListQuery) => adminApi.listModule('support-tickets', query) as Promise<{ data: ApiResponse<AdminListResult> }>,
  getById: (id: string) => api.get<ApiResponse<AdminSupportTicketDetail>>(`/admin/support-tickets/${id}`),
  close: (id: string) => api.post<ApiResponse<AdminSupportTicketDetail>>(`/admin/support-tickets/${id}/close`, {}),
};

export const adminPaymentsApi = {
  list: (query: AdminListQuery) => adminApi.listModule('payments', query) as Promise<{ data: ApiResponse<AdminListResult> }>,
  getById: (id: string) => api.get<ApiResponse<AdminPaymentDetail>>(`/admin/payments/${id}`),
};

export const BOOKING_STATUSES = [
  'Draft', 'AwaitingPayment', 'PendingCraftsmanConfirmation', 'Confirmed',
  'InProgress', 'Completed', 'Cancelled', 'Rejected', 'Expired',
] as const;
