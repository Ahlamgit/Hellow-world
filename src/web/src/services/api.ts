import axios from 'axios';

const API_BASE_URL = import.meta.env.VITE_API_URL || 'http://localhost:5000/api/v1';

export const api = axios.create({
  baseURL: API_BASE_URL,
  headers: { 'Content-Type': 'application/json' },
});

api.interceptors.request.use((config) => {
  const token = localStorage.getItem('accessToken');
  if (token) config.headers.Authorization = `Bearer ${token}`;
  const sessionId = localStorage.getItem('sessionId');
  if (sessionId) config.headers['X-Session-Id'] = sessionId;
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
          if (data.data.sessionId) localStorage.setItem('sessionId', data.data.sessionId);
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
  sessionId: string;
  user: UserDto;
}

export interface UserDto {
  id: string;
  email: string;
  phone: string;
  role: string;
  primaryRole?: string;
  roles?: string[];
  status: string;
  verificationStatus: string;
  subscriptionStatus: string;
  firstName: string;
  lastName: string;
  profilePictureUrl?: string;
  preferredLanguage: string;
  emailVerified?: boolean;
  phoneVerified?: boolean;
  requiresEmailVerification?: boolean;
  permissions?: string[];
}

export interface ProfileDto {
  id: string;
  email: string;
  phone: string;
  firstName: string;
  lastName: string;
  fullName: string;
  gender?: string;
  birthDate?: string;
  nationality?: string;
  profilePictureUrl?: string;
  addressLine?: string;
  country?: string;
  city?: string;
  region?: string;
  preferredLanguage: string;
  timezone: string;
  emailVerified: boolean;
  phoneVerified: boolean;
}

export interface SessionDto {
  id: string;
  deviceName?: string;
  platform?: string;
  browser?: string;
  ipAddress?: string;
  rememberMe: boolean;
  createdAt: string;
  lastActivityAt?: string;
  expiresAt: string;
  isCurrent: boolean;
}

export interface UpdateProfileRequest {
  firstName: string;
  lastName: string;
  gender?: string;
  birthDate?: string;
  nationality?: string;
  profilePictureUrl?: string;
  addressLine?: string;
  country?: string;
  city?: string;
  region?: string;
  preferredLanguage: string;
  timezone: string;
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
  distanceKm?: number;
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
  sessionId?: string;
  checkoutUrl?: string;
  provider?: string;
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
  customerRating?: number;
  customerReview?: string;
  payment?: BookingPayment;
  statusHistory: { oldStatus?: string; newStatus: string; notes?: string; createdAt: string }[];
}

export const authApi = {
  login: (email: string, password: string, rememberMe = false) =>
    api.post<ApiResponse<AuthResponse>>('/auth/login', { email, password, rememberMe }),
  register: (data: Record<string, string>) =>
    api.post<ApiResponse<AuthResponse>>('/auth/register', data),
  logout: () => api.post('/auth/revoke', { refreshToken: localStorage.getItem('refreshToken') }),
  forgotPassword: (email: string) =>
    api.post<ApiResponse<{ message: string }>>('/auth/forgot-password', { email }),
  resetPassword: (token: string, newPassword: string, confirmPassword: string) =>
    api.post<ApiResponse<{ message: string }>>('/auth/reset-password', { token, newPassword, confirmPassword }),
  changePassword: (currentPassword: string, newPassword: string, confirmPassword: string) =>
    api.post<ApiResponse<{ message: string }>>('/auth/change-password', { currentPassword, newPassword, confirmPassword }),
  verifyEmail: (token: string) =>
    api.post<ApiResponse<{ message: string }>>('/auth/verify-email', { token }),
  resendEmailVerification: (email: string) =>
    api.post<ApiResponse<{ message: string }>>('/auth/resend-email-verification', { email }),
  getMe: () => api.get<ApiResponse<{ permissions: string[] }>>('/auth/me'),
  getPermissions: () => api.get<ApiResponse<string[]>>('/auth/permissions'),
};

export const identityApi = {
  getProfile: () => api.get<ApiResponse<ProfileDto>>('/profile'),
  updateProfile: (data: UpdateProfileRequest) =>
    api.put<ApiResponse<ProfileDto>>('/profile', data),
  getSessions: () => api.get<ApiResponse<SessionDto[]>>('/sessions'),
  revokeSession: (sessionId: string) => api.delete(`/sessions/${sessionId}`),
  revokeOtherSessions: () => api.delete('/sessions/others'),
  revokeAllSessions: () => api.delete('/sessions'),
};

export const servicesApi = {
  getCategories: () => api.get<ApiResponse<ServiceCategory[]>>('/services/categories'),
  getServices: (categoryId?: string) =>
    api.get<ApiResponse<Service[]>>('/services', { params: { categoryId } }),
};

export const usersApi = {
  getProfile: () => api.get<ApiResponse<UserDto>>('/users/me'),
  listAddresses: () => api.get<ApiResponse<AddressDto[]>>('/users/me/addresses'),
  addAddress: (data: CreateAddressDto) => api.post<ApiResponse<AddressDto>>('/users/me/addresses', data),
  updateAddress: (id: string, data: CreateAddressDto) => api.put<ApiResponse<AddressDto>>(`/users/me/addresses/${id}`, data),
  deleteAddress: (id: string) => api.delete(`/users/me/addresses/${id}`),
};

export interface AddressDto {
  id: string;
  label: string;
  street: string;
  city: string;
  district?: string;
  postalCode?: string;
  country: string;
  latitude?: number;
  longitude?: number;
  isDefault: boolean;
}

export interface CreateAddressDto {
  label: string;
  street: string;
  city: string;
  district?: string;
  postalCode?: string;
  country: string;
  latitude?: number;
  longitude?: number;
  isDefault: boolean;
}

export const bookingsApi = {
  getCraftsmen: (serviceId: string) =>
    api.get<ApiResponse<CraftsmanOption[]>>('/bookings/craftsmen', { params: { serviceId } }),
  getNearbyCraftsmen: (serviceId: string, latitude: number, longitude: number, radiusKm = 25) =>
    api.get<ApiResponse<CraftsmanOption[]>>('/bookings/craftsmen/nearby', {
      params: { serviceId, latitude, longitude, radiusKm },
    }),
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
  noShow: (id: string) => api.post<ApiResponse<Booking>>(`/bookings/${id}/no-show`),
  submitReview: (id: string, rating: number, review?: string) =>
    api.post<ApiResponse<Booking>>(`/bookings/${id}/review`, { rating, review }),
};

export interface Notification {
  id: string;
  titleEn: string;
  titleAr: string;
  messageEn: string;
  messageAr: string;
  notificationType: string;
  referenceId?: string;
  isRead: boolean;
  createdAt: string;
}

export const notificationsApi = {
  list: (params?: { unreadOnly?: boolean; page?: number; pageSize?: number }) =>
    api.get<ApiResponse<PagedResult<Notification>>>('/notifications', {
      params: {
        unreadOnly: params?.unreadOnly ?? false,
        page: params?.page ?? 1,
        pageSize: params?.pageSize ?? 20,
      },
    }),
  markRead: (id: string) => api.post<ApiResponse<object>>(`/notifications/${id}/read`),
  unreadCount: async () => {
    const res = await api.get<ApiResponse<PagedResult<Notification>>>('/notifications', {
      params: { unreadOnly: true, page: 1, pageSize: 1 },
    });
    return res.data.data.totalCount;
  },
};

export interface ChatConversation {
  id: string;
  bookingId: string;
  bookingReference: string;
  serviceName: string;
  customerId: string;
  customerName: string;
  craftsmanId: string;
  craftsmanName: string;
  lastMessageAt?: string;
  lastMessagePreview?: string;
  unreadCount: number;
}

export interface ChatMessage {
  id: string;
  conversationId: string;
  senderId: string;
  senderName: string;
  body: string;
  sentAt: string;
  isRead: boolean;
  isMine: boolean;
}

export const chatApi = {
  listConversations: () => api.get<ApiResponse<ChatConversation[]>>('/chat/conversations'),
  getBookingChat: (bookingId: string) =>
    api.get<ApiResponse<ChatConversation>>(`/chat/bookings/${bookingId}`),
  getMessages: (conversationId: string, page = 1, pageSize = 50) =>
    api.get<ApiResponse<PagedResult<ChatMessage>>>(`/chat/conversations/${conversationId}/messages`, {
      params: { page, pageSize },
    }),
  sendMessage: (conversationId: string, body: string) =>
    api.post<ApiResponse<ChatMessage>>(`/chat/conversations/${conversationId}/messages`, { body }),
};

export const craftsmanApi = {
  getProfile: () => api.get<ApiResponse<CraftsmanProfile>>('/me/craftsman'),
  updateProfile: (data: Partial<CraftsmanProfile>) => api.put<ApiResponse<CraftsmanProfile>>('/me/craftsman', data),
  upsertService: (data: { serviceId: string; customPrice: number; isAvailable: boolean }) =>
    api.post<ApiResponse<CraftsmanServiceItem>>('/me/craftsman/services', data),
  deleteService: (id: string) => api.delete(`/me/craftsman/services/${id}`),
  upsertWorkingHour: (data: { dayOfWeek: number; startTime: string; endTime: string; isActive: boolean }) =>
    api.post<ApiResponse<CraftsmanWorkingHour>>('/me/craftsman/working-hours', data),
  deleteWorkingHour: (id: string) => api.delete(`/me/craftsman/working-hours/${id}`),
};

export interface CraftsmanProfile {
  id: string;
  userId: string;
  specialization?: string;
  yearsOfExperience: number;
  rating: number;
  totalReviews: number;
  completedJobs: number;
  isAvailable: boolean;
  serviceRadiusKm?: number;
  licenseNumber?: string;
  services: CraftsmanServiceItem[];
  workingHours: CraftsmanWorkingHour[];
}

export interface CraftsmanServiceItem {
  id: string;
  serviceId: string;
  serviceNameEn: string;
  serviceNameAr: string;
  customPrice: number;
  isAvailable: boolean;
}

export interface CraftsmanWorkingHour {
  id: string;
  dayOfWeek: number;
  startTime: string;
  endTime: string;
  isActive: boolean;
}

export const storeApi = {
  getProfile: () => api.get<ApiResponse<StoreProfile>>('/me/store'),
  updateProfile: (data: Partial<StoreProfile>) => api.put<ApiResponse<StoreProfile>>('/me/store', data),
  createProduct: (data: StoreProductInput) => api.post<ApiResponse<StoreProduct>>('/me/store/products', data),
  updateProduct: (id: string, data: StoreProductInput) => api.put<ApiResponse<StoreProduct>>(`/me/store/products/${id}`, data),
  deleteProduct: (id: string) => api.delete(`/me/store/products/${id}`),
};

export interface StoreProfile {
  id: string;
  userId: string;
  storeName: string;
  commercialRegistration?: string;
  description?: string;
  rating: number;
  totalReviews: number;
  isOpen: boolean;
  openingTime?: string;
  closingTime?: string;
  products: StoreProduct[];
}

export interface StoreProduct {
  id: string;
  nameAr: string;
  nameEn: string;
  descriptionAr?: string;
  descriptionEn?: string;
  price: number;
  stockQuantity: number;
  sku?: string;
  imageUrl?: string;
  isActive: boolean;
}

export interface StoreProductInput {
  nameAr: string;
  nameEn: string;
  descriptionAr?: string;
  descriptionEn?: string;
  price: number;
  stockQuantity: number;
  sku?: string;
  imageUrl?: string;
  isActive: boolean;
}

export const supportApi = {
  createComplaint: (data: { subject: string; description: string; priority?: string }) =>
    api.post<ApiResponse<Complaint>>('/complaints', data),
  myComplaints: () => api.get<ApiResponse<Complaint[]>>('/complaints/mine'),
  createTicket: (data: { subject: string; description: string; category?: string; priority?: string }) =>
    api.post<ApiResponse<SupportTicket>>('/support-tickets', data),
  myTickets: () => api.get<ApiResponse<SupportTicket[]>>('/support-tickets/mine'),
  validateCoupon: (code: string, amount: number) =>
    api.get<ApiResponse<CouponValidation>>('/coupons/validate', { params: { code, amount } }),
  getAds: (placement = 'HomePage') => api.get<ApiResponse<Advertisement[]>>('/advertisements', { params: { placement } }),
};

export interface Complaint {
  id: string;
  subject: string;
  description: string;
  status: string;
  priority: string;
  createdAt: string;
}

export interface SupportTicket {
  id: string;
  ticketNumber: string;
  subject: string;
  description: string;
  status: string;
  priority: string;
  category: string;
  createdAt: string;
}

export interface CouponValidation {
  isValid: boolean;
  code: string;
  discountAmount: number;
  finalAmount: number;
  message?: string;
}

export interface Advertisement {
  id: string;
  titleEn: string;
  titleAr: string;
  placement: string;
}
