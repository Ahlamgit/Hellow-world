import { api, type ApiResponse, type PagedResult } from '../services/api';
import type { UserSubscription } from '../services/subscriptionsApi';

export interface UserSubscriptionListQuery {
  search?: string;
  status?: string;
  userId?: string;
  planId?: string;
  page?: number;
  pageSize?: number;
}

export interface GrantSubscriptionRequest {
  planId: string;
  billingOptionId: string;
  autoRenew?: boolean;
  notes?: string;
}

export const adminUserSubscriptionsApi = {
  list: (query: UserSubscriptionListQuery = {}) =>
    api.get<ApiResponse<PagedResult<UserSubscription>>>('/admin/user-subscriptions', { params: query }),
  getById: (id: string) =>
    api.get<ApiResponse<UserSubscription>>(`/admin/user-subscriptions/${id}`),
  grant: (userId: string, data: GrantSubscriptionRequest) =>
    api.post<ApiResponse<UserSubscription>>(`/admin/user-subscriptions/users/${userId}`, data),
  cancel: (id: string, reason?: string) =>
    api.post<ApiResponse<{ subscriptionId: string; status: string; message: string }>>(
      `/admin/user-subscriptions/${id}/cancel`,
      { reason },
    ),
};
