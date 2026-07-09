import { api, type ApiResponse, type PagedResult } from '../services/api';

export interface CouponListQuery {
  search?: string;
  isActive?: boolean;
  page?: number;
  pageSize?: number;
}

export interface AdvertisementListQuery {
  search?: string;
  placement?: string;
  isActive?: boolean;
  page?: number;
  pageSize?: number;
}

export interface Coupon {
  id: string;
  code: string;
  descriptionEn: string;
  descriptionAr: string;
  discountPercentage: number;
  maxDiscountAmount?: number;
  maxUses: number;
  usedCount: number;
  validFrom: string;
  validTo: string;
  isActive: boolean;
}

export interface CreateCouponRequest {
  code: string;
  descriptionEn: string;
  descriptionAr: string;
  discountPercentage: number;
  maxDiscountAmount?: number;
  maxUses: number;
  validFrom: string;
  validTo: string;
  isActive: boolean;
}

export type UpdateCouponRequest = CreateCouponRequest;

export interface Advertisement {
  id: string;
  titleEn: string;
  titleAr: string;
  descriptionEn?: string;
  imageUrl?: string;
  placement: string;
  targetUserId?: string;
  startDate: string;
  endDate?: string;
  impressions: number;
  clicks: number;
  isActive: boolean;
}

export interface CreateAdvertisementRequest {
  titleEn: string;
  titleAr: string;
  descriptionEn?: string;
  imageUrl?: string;
  placement: string;
  targetUserId?: string;
  startDate: string;
  endDate?: string;
  isActive: boolean;
}

export type UpdateAdvertisementRequest = CreateAdvertisementRequest;

export const AD_PLACEMENTS = ['HomePage', 'Banner', 'Category', 'Sidebar'] as const;

export const emptyCouponForm = (): CreateCouponRequest => ({
  code: '',
  descriptionEn: '',
  descriptionAr: '',
  discountPercentage: 10,
  maxUses: 0,
  validFrom: new Date().toISOString().slice(0, 10),
  validTo: new Date(Date.now() + 90 * 86400000).toISOString().slice(0, 10),
  isActive: true,
});

export const emptyAdvertisementForm = (): CreateAdvertisementRequest => ({
  titleEn: '',
  titleAr: '',
  placement: 'HomePage',
  startDate: new Date().toISOString().slice(0, 10),
  isActive: true,
});

export const adminCouponsApi = {
  list: (query: CouponListQuery) =>
    api.get<ApiResponse<PagedResult<Coupon>>>('/admin/coupons', { params: query }),
  getById: (id: string) => api.get<ApiResponse<Coupon>>(`/admin/coupons/${id}`),
  create: (data: CreateCouponRequest) => api.post<ApiResponse<Coupon>>('/admin/coupons', data),
  update: (id: string, data: UpdateCouponRequest) => api.put<ApiResponse<Coupon>>(`/admin/coupons/${id}`, data),
  delete: (id: string) => api.delete<ApiResponse<object>>(`/admin/coupons/${id}`),
};

export const adminAdvertisementsApi = {
  list: (query: AdvertisementListQuery) =>
    api.get<ApiResponse<PagedResult<Advertisement>>>('/admin/advertisements', { params: query }),
  getById: (id: string) => api.get<ApiResponse<Advertisement>>(`/admin/advertisements/${id}`),
  create: (data: CreateAdvertisementRequest) => api.post<ApiResponse<Advertisement>>('/admin/advertisements', data),
  update: (id: string, data: UpdateAdvertisementRequest) => api.put<ApiResponse<Advertisement>>(`/admin/advertisements/${id}`, data),
  delete: (id: string) => api.delete<ApiResponse<object>>(`/admin/advertisements/${id}`),
};
