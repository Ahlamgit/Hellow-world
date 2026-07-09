import { api, type ApiResponse, type PagedResult } from '../services/api';

export interface RegionListQuery {
  search?: string;
  isActive?: boolean;
  page?: number;
  pageSize?: number;
}

export interface CityListQuery {
  search?: string;
  regionId?: string;
  isActive?: boolean;
  page?: number;
  pageSize?: number;
}

export interface Region {
  id: string;
  nameEn: string;
  nameAr: string;
  code: string;
  isActive: boolean;
  cityCount: number;
}

export interface CreateRegionRequest {
  nameEn: string;
  nameAr: string;
  code: string;
  isActive: boolean;
}

export type UpdateRegionRequest = CreateRegionRequest;

export interface City {
  id: string;
  regionId: string;
  regionName: string;
  nameEn: string;
  nameAr: string;
  code: string;
  isActive: boolean;
}

export interface CreateCityRequest {
  regionId: string;
  nameEn: string;
  nameAr: string;
  code: string;
  isActive: boolean;
}

export type UpdateCityRequest = CreateCityRequest;

export const emptyRegionForm = (): CreateRegionRequest => ({
  nameEn: '',
  nameAr: '',
  code: '',
  isActive: true,
});

export const emptyCityForm = (): CreateCityRequest => ({
  regionId: '',
  nameEn: '',
  nameAr: '',
  code: '',
  isActive: true,
});

export const adminRegionsApi = {
  list: (query: RegionListQuery) =>
    api.get<ApiResponse<PagedResult<Region>>>('/admin/regions', { params: query }),
  getById: (id: string) => api.get<ApiResponse<Region>>(`/admin/regions/${id}`),
  create: (data: CreateRegionRequest) => api.post<ApiResponse<Region>>('/admin/regions', data),
  update: (id: string, data: UpdateRegionRequest) => api.put<ApiResponse<Region>>(`/admin/regions/${id}`, data),
  delete: (id: string) => api.delete<ApiResponse<object>>(`/admin/regions/${id}`),
};

export const adminCitiesApi = {
  list: (query: CityListQuery) =>
    api.get<ApiResponse<PagedResult<City>>>('/admin/cities', { params: query }),
  getById: (id: string) => api.get<ApiResponse<City>>(`/admin/cities/${id}`),
  create: (data: CreateCityRequest) => api.post<ApiResponse<City>>('/admin/cities', data),
  update: (id: string, data: UpdateCityRequest) => api.put<ApiResponse<City>>(`/admin/cities/${id}`, data),
  delete: (id: string) => api.delete<ApiResponse<object>>(`/admin/cities/${id}`),
};
