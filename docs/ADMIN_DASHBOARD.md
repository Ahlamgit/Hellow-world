# Enterprise Administration Dashboard

The Khadamati Enterprise Administration Dashboard provides a unified admin interface for managing all platform modules with consistent search, filtering, sorting, pagination, bulk actions, and Excel/PDF export.

## Access

- **URL:** `/admin` (web)
- **API base:** `/api/v1/admin`
- **Policy:** `AdminOnly` (requires `Administrator` role)
- **Default admin:** `admin@khadamati.com` / `Admin@123456`

## Modules (26)

| Module | API Key | Features |
|--------|---------|----------|
| Dashboard | `GET /dashboard` | KPI cards |
| Users | `users` | CRUD list, bulk activate/deactivate/suspend/delete |
| Customers | `customers` | Filtered user list |
| Craftsmen | `craftsmen` | Craftsman profiles |
| Stores | `stores` | Store profiles |
| Categories | `categories` | Service categories |
| Services | `services` | Service catalog |
| Bookings | `bookings` | All bookings |
| Subscriptions | `subscriptions` | Subscription plans |
| Advertisements | `advertisements` | Ad campaigns |
| Coupons | `coupons` | Discount codes |
| Notifications | `notifications` | Platform notifications |
| Payments | `payments` | Payment records |
| Reports | `reports` | Generated reports |
| Analytics | `GET /analytics/data` | Charts |
| Complaints | `complaints` | User complaints |
| Support Tickets | `support-tickets` | Support queue |
| Cities | `cities` | City management |
| Regions | `regions` | Region management |
| Settings | `settings` | System settings |
| Roles | `roles` | Role listing |
| Permissions | `permissions` | Permission matrix |
| Audit Logs | `audit-logs` | Data audit trail |
| Activity Logs | `activity-logs` | Admin activity |
| Backup | `backup` | Backup jobs + create |
| Restore | `restore` | Restore from backup |
| System Health | `GET /system/health` | Live health check |

## Common List API

Every list module supports:

```
GET /api/v1/admin/{module}?search=&sortBy=&sortDirection=asc|desc&page=1&pageSize=20&status=&role=&fromDate=&toDate=&isActive=
```

### Bulk Actions

```
POST /api/v1/admin/{module}/bulk
{ "action": "activate", "ids": ["guid1", "guid2"], "reason": "optional" }
```

Supported actions: `activate`, `deactivate`, `suspend`, `delete`, `archive`, `markread`, `resolve`, `close`

### Export

```
GET /api/v1/admin/{module}/export?format=xlsx|pdf&search=...
```

Returns Excel (ClosedXML) or PDF (QuestPDF) file download.

## Web UI

- **Layout:** `AdminLayout.tsx` — sidebar navigation grouped by section
- **Table:** `AdminDataTable.tsx` — reusable data grid with all standard features
- **Config:** `moduleConfig.ts` — column definitions, filters, bulk actions per module
- **Routes:** `/admin/*` protected by `AdminRoute` (Administrator only)

## Database

- EF Migration: `AdminDashboard`
- SQL Script: `src/database/011_AdminDashboard.sql`
- New tables: Advertisements, Coupons, Complaints, SupportTickets, Regions, Cities, SystemSettings, Permissions, RolePermissions, ActivityLogs, BackupJobs

## Backup & Restore

```
POST /api/v1/admin/backup/create
POST /api/v1/admin/backup/restore
{ "backupId": "guid", "confirm": true }
```

Restore is simulated in development; production should integrate with actual database backup tooling.
