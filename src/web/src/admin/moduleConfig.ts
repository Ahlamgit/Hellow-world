export interface AdminListQuery {
  search?: string;
  sortBy?: string;
  sortDirection?: 'asc' | 'desc';
  page?: number;
  pageSize?: number;
  status?: string;
  role?: string;
  type?: string;
  category?: string;
  fromDate?: string;
  toDate?: string;
  isActive?: boolean;
}

export interface AdminRow {
  id: string;
  columns: Record<string, string | null | undefined>;
}

export interface AdminListResult {
  items: AdminRow[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface AdminDashboard {
  totalUsers: number;
  totalCustomers: number;
  totalCraftsmen: number;
  totalStores: number;
  totalBookings: number;
  pendingBookings: number;
  activeSubscriptions: number;
  totalRevenue: number;
  openComplaints: number;
  openTickets: number;
  unreadNotifications: number;
}

export interface AdminSystemHealth {
  status: string;
  timestamp: string;
  databaseConnected: boolean;
  databaseResponseMs: number;
  apiVersion: string;
  memoryUsedMb: number;
}

export interface AdminAnalytics {
  bookingsByMonth: { label: string; value: number }[];
  revenueByMonth: { label: string; value: number }[];
  usersByRole: { label: string; value: number }[];
}

export interface ModuleConfig {
  key: string;
  title: string;
  titleAr: string;
  columns: { key: string; label: string; sortable?: boolean }[];
  bulkActions?: string[];
  filters?: { key: string; label: string; options?: string[] }[];
}

export const ADMIN_MODULES: ModuleConfig[] = [
  { key: 'users', title: 'Users', titleAr: 'المستخدمون', columns: [{ key: 'email', label: 'Email', sortable: true }, { key: 'phone', label: 'Phone' }, { key: 'role', label: 'Role' }, { key: 'status', label: 'Status' }, { key: 'createdAt', label: 'Created' }], bulkActions: ['activate', 'deactivate', 'suspend', 'delete'], filters: [{ key: 'status', label: 'Status', options: ['Active', 'Pending', 'Suspended'] }, { key: 'role', label: 'Role', options: ['Customer', 'Craftsman', 'Store', 'Administrator'] }] },
  { key: 'customers', title: 'Customers', titleAr: 'العملاء', columns: [{ key: 'email', label: 'Email' }, { key: 'name', label: 'Name' }, { key: 'phone', label: 'Phone' }, { key: 'status', label: 'Status' }], bulkActions: ['activate', 'suspend', 'delete'] },
  { key: 'craftsmen', title: 'Craftsmen', titleAr: 'الحرفيون', columns: [{ key: 'email', label: 'Email' }, { key: 'name', label: 'Name' }, { key: 'specialization', label: 'Specialization' }, { key: 'rating', label: 'Rating' }, { key: 'status', label: 'Status' }], bulkActions: ['activate', 'suspend'] },
  { key: 'stores', title: 'Stores', titleAr: 'المتاجر', columns: [{ key: 'email', label: 'Email' }, { key: 'name', label: 'Name' }, { key: 'status', label: 'Status' }, { key: 'createdAt', label: 'Created' }], bulkActions: ['activate', 'suspend'] },
  { key: 'categories', title: 'Categories', titleAr: 'الفئات', columns: [{ key: 'nameEn', label: 'Name (EN)' }, { key: 'nameAr', label: 'Name (AR)' }, { key: 'displayOrder', label: 'Order' }, { key: 'isActive', label: 'Active' }], bulkActions: ['activate', 'deactivate', 'delete'] },
  { key: 'services', title: 'Services', titleAr: 'الخدمات', columns: [{ key: 'nameEn', label: 'Name' }, { key: 'basePrice', label: 'Price' }, { key: 'isActive', label: 'Active' }], bulkActions: ['activate', 'deactivate', 'delete'] },
  { key: 'bookings', title: 'Bookings', titleAr: 'الحجوزات', columns: [{ key: 'reference', label: 'Reference' }, { key: 'service', label: 'Service' }, { key: 'customer', label: 'Customer' }, { key: 'status', label: 'Status' }, { key: 'scheduledAt', label: 'Scheduled' }], bulkActions: ['delete'], filters: [{ key: 'status', label: 'Status' }] },
  { key: 'subscriptions', title: 'Subscriptions', titleAr: 'الاشتراكات', columns: [{ key: 'planCode', label: 'Code' }, { key: 'nameEn', label: 'Name' }, { key: 'price', label: 'Price' }, { key: 'status', label: 'Status' }, { key: 'targetRole', label: 'Role' }], bulkActions: ['activate', 'deactivate', 'archive'] },
  { key: 'advertisements', title: 'Advertisements', titleAr: 'الإعلانات', columns: [{ key: 'titleEn', label: 'Title' }, { key: 'placement', label: 'Placement' }, { key: 'startDate', label: 'Start' }, { key: 'isActive', label: 'Active' }], bulkActions: ['activate', 'deactivate', 'delete'] },
  { key: 'coupons', title: 'Coupons', titleAr: 'الكوبونات', columns: [{ key: 'code', label: 'Code' }, { key: 'discount', label: 'Discount' }, { key: 'usedCount', label: 'Used' }, { key: 'validTo', label: 'Valid To' }, { key: 'isActive', label: 'Active' }], bulkActions: ['activate', 'deactivate', 'delete'] },
  { key: 'notifications', title: 'Notifications', titleAr: 'الإشعارات', columns: [{ key: 'titleEn', label: 'Title' }, { key: 'type', label: 'Type' }, { key: 'isRead', label: 'Read' }, { key: 'createdAt', label: 'Created' }], bulkActions: ['markread'] },
  { key: 'payments', title: 'Payments', titleAr: 'المدفوعات', columns: [{ key: 'amount', label: 'Amount' }, { key: 'currency', label: 'Currency' }, { key: 'status', label: 'Status' }, { key: 'method', label: 'Method' }, { key: 'paidAt', label: 'Paid At' }], filters: [{ key: 'status', label: 'Status' }] },
  { key: 'reports', title: 'Reports', titleAr: 'التقارير', columns: [{ key: 'name', label: 'Name' }, { key: 'type', label: 'Type' }, { key: 'status', label: 'Status' }, { key: 'generatedAt', label: 'Generated' }] },
  { key: 'complaints', title: 'Complaints', titleAr: 'الشكاوى', columns: [{ key: 'subject', label: 'Subject' }, { key: 'status', label: 'Status' }, { key: 'priority', label: 'Priority' }, { key: 'createdAt', label: 'Created' }], bulkActions: ['resolve'], filters: [{ key: 'status', label: 'Status', options: ['Open', 'Resolved'] }] },
  { key: 'support-tickets', title: 'Support Tickets', titleAr: 'تذاكر الدعم', columns: [{ key: 'ticketNumber', label: 'Ticket #' }, { key: 'subject', label: 'Subject' }, { key: 'status', label: 'Status' }, { key: 'priority', label: 'Priority' }], bulkActions: ['close'], filters: [{ key: 'status', label: 'Status', options: ['Open', 'Closed'] }] },
  { key: 'cities', title: 'Cities', titleAr: 'المدن', columns: [{ key: 'nameEn', label: 'Name (EN)' }, { key: 'nameAr', label: 'Name (AR)' }, { key: 'code', label: 'Code' }, { key: 'isActive', label: 'Active' }], bulkActions: ['activate', 'deactivate'] },
  { key: 'regions', title: 'Regions', titleAr: 'المناطق', columns: [{ key: 'nameEn', label: 'Name (EN)' }, { key: 'nameAr', label: 'Name (AR)' }, { key: 'code', label: 'Code' }, { key: 'isActive', label: 'Active' }], bulkActions: ['activate', 'deactivate'] },
  { key: 'settings', title: 'Settings', titleAr: 'الإعدادات', columns: [{ key: 'key', label: 'Key' }, { key: 'value', label: 'Value' }, { key: 'category', label: 'Category' }] },
  { key: 'roles', title: 'Roles', titleAr: 'الأدوار', columns: [{ key: 'role', label: 'Role' }, { key: 'description', label: 'Description' }] },
  { key: 'permissions', title: 'Permissions', titleAr: 'الصلاحيات', columns: [{ key: 'code', label: 'Code' }, { key: 'module', label: 'Module' }, { key: 'nameEn', label: 'Name' }] },
  { key: 'audit-logs', title: 'Audit Logs', titleAr: 'سجلات التدقيق', columns: [{ key: 'table', label: 'Table' }, { key: 'action', label: 'Action' }, { key: 'userEmail', label: 'User' }, { key: 'createdAt', label: 'Time' }] },
  { key: 'activity-logs', title: 'Activity Logs', titleAr: 'سجلات النشاط', columns: [{ key: 'action', label: 'Action' }, { key: 'module', label: 'Module' }, { key: 'details', label: 'Details' }, { key: 'createdAt', label: 'Time' }] },
  { key: 'backup', title: 'Backup', titleAr: 'النسخ الاحتياطي', columns: [{ key: 'name', label: 'Name' }, { key: 'status', label: 'Status' }, { key: 'sizeBytes', label: 'Size' }, { key: 'createdAt', label: 'Created' }] },
  { key: 'restore', title: 'Restore', titleAr: 'الاستعادة', columns: [{ key: 'name', label: 'Name' }, { key: 'status', label: 'Status' }, { key: 'completedAt', label: 'Completed' }, { key: 'createdAt', label: 'Created' }] },
];

export function getModuleConfig(key: string): ModuleConfig | undefined {
  return ADMIN_MODULES.find((m) => m.key === key);
}
