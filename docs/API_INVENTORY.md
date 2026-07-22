# KHADAMATI — API Inventory (Phase 0)

**Document version:** 1.0  
**Date:** 2026-07-22  
**Base URL:** `http://localhost:5000/api/v1` (Development)  
**Format:** JSON (`ApiResponse<T>` envelope)  
**Auth:** JWT Bearer (`Authorization: Bearer <token>`) unless marked **Public**

---

## 1. Controller Summary

| # | Controller | Route Prefix | Auth |
|---|------------|--------------|------|
| 1 | `HealthController` | `/health` | Public |
| 2 | `AuthController` | `/auth` | Mixed |
| 3 | `UsersController` | `/users` | JWT |
| 4 | `ServicesController` | `/services` | Public (read) |
| 5 | `BookingsController` | `/bookings` | JWT |
| 6 | `NotificationsController` | `/notifications` | JWT |
| 7 | `ProfileController` | `/profile` | JWT |
| 8 | `SessionsController` | `/sessions` | JWT + permissions |
| 9 | `LoginHistoryController` | `/login-history` | JWT + permissions |
| 10 | `LocationsController` | `/locations` | Public |
| 11 | `SubscriptionPlansController` | `/subscription-plans` | Public (read) |
| 12 | `MeSubscriptionController` | `/me` | JWT |
| 13 | `MeCraftsmanController` | `/me/craftsman` | JWT |
| 14 | `MeStoreController` | `/me/store` | JWT |
| 15 | `DevicesController` | `/devices` | JWT |
| 16 | `ChatController` | `/chat` | JWT |
| 17 | `SupportController` | `/api/v1` (root) | Mixed |
| 18 | `VerificationController` | `/verification/documents` | JWT |
| 19 | `AdminController` | `/admin` | JWT + permissions |
| 20 | `AdminUsersController` | `/admin/users` | JWT + permissions |
| 21 | `AdminCatalogController` | `/admin/categories`, `/admin/services` | JWT + permissions |
| 22 | `AdminBookingsController` | `/admin/bookings` | JWT + permissions |
| 23 | `AdminSubscriptionPlansController` | `/admin/subscription-plans` | JWT + permissions |
| 24 | `AdminUserSubscriptionsController` | `/admin/user-subscriptions` | JWT + permissions |
| 25 | `AdminMarketingController` | `/admin/coupons`, `/admin/advertisements` | JWT + permissions |
| 26 | `AdminOperationsControllers` | `/admin/complaints`, `/admin/support-tickets`, `/admin/payments` | JWT + permissions |
| 27 | `AdminLocationsController` | `/admin/regions`, `/admin/cities` | JWT + permissions |
| 28 | `AdminSettingsController` | `/admin/settings` | JWT + permissions |
| 29 | `AdminRbacController` | `/admin/rbac` | JWT + permissions |
| 30 | `AdminVerificationDocumentsController` | `/admin/verification-documents` | JWT + permissions |
| 31 | Moyasar webhook | `/webhooks/moyasar` | Webhook signature |

**Total route groups:** 24 controller files, ~31 logical API groups, **~150+ endpoints**.

---

## 2. Health & Operations

### `GET /health` — Public
Liveness check. Returns `{ status: "healthy" }`.

### `GET /health/ready` — Public
Readiness including database connectivity.

### `GET /health/integrations` — JWT
Integration provider readiness (email, SMS, payment, push).

---

## 3. Authentication (`/auth`)

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| POST | `/auth/register` | Public | Register new user |
| POST | `/auth/login` | Public | Login; returns tokens |
| POST | `/auth/refresh` | Public | Refresh access token |
| POST | `/auth/revoke` | JWT | Logout current session |
| POST | `/auth/forgot-password` | Public | Send reset email |
| POST | `/auth/reset-password` | Public | Reset with token |
| POST | `/auth/change-password` | JWT | Change password |
| POST | `/auth/verify-email` | Public | Verify email token |
| POST | `/auth/resend-email-verification` | JWT | Resend verification |
| POST | `/auth/admin/verify-email` | JWT + Admin | Force-verify user |
| POST | `/auth/phone/send-otp` | JWT | Send phone OTP |
| POST | `/auth/phone/verify-otp` | JWT | Verify phone OTP |
| GET | `/auth/me` | JWT | Current user summary |
| GET | `/auth/permissions` | JWT | Current user permissions |

---

## 4. Users & Profile

### `/users` (JWT)

| Method | Path | Description |
|--------|------|-------------|
| GET | `/users/me` | User profile DTO |
| PUT | `/users/me` | Update profile |
| GET | `/users/me/addresses` | List addresses |
| POST | `/users/me/addresses` | Create address |
| PUT | `/users/me/addresses/{id}` | Update address |
| DELETE | `/users/me/addresses/{id}` | Delete address |

### `/profile` (JWT)

| Method | Path | Description |
|--------|------|-------------|
| GET | `/profile` | Profile details |
| PUT | `/profile` | Update profile |

### `/sessions` (JWT)

| Method | Path | Permission | Description |
|--------|------|------------|-------------|
| GET | `/sessions` | SessionsView | My active sessions |
| GET | `/sessions/user/{userId}` | SessionsView | User sessions (admin) |
| DELETE | `/sessions/{sessionId}` | — | Revoke session |
| DELETE | `/sessions/others` | — | Logout other devices |
| DELETE | `/sessions` | SessionsRevokeAll | Logout all devices |

### `/login-history` (JWT)

| Method | Path | Permission | Description |
|--------|------|------------|-------------|
| GET | `/login-history` | LoginHistoryView | Paginated login history |

---

## 5. Service Catalog

### `/services` — Public read

| Method | Path | Description |
|--------|------|-------------|
| GET | `/services/categories` | All categories |
| GET | `/services` | Services (`?categoryId=`) |

---

## 6. Bookings (`/bookings`, JWT)

| Method | Path | Description |
|--------|------|-------------|
| GET | `/bookings/craftsmen` | Search craftsmen (`serviceId`, filters) |
| GET | `/bookings/craftsmen/nearby` | Geo search (`latitude`, `longitude`, `radiusKm`) |
| GET | `/bookings/availability` | Available slots |
| POST | `/bookings` | Create booking |
| GET | `/bookings` | My bookings (paginated) |
| GET | `/bookings/{id}` | Booking detail |
| POST | `/bookings/{id}/confirm` | Customer confirm |
| POST | `/bookings/{id}/payment` | Initiate payment |
| POST | `/bookings/{id}/payment/confirm` | Confirm payment |
| POST | `/bookings/{id}/accept` | Craftsman accept |
| POST | `/bookings/{id}/reject` | Craftsman reject |
| POST | `/bookings/{id}/cancel` | Cancel booking |
| POST | `/bookings/{id}/complete` | Mark complete |
| POST | `/bookings/{id}/no-show` | Mark no-show |
| POST | `/bookings/{id}/reschedule` | Reschedule |

---

## 7. Notifications (`/notifications`, JWT)

| Method | Path | Description |
|--------|------|-------------|
| GET | `/notifications` | List (`?unreadOnly`, `page`, `pageSize`) |
| POST | `/notifications/{id}/read` | Mark read |

---

## 8. Subscriptions

### `/subscription-plans` — Public read

| Method | Path | Description |
|--------|------|-------------|
| GET | `/subscription-plans` | Active plans |
| GET | `/subscription-plans/{id}` | Plan detail |

### `/me` (JWT) — subscription subset

| Method | Path | Description |
|--------|------|-------------|
| GET | `/me/subscription` | Current subscription |
| GET | `/me/subscriptions` | Subscription history |
| POST | `/me/subscription` | Subscribe to plan |
| POST | `/me/subscription/{id}/cancel` | Cancel subscription |
| PATCH | `/me/subscription/auto-renew` | Toggle auto-renew |

---

## 9. Craftsman & Store Portals (JWT)

### `/me/craftsman`

| Method | Path | Description |
|--------|------|-------------|
| GET | `/me/craftsman` | Craftsman profile |
| PUT | `/me/craftsman` | Update profile |
| POST | `/me/craftsman/services` | Link service |
| DELETE | `/me/craftsman/services/{id}` | Unlink service |
| POST | `/me/craftsman/working-hours` | Add working hours |
| DELETE | `/me/craftsman/working-hours/{id}` | Remove hours |

### `/me/store`

| Method | Path | Description |
|--------|------|-------------|
| GET | `/me/store` | Store profile |
| PUT | `/me/store` | Update profile |
| POST | `/me/store/products` | Add product |
| PUT | `/me/store/products/{id}` | Update product |
| DELETE | `/me/store/products/{id}` | Delete product |

---

## 10. Messaging

### `/devices` (JWT)

| Method | Path | Description |
|--------|------|-------------|
| POST | `/devices/push-token` | Register FCM/APNs token |
| DELETE | `/devices/push-token` | Unregister token |

### `/chat` (JWT)

| Method | Path | Description |
|--------|------|-------------|
| GET | `/chat/conversations` | My conversations |
| GET | `/chat/bookings/{bookingId}` | Get/create conversation |
| GET | `/chat/conversations/{id}/messages` | Message history |
| POST | `/chat/conversations/{id}/messages` | Send message |

---

## 11. Support & Public Content

### `/api/v1` root routes (`SupportController`)

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| POST | `/complaints` | JWT | File complaint |
| GET | `/complaints/mine` | JWT | My complaints |
| POST | `/support-tickets` | JWT | Open ticket |
| GET | `/support-tickets/mine` | JWT | My tickets |
| POST | `/bookings/{id}/review` | JWT | Submit review |
| GET | `/coupons/validate` | JWT | Validate coupon code |
| GET | `/advertisements` | Public | Active ads by placement |

### `/verification/documents` (JWT)

| Method | Path | Description |
|--------|------|-------------|
| POST | `/verification/documents` | Upload document |

### `/locations` — Public

| Method | Path | Description |
|--------|------|-------------|
| GET | `/locations/regions` | Active regions |
| GET | `/locations/cities` | Cities (`?regionId=`) |

---

## 12. Admin API (`/admin`, JWT + `HasPermission`)

### Core Admin (`AdminController`)

| Method | Path | Description |
|--------|------|-------------|
| GET | `/admin/dashboard` | KPI dashboard |
| GET | `/admin/{module}` | Generic module list |
| POST | `/admin/{module}/bulk` | Bulk actions |
| GET | `/admin/{module}/export` | Export module data |
| GET | `/admin/analytics/data` | Analytics charts |
| GET | `/admin/reports/list` | Report list |
| POST | `/admin/reports/generate` | Generate report |
| GET | `/admin/system/health` | System health detail |
| GET | `/admin/backup/list` | List backups |
| POST | `/admin/backup/create` | Create backup |
| POST | `/admin/backup/restore` | Restore backup |
| GET | `/admin/backup/{id}/download` | Download backup file |

**Supported `{module}` keys:** `users`, `customers`, `craftsmen`, `stores`, `categories`, `services`, `bookings`, `subscriptions`, `advertisements`, `coupons`, `notifications`, `payments`, `reports`, `complaints`, `verification-documents`, `support-tickets`, `cities`, `regions`, `settings`, `roles`, `permissions`, `audit-logs`, `activity-logs`, `backup`, `restore`

### User Management (`/admin/users`)

| Method | Path | Description |
|--------|------|-------------|
| GET | `/admin/users` | List users |
| GET | `/admin/users/{id}` | User detail |
| POST | `/admin/users` | Create user |
| PUT | `/admin/users/{id}` | Update user |
| DELETE | `/admin/users/{id}` | Soft-delete user |
| POST | `/admin/users/{id}/suspend` | Suspend |
| POST | `/admin/users/{id}/activate` | Activate |
| PUT | `/admin/users/{id}/roles` | Set roles |
| POST | `/admin/users/{id}/verify-email` | Verify email |
| GET | `/admin/users/{id}/permissions` | Get permissions |
| PUT | `/admin/users/{id}/permissions` | Set permissions |

### Catalog (`/admin/categories`, `/admin/services`)

Standard CRUD: `GET`, `GET/{id}`, `POST`, `PUT/{id}`, `DELETE/{id}`

### Bookings (`/admin/bookings`)

| Method | Path | Description |
|--------|------|-------------|
| GET | `/admin/bookings` | List all bookings |
| GET | `/admin/bookings/stats` | Booking statistics |
| GET | `/admin/bookings/{id}` | Booking detail |

### Subscription Plans (`/admin/subscription-plans`)

CRUD + lifecycle: `clone`, `activate`, `deactivate`, `suspend`, `archive`

### User Subscriptions (`/admin/user-subscriptions`)

| Method | Path | Description |
|--------|------|-------------|
| GET | `/admin/user-subscriptions` | List |
| GET | `/admin/user-subscriptions/{id}` | Detail |
| POST | `/admin/user-subscriptions/users/{userId}` | Assign |
| POST | `/admin/user-subscriptions/{id}/cancel` | Cancel |

### Marketing (`/admin/coupons`, `/admin/advertisements`)

Standard CRUD on both resources.

### Operations

| Resource | Key actions |
|----------|-------------|
| `/admin/complaints` | GET, GET/{id}, POST/{id}/resolve |
| `/admin/support-tickets` | GET, GET/{id}, POST/{id}/close |
| `/admin/payments` | GET, GET/{id} |

### Locations (`/admin/regions`, `/admin/cities`)

CRUD + `POST /admin/regions/seed` (bulk seed)

### Settings (`/admin/settings`)

| Method | Path | Description |
|--------|------|-------------|
| GET | `/admin/settings` | List settings |
| PUT | `/admin/settings/{id}` | Update setting |

### RBAC (`/admin/rbac`)

| Method | Path | Description |
|--------|------|-------------|
| GET | `/admin/rbac/roles` | List roles |
| GET | `/admin/rbac/roles/{roleId}/permissions` | Role permissions |
| PUT | `/admin/rbac/roles/{roleId}/permissions` | Update role permissions |

### Verification (`/admin/verification-documents`)

| Method | Path | Description |
|--------|------|-------------|
| GET | `/admin/verification-documents` | List |
| GET | `/admin/verification-documents/{id}` | Detail |
| POST | `/admin/verification-documents/{id}/approve` | Approve |
| POST | `/admin/verification-documents/{id}/reject` | Reject |

---

## 13. Webhooks

| Method | Path | Auth | Description |
|--------|------|------|-------------|
| POST | `/webhooks/moyasar` | Webhook secret | Payment status callback |

---

## 14. Cross-Cutting Concerns

| Concern | Implementation |
|---------|----------------|
| Rate limiting | AspNetCoreRateLimit — 100/min general; 10/min login; 5/min register |
| CORS | `Cors:AllowedOrigins` in appsettings |
| Versioning | URL prefix `api/v1` |
| Errors | `ApiResponse` with `success`, `message`, `errors` |
| Pagination | `page`, `pageSize` query params on list endpoints |
| Session header | `X-Session-Id` for session-aware operations |
| Swagger | `/swagger` in Development |
| Permissions | `[HasPermission(PermissionCodes.*)]` on admin endpoints |

---

## 15. Client Configuration

| Client | API base URL config |
|--------|---------------------|
| Web | `VITE_API_URL` → `src/web/.env.development` (`http://localhost:5000/api/v1`) |
| Android | Build config / `ApiClient` |
| iOS | `API_BASE_URL` in app config |

---

## 16. Related Documents

- [TRACEABILITY_MATRIX.md](./TRACEABILITY_MATRIX.md)
- [MODULE_INVENTORY.md](./MODULE_INVENTORY.md)
- [DEPLOYMENT_RUNBOOK.md](./DEPLOYMENT_RUNBOOK.md)
