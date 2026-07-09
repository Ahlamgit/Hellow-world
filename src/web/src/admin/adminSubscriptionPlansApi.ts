import { api, type ApiResponse, type PagedResult } from '../services/api';
import type { PlanBillingOption } from '../services/subscriptionsApi';

export const PLAN_STATUSES = ['Active', 'Inactive', 'Suspended', 'Archived'] as const;
export const TARGET_ROLES = ['Customer', 'Craftsman', 'Store', 'Administrator'] as const;
export const BILLING_CYCLES = ['Monthly', 'Quarterly', 'SemiAnnual', 'Annual', 'Lifetime'] as const;

export interface AdminSubscriptionPlan {
  id: string;
  planCode: string;
  nameEn: string;
  nameAr: string;
  descriptionEn?: string;
  descriptionAr?: string;
  currency: string;
  targetRole: string;
  status: string;
  displayPriority: number;
  searchPriority: number;
  isFeatured: boolean;
  homePageVisible: boolean;
  bannerVisible: boolean;
  categoryVisible: boolean;
  maxCategories?: number;
  maxServices?: number;
  maxPhotos?: number;
  maxVideos?: number;
  maxAdvertisements?: number;
  advertisementCredits: number;
  featuredDays: number;
  verificationBadge: boolean;
  premiumBadge: boolean;
  statisticsDashboard: boolean;
  analytics: boolean;
  priorityCustomerSupport: boolean;
  renewalReminder: boolean;
  autoRenewal: boolean;
  expiryNotification: boolean;
  gracePeriodDays: number;
  trialDays: number;
  discountPercentage?: number;
  couponSupport: boolean;
  taxRate?: number;
  vatRate?: number;
  paymentRequired: boolean;
  paymentMethods: string[];
  planColor?: string;
  planIcon?: string;
  clonedFromPlanId?: string;
  suspendedAt?: string;
  archivedAt?: string;
  createdAt: string;
  updatedAt?: string;
  billingOptions: PlanBillingOption[];
}

export interface SubscriptionPlanListQuery {
  search?: string;
  status?: string;
  targetRole?: string;
  featured?: boolean;
  includeArchived?: boolean;
  page?: number;
  pageSize?: number;
}

export type CreateSubscriptionPlanRequest = Omit<AdminSubscriptionPlan, 'id' | 'createdAt' | 'updatedAt' | 'clonedFromPlanId' | 'suspendedAt' | 'archivedAt'>;

export interface CloneSubscriptionPlanRequest {
  newPlanCode?: string;
  nameEnSuffix?: string;
  nameArSuffix?: string;
}

export interface PlanActionResponse {
  planId: string;
  status: string;
  message: string;
}

export const defaultBillingOption = (cycle: string = 'Monthly'): PlanBillingOption => ({
  cycle,
  price: 0,
  durationDays: cycle === 'Lifetime' ? 0 : cycle === 'Annual' ? 365 : cycle === 'SemiAnnual' ? 180 : cycle === 'Quarterly' ? 90 : 30,
  isActive: true,
});

export const emptyPlanForm = (): CreateSubscriptionPlanRequest => ({
  planCode: '',
  nameEn: '',
  nameAr: '',
  descriptionEn: '',
  descriptionAr: '',
  currency: 'SAR',
  targetRole: 'Craftsman',
  status: 'Inactive',
  displayPriority: 0,
  searchPriority: 0,
  isFeatured: false,
  homePageVisible: true,
  bannerVisible: false,
  categoryVisible: true,
  maxServices: 10,
  maxCategories: 5,
  advertisementCredits: 0,
  featuredDays: 0,
  verificationBadge: false,
  premiumBadge: false,
  statisticsDashboard: false,
  analytics: false,
  priorityCustomerSupport: false,
  renewalReminder: true,
  autoRenewal: false,
  expiryNotification: true,
  gracePeriodDays: 0,
  trialDays: 0,
  couponSupport: false,
  paymentRequired: true,
  paymentMethods: ['Card'],
  billingOptions: [defaultBillingOption('Monthly')],
});

export const adminSubscriptionPlansApi = {
  list: (query: SubscriptionPlanListQuery = {}) =>
    api.get<ApiResponse<PagedResult<AdminSubscriptionPlan>>>('/admin/subscription-plans', { params: query }),
  getById: (id: string) =>
    api.get<ApiResponse<AdminSubscriptionPlan>>(`/admin/subscription-plans/${id}`),
  create: (data: CreateSubscriptionPlanRequest) =>
    api.post<ApiResponse<AdminSubscriptionPlan>>('/admin/subscription-plans', data),
  update: (id: string, data: CreateSubscriptionPlanRequest) =>
    api.put<ApiResponse<AdminSubscriptionPlan>>(`/admin/subscription-plans/${id}`, data),
  delete: (id: string) =>
    api.delete<ApiResponse<object>>(`/admin/subscription-plans/${id}`),
  clone: (id: string, data: CloneSubscriptionPlanRequest) =>
    api.post<ApiResponse<AdminSubscriptionPlan>>(`/admin/subscription-plans/${id}/clone`, data),
  activate: (id: string) =>
    api.post<ApiResponse<PlanActionResponse>>(`/admin/subscription-plans/${id}/activate`),
  deactivate: (id: string) =>
    api.post<ApiResponse<PlanActionResponse>>(`/admin/subscription-plans/${id}/deactivate`),
  suspend: (id: string) =>
    api.post<ApiResponse<PlanActionResponse>>(`/admin/subscription-plans/${id}/suspend`),
  archive: (id: string) =>
    api.post<ApiResponse<PlanActionResponse>>(`/admin/subscription-plans/${id}/archive`),
};
