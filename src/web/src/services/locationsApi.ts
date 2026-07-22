import { api, type ApiResponse, type PagedResult } from './api';

export interface Region {
  id: string;
  nameEn: string;
  nameAr: string;
  code: string;
  isActive: boolean;
  cityCount: number;
}

export interface City {
  id: string;
  regionId: string;
  regionName: string;
  nameEn: string;
  nameAr: string;
  code: string;
  isActive: boolean;
}

export const locationsApi = {
  listRegions: () =>
    api.get<ApiResponse<PagedResult<Region>>>('/locations/regions'),
  listCities: (regionId?: string) =>
    api.get<ApiResponse<PagedResult<City>>>('/locations/cities', { params: regionId ? { regionId } : undefined }),
};
