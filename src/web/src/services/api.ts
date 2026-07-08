import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api/v1';

export const api = axios.create({
  baseURL: API_BASE_URL,
  headers: { 'Content-Type': 'application/json' },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  return config;
});

api.interceptors.response.use(
  (response) => response,
  async (error) => {
    const original = error.config;
    if (error.response?.status === 401 && !original._retry) {
      original._retry = true;
      const refreshToken = localStorage.getItem('refreshToken');
      const accessToken = localStorage.getItem('accessToken');
      if (refreshToken && accessToken) {
        try {
          const { data } = await axios.post(`${API_BASE_URL}/auth/refresh`, {
            accessToken,
            refreshToken,
          });
          localStorage.setItem('accessToken', data.data.accessToken);
          localStorage.setItem('refreshToken', data.data.refreshToken);
          original.headers.Authorization = `Bearer ${data.data.accessToken}`;
          return api(original);
        } catch {
          localStorage.clear();
          window.location.href = '/login';
        }
      }
    }
    return Promise.reject(error);
  }
);

export interface ApiResponse<T> {
  success: boolean;
  message?: string;
  data: T;
  errors?: string[];
}

export interface AuthResponse {
  accessToken: string;
  refreshToken: string;
  expiresAt: string;
  user: UserDto;
}

export interface UserDto {
  id: string;
  email: string;
  phone: string;
  role: string;
  status: string;
  verificationStatus: string;
  subscriptionStatus: string;
  firstName: string;
  lastName: string;
  profilePictureUrl?: string;
  preferredLanguage: string;
}

export interface ServiceCategory {
  id: string;
  nameAr: string;
  nameEn: string;
  descriptionAr?: string;
  descriptionEn?: string;
  iconUrl?: string;
  displayOrder: number;
}

export interface Service {
  id: string;
  categoryId: string;
  nameAr: string;
  nameEn: string;
  descriptionAr?: string;
  descriptionEn?: string;
  basePrice: number;
  imageUrl?: string;
  estimatedDurationMinutes: number;
}

export const authApi = {
  login: (email: string, password: string) =>
    api.post<ApiResponse<AuthResponse>>('/auth/login', { email, password }),
  register: (data: Record<string, string>) =>
    api.post<ApiResponse<AuthResponse>>('/auth/register', data),
  logout: () => api.post('/auth/revoke', localStorage.getItem('refreshToken')),
};

export const servicesApi = {
  getCategories: () => api.get<ApiResponse<ServiceCategory[]>>('/services/categories'),
  getServices: (categoryId?: string) =>
    api.get<ApiResponse<Service[]>>('/services', { params: { categoryId } }),
};

export const usersApi = {
  getProfile: () => api.get<ApiResponse<UserDto>>('/users/me'),
};
