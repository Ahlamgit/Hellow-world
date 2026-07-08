import { api, type ApiResponse, type PagedResult } from '../services/api';

export interface PlanBillingOption {
  id?: string;
  cycle: string;
  price: number;
  durationDays: number;
  isActive: boolean;
}

export interface SubscriptionPlan {
  id: string;
  planCode: string;
  nameEn: string;
  nameAr: string;
  descriptionEn?: string;
  descriptionAr?: string;
  currency: string;
  targetRole: string;
  status: string;
  isFeatured: boolean;
  maxServices?: number;
  verificationBadge: boolean;
  premiumBadge: boolean;
  trialDays: number;
  billingOptions: PlanBillingOption[];
}

export interface UserSubscription {
  id: string;
  userId: string;
  userEmail: string;
  userName: string;
  planId: string;
  planCode: string;
  planNameEn: string;
  planNameAr: string;
  billingOptionId?: string;
  billingCycle?: string;
  status: string;
  startDate: string;
  endDate?: string;
  autoRenew: boolean;
  amountPaid?: number;
  currency?: string;
  couponCode?: string;
  cancelledAt?: string;
  cancellationReason?: string;
  createdAt: string;
}

export interface SubscribeRequest {
  planId: string;
  billingOptionId: string;
  autoRenew?: boolean;
  couponCode?: string;
}

export const subscriptionPlansApi = {
  list: (targetRole?: string) =>
    api.get<ApiResponse<SubscriptionPlan[]>>('/subscription-plans', { params: { targetRole } }),
  getById: (id: string) =>
    api.get<ApiResponse<SubscriptionPlan>>(`/subscription-plans/${id}`),
};

export const userSubscriptionApi = {
  getCurrent: () =>
    api.get<ApiResponse<UserSubscription | null>>('/me/subscription'),
  getHistory: (page = 1, pageSize = 20) =>
    api.get<ApiResponse<PagedResult<UserSubscription>>>('/me/subscriptions', { params: { page, pageSize } }),
  subscribe: (data: SubscribeRequest) =>
    api.post<ApiResponse<UserSubscription>>('/me/subscription', data),
  cancel: (id: string, reason?: string) =>
    api.post<ApiResponse<{ subscriptionId: string; status: string; message: string }>>(
      `/me/subscription/${id}/cancel`,
      { reason },
    ),
  updateAutoRenew: (autoRenew: boolean) =>
    api.patch<ApiResponse<UserSubscription>>('/me/subscription/auto-renew', { autoRenew }),
};
