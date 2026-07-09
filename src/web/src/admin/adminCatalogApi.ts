import { api, type ApiResponse, type PagedResult } from '../services/api';

export interface CategoryListQuery {
  search?: string;
  isActive?: boolean;
  page?: number;
  pageSize?: number;
}

export interface ServiceListQuery {
  search?: string;
  categoryId?: string;
  isActive?: boolean;
  page?: number;
  pageSize?: number;
}

export interface Category {
  id: string;
  nameEn: string;
  nameAr: string;
  descriptionEn?: string;
  descriptionAr?: string;
  iconUrl?: string;
  displayOrder: number;
  isActive: boolean;
  parentCategoryId?: string;
  parentCategoryName?: string;
  serviceCount: number;
}

export interface CreateCategoryRequest {
  nameEn: string;
  nameAr: string;
  descriptionEn?: string;
  descriptionAr?: string;
  iconUrl?: string;
  displayOrder: number;
  isActive: boolean;
  parentCategoryId?: string;
}

export type UpdateCategoryRequest = CreateCategoryRequest;

export interface Service {
  id: string;
  categoryId: string;
  categoryName: string;
  nameEn: string;
  nameAr: string;
  descriptionEn?: string;
  descriptionAr?: string;
  basePrice: number;
  imageUrl?: string;
  isActive: boolean;
  estimatedDurationMinutes: number;
}

export interface CreateServiceRequest {
  categoryId: string;
  nameEn: string;
  nameAr: string;
  descriptionEn?: string;
  descriptionAr?: string;
  basePrice: number;
  imageUrl?: string;
  isActive: boolean;
  estimatedDurationMinutes: number;
}

export type UpdateServiceRequest = CreateServiceRequest;

export const emptyCategoryForm = (): CreateCategoryRequest => ({
  nameEn: '',
  nameAr: '',
  displayOrder: 0,
  isActive: true,
});

export const emptyServiceForm = (): CreateServiceRequest => ({
  categoryId: '',
  nameEn: '',
  nameAr: '',
  basePrice: 0,
  isActive: true,
  estimatedDurationMinutes: 60,
});

export const adminCategoriesApi = {
  list: (query: CategoryListQuery) =>
    api.get<ApiResponse<PagedResult<Category>>>('/admin/categories', { params: query }),
  getById: (id: string) => api.get<ApiResponse<Category>>(`/admin/categories/${id}`),
  create: (data: CreateCategoryRequest) => api.post<ApiResponse<Category>>('/admin/categories', data),
  update: (id: string, data: UpdateCategoryRequest) => api.put<ApiResponse<Category>>(`/admin/categories/${id}`, data),
  delete: (id: string) => api.delete<ApiResponse<object>>(`/admin/categories/${id}`),
};

export const adminServicesApi = {
  list: (query: ServiceListQuery) =>
    api.get<ApiResponse<PagedResult<Service>>>('/admin/services', { params: query }),
  getById: (id: string) => api.get<ApiResponse<Service>>(`/admin/services/${id}`),
  create: (data: CreateServiceRequest) => api.post<ApiResponse<Service>>('/admin/services', data),
  update: (id: string, data: UpdateServiceRequest) => api.put<ApiResponse<Service>>(`/admin/services/${id}`, data),
  delete: (id: string) => api.delete<ApiResponse<object>>(`/admin/services/${id}`),
};
