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

export interface PagedResult<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
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

export interface CraftsmanOption {
  id: string;
  firstName: string;
  lastName: string;
  specialization?: string;
  rating: number;
  totalReviews: number;
  completedJobs: number;
  price: number;
  isAvailable: boolean;
}

export interface TimeSlot {
  start: string;
  end: string;
  isAvailable: boolean;
}

export interface BookingPayment {
  id: string;
  amount: number;
  currency: string;
  status: string;
  paymentMethod: string;
  transactionReference?: string;
  paidAt?: string;
}

export interface Booking {
  id: string;
  bookingReference: string;
  serviceId: string;
  serviceName: string;
  customerId: string;
  customerName: string;
  craftsmanId: string;
  craftsmanName: string;
  status: string;
  description?: string;
  scheduledAt: string;
  slotEnd: string;
  completedAt?: string;
  paymentDueAt?: string;
  expiresAt?: string;
  estimatedPrice: number;
  finalPrice?: number;
  notes?: string;
  rejectionReason?: string;
  cancellationReason?: string;
  payment?: BookingPayment;
  statusHistory: { oldStatus?: string; newStatus: string; notes?: string; createdAt: string }[];
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

export const bookingsApi = {
  getCraftsmen: (serviceId: string) =>
    api.get<ApiResponse<CraftsmanOption[]>>('/bookings/craftsmen', { params: { serviceId } }),
  getAvailability: (craftsmanId: string, serviceId: string, date: string) =>
    api.get<ApiResponse<TimeSlot[]>>('/bookings/availability', { params: { craftsmanId, serviceId, date } }),
  create: (data: { serviceId: string; craftsmanId: string; scheduledAt: string; addressId?: string; description?: string }) =>
    api.post<ApiResponse<Booking>>('/bookings', data),
  list: (params?: { status?: string; page?: number; pageSize?: number }) =>
    api.get<ApiResponse<PagedResult<Booking>>>('/bookings', { params }),
  getById: (id: string) => api.get<ApiResponse<Booking>>(`/bookings/${id}`),
  confirm: (id: string, notes?: string) =>
    api.post<ApiResponse<Booking>>(`/bookings/${id}/confirm`, { notes }),
  initiatePayment: (id: string, paymentMethod: string) =>
    api.post<ApiResponse<BookingPayment>>(`/bookings/${id}/payment`, { paymentMethod }),
  confirmPayment: (id: string, transactionReference: string) =>
    api.post<ApiResponse<Booking>>(`/bookings/${id}/payment/confirm`, { transactionReference }),
  accept: (id: string) => api.post<ApiResponse<Booking>>(`/bookings/${id}/accept`),
  reject: (id: string, reason: string) =>
    api.post<ApiResponse<Booking>>(`/bookings/${id}/reject`, { reason }),
  cancel: (id: string, reason: string) =>
    api.post<ApiResponse<Booking>>(`/bookings/${id}/cancel`, { reason }),
  complete: (id: string) => api.post<ApiResponse<Booking>>(`/bookings/${id}/complete`),
  reschedule: (id: string, newScheduledAt: string, reason?: string) =>
    api.post<ApiResponse<Booking>>(`/bookings/${id}/reschedule`, { newScheduledAt, reason }),
};

export const notificationsApi = {
  list: (unreadOnly = false) =>
    api.get<ApiResponse<PagedResult<{ id: string; titleEn: string; titleAr: string; messageEn: string; messageAr: string; isRead: boolean }>>>('/notifications', { params: { unreadOnly } }),
  markRead: (id: string) => api.post(`/notifications/${id}/read`),
};
