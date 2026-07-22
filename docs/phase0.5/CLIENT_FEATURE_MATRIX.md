# Client Feature Matrix — Phase 0.5 Discovery

**Generated:** 2026-07-22  
**Scope:** Web (`src/web`), Android (`src/android`), iOS (`src/ios`)

## Summary

| Dimension | Web | Android | iOS |
|-----------|-----|---------|-----|
| Public routes/screens | 8 | 6 (+ splash) | Auth flow + 4 tabs |
| Protected routes/screens | 18 | 17 | 12+ pushed views |
| Admin | Full (`/admin/*`) | None | None |
| i18n keys | 276 en / 276 ar (parity) | `strings.xml` + locale | 187 en / 187 ar (parity) |
| Offline support | None | Partial (Room: user, catalog) | Tokens only (Keychain) |
| Push notifications | N/A (web) | FCM + `devices/push-token` | APNs + push token |

---

## 1. Web Routes (`App.tsx` + `AdminRoutes.tsx`)

### 1.1 Public routes

| Route | Page | Auth guard |
|-------|------|------------|
| `/` | HomePage | — |
| `/services` | ServicesPage | — |
| `/login` | LoginPage | PublicRoute |
| `/register` | RegisterPage | PublicRoute |
| `/forgot-password` | ForgotPasswordPage | PublicRoute |
| `/reset-password` | ResetPasswordPage | PublicRoute |
| `/verify-email` | VerifyEmailPage | PublicRoute |
| `/pay` | PaymentCheckoutPage | PublicRoute |

### 1.2 Protected routes (customer / provider)

| Route | Page | Primary APIs |
|-------|------|--------------|
| `/dashboard` | DashboardPage | — (navigation hub only) |
| `/profile` | ProfilePage | `GET/PUT /profile`, `GET /locations/regions`, `GET /locations/cities` |
| `/change-password` | ChangePasswordPage | `POST /auth/change-password` |
| `/sessions` | SessionsPage | `GET /sessions`, `DELETE /sessions/{id}`, `/sessions/others`, `/sessions` |
| `/bookings` | MyBookingsPage | `GET /bookings` |
| `/bookings/new` | BookingWizardPage | `GET /services`, `GET /bookings/craftsmen`, `GET /bookings/craftsmen/nearby`, `GET /bookings/availability`, `POST /bookings`, `POST /bookings/{id}/confirm`, `GET /users/me/addresses` |
| `/bookings/:id` | BookingDetailPage | `GET /bookings/{id}`, accept/reject/cancel/complete/reschedule/no-show/review |
| `/bookings/:id/payment` | BookingPaymentPage | `POST /bookings/{id}/payment`, `POST /bookings/{id}/payment/confirm` |
| `/subscriptions` | SubscriptionPlansPage | `GET /subscription-plans`, `GET /me/subscription` |
| `/subscriptions/:planId` | SubscribePage | `GET /subscription-plans/{id}`, `GET /coupons/validate`, `POST /me/subscription` |
| `/subscription` | MySubscriptionPage | `GET /me/subscription`, `GET /me/subscriptions`, `PATCH /me/subscription/auto-renew`, `POST /me/subscription/{id}/cancel` |
| `/notifications` | NotificationsPage | `GET /notifications`, `POST /notifications/{id}/read` |
| `/chat` | ChatListPage | `GET /chat/conversations` |
| `/chat/:bookingId` | BookingChatPage | `GET /chat/bookings/{id}`, `GET/POST /chat/conversations/{id}/messages` |
| `/addresses` | AddressesPage | `GET/POST/PUT/DELETE /users/me/addresses`, locations catalog |
| `/craftsman` | CraftsmanPortalPage | `GET/PUT /me/craftsman`, `POST /me/craftsman/services`, `GET /services`, `GET /users/me/addresses` |
| `/store` | StorePortalPage | `GET/PUT /me/store`, `POST /me/store/products` |
| `/support` | SupportPage | `GET/POST /complaints`, `GET/POST /support-tickets` |

### 1.3 Auth context (global)

| Consumer | APIs |
|----------|------|
| `AuthContext` | `POST /auth/login`, `POST /auth/register`, `POST /auth/revoke`, `GET /auth/me`, `GET /auth/permissions` |

### 1.4 Home / marketing

| Page | APIs |
|------|------|
| HomePage | `GET /advertisements?placement=HomePage` |

### 1.5 Admin routes (`/admin/*`)

| Route | Page / component | API module |
|-------|------------------|------------|
| `/admin` | AdminDashboardPage | `GET /admin/dashboard` |
| `/admin/analytics` | AdminAnalyticsPage | `GET /admin/analytics/data` |
| `/admin/reports` | AdminReportsPage | `GET /admin/reports/list`, `POST /admin/reports/generate` |
| `/admin/system-health` | AdminSystemHealthPage | `GET /admin/system/health` |
| `/admin/restore` | AdminRestorePage | `POST /admin/backup/restore` |
| `/admin/users` | AdminUsersPage | `adminUsersApi` → `/admin/users/*` |
| `/admin/customers` | AdminUsersPage (role filter) | same |
| `/admin/craftsmen` | AdminUsersPage (role filter) | same |
| `/admin/stores` | AdminUsersPage (role filter) | same |
| `/admin/categories` | AdminCategoriesPage | `adminCatalogApi` → `/admin/categories/*` |
| `/admin/services` | AdminServicesPage | `adminCatalogApi` → `/admin/services/*` |
| `/admin/bookings` | AdminBookingsPage | `adminOpsApi` → `/admin/bookings/*` |
| `/admin/subscriptions` | AdminSubscriptionPlansPage | `adminSubscriptionPlansApi` |
| `/admin/user-subscriptions` | AdminUserSubscriptionsPage | `adminUserSubscriptionsApi` |
| `/admin/advertisements` | AdminAdvertisementsPage | `adminMarketingApi` |
| `/admin/coupons` | AdminCouponsPage | `adminMarketingApi` |
| `/admin/notifications` | AdminDataTable (generic) | `adminApi.listModule('notifications')` |
| `/admin/payments` | AdminPaymentsPage | `adminOpsApi` → `/admin/payments/*` |
| `/admin/complaints` | AdminComplaintsPage | `adminOpsApi` → `/admin/complaints/*` |
| `/admin/support-tickets` | AdminSupportTicketsPage | `adminOpsApi` → `/admin/support-tickets/*` |
| `/admin/verification-documents` | AdminVerificationDocumentsPage | `adminVerificationApi` |
| `/admin/cities` | AdminCitiesPage | `adminLocationApi` → `/admin/cities/*` |
| `/admin/regions` | AdminRegionsPage | `adminLocationApi` → `/admin/regions/*` |
| `/admin/settings` | AdminSettingsPage | `adminSettingsApi` |
| `/admin/roles` | AdminRbacMatrixPage | `adminRbacApi` → `/admin/rbac/*` |
| `/admin/permissions` | AdminDataTable (generic) | `adminApi.listModule('permissions')` |
| `/admin/audit-logs` | AdminDataTable (generic) | `adminApi.listModule('audit-logs')` |
| `/admin/activity-logs` | AdminDataTable (generic) | `adminApi.listModule('activity-logs')` |
| `/admin/backup` | AdminBackupPage | `adminApi.createBackup`, `GET /admin/backup/{id}/download` |
| Generic modules | AdminDataTable | `GET /admin/{module}`, `POST /admin/{module}/bulk`, `GET /admin/{module}/export` |

**Admin API files:** `adminApi.ts`, `adminUsersApi.ts`, `adminCatalogApi.ts`, `adminOpsApi.ts`, `adminMarketingApi.ts`, `adminLocationApi.ts`, `adminSettingsApi.ts`, `adminRbacApi.ts`, `adminVerificationApi.ts`, `adminSubscriptionPlansApi.ts`, `adminUserSubscriptionsApi.ts`

**Shared services:** `api.ts`, `subscriptionsApi.ts`, `locationsApi.ts` (public catalog via `useLocationCatalog` hook)

---

## 2. Android (`NavGraph.kt` + `Routes.kt`)

### 2.1 Navigation graph

| Route constant | Screen | Bottom nav |
|----------------|--------|------------|
| `splash` | SplashScreen | — |
| `login` | LoginScreen | — |
| `register` | RegisterScreen | — |
| `forgot-password` | ForgotPasswordScreen | — |
| `reset-password` | ResetPasswordScreen | — |
| `home` | HomeScreen | Yes |
| `services` | ServicesScreen | Yes |
| `booking-wizard` | BookingWizardScreen | — |
| `booking-wizard/{serviceId}` | BookingWizardScreen (preselected) | — |
| `bookings` | MyBookingsScreen | Yes |
| `booking/{bookingId}` | BookingDetailScreen | — |
| `profile` | ProfileScreen | Yes |
| `profile/edit` | ProfileEditScreen | — |
| `notifications` | NotificationsScreen | — |
| `subscriptions` | SubscriptionPlansScreen | — |
| `subscriptions/{planId}` | SubscribeScreen | — |
| `subscription` | MySubscriptionScreen | — |
| `addresses` | AddressesScreen | — |
| `chat` | ChatListScreen | — |
| `booking/{bookingId}/chat` | BookingChatScreen | — |
| `support` | SupportScreen | — |
| `craftsman-portal` | CraftsmanPortalScreen | — |
| `store-portal` | StorePortalScreen | — |

### 2.2 API layer (`ApiService.kt` → repositories)

| Repository | Endpoints covered |
|------------|-------------------|
| AuthRepository | auth/*, users/me, users/me (PUT) |
| ServicesRepository | services/categories, services |
| BookingRepository | bookings/* (full lifecycle + review) |
| NotificationRepository | notifications/* |
| SubscriptionRepository | subscription-plans/*, me/subscription* |
| AddressRepository | users/me/addresses |
| ChatRepository | chat/* |
| SupportRepository | complaints, support-tickets |
| PortalRepository | me/craftsman, me/store |
| PushRepository | devices/push-token |

### 2.3 Offline support indicators

| Capability | Implementation | Scope |
|------------|----------------|-------|
| Auth token persistence | `TokenManager` (EncryptedSharedPreferences) | Session restore |
| User cache | Room `UserEntity` + `UserDao` | Profile display after login |
| Service catalog cache | Room `ServiceCategoryEntity`, `ServiceEntity` | `ServicesRepository.observe*` + `refresh*` |
| Bookings / chat / notifications | Network-only | No local DB |
| Offline queue / sync | **Not implemented** | — |
| Conflict resolution | **Not implemented** | — |

**Partial offline UX:** Services screen can show last-cached categories/services via Room `Flow` while refresh runs.

### 2.4 Web gaps (Android missing)

- `/dashboard`, `/verify-email`, `/sessions`, `/change-password`
- `/pay` standalone checkout
- Home advertisements (`GET /advertisements`)
- Coupon validation on subscribe (`GET /coupons/validate`)
- Locations catalog (`/locations/regions`, `/locations/cities`) for address/profile
- Full admin portal
- `identityApi` `/profile` (uses `users/me` instead)
- Craftsman working-hours CRUD, store product update/delete (API exists on web, partial on Android)

---

## 3. iOS (`ContentView.swift` + feature views)

### 3.1 Navigation structure

```
ContentView
├── AuthFlowView (unauthenticated)
│   ├── LoginView → ForgotPasswordView, ResetPasswordView, RegisterView
└── MainTabView (authenticated)
    ├── HomeView (tab)
    ├── ServicesView → BookingWizardView → BookingDetailView
    ├── MyBookingsView → BookingDetailView
    └── ProfileView → [pushed stack]
        ├── NotificationsView → BookingDetailView
        ├── SubscriptionPlansView → PlanDetail / Subscribe
        ├── MySubscriptionView
        ├── ChatListView → ChatThreadView
        ├── AddressesView
        ├── CraftsmanPortalView
        ├── StorePortalView
        ├── SupportView
        └── ProfileEditView
```

### 3.2 API layer (`APIEndpoints.swift` + ViewModels)

| Feature | ViewModel / Service | Endpoints |
|---------|---------------------|-----------|
| Auth | AuthService | auth/login, register, revoke, forgot-password, reset-password |
| Profile | ProfileViewModel | users/me (GET/PUT) |
| Home | HomeViewModel | services/categories |
| Services | ServicesViewModel | services/categories, services |
| Bookings | BookingViewModel | bookings/* (full lifecycle), review |
| Notifications | NotificationsViewModel | notifications/* |
| Subscriptions | SubscriptionViewModel | subscription-plans/*, me/subscription* |
| Chat | ChatViewModel | chat/* |
| Support | SupportViewModel | complaints, support-tickets |
| Portals | CraftsmanPortalViewModel, StorePortalViewModel | me/craftsman, me/store |
| Addresses | AddressesView (inline) | users/me/addresses |
| Push | PushRegistrationService | devices/push-token |
| Health | — (defined, unused in UI) | health |

### 3.3 Offline support indicators

| Capability | Implementation | Scope |
|------------|----------------|-------|
| Auth tokens | Keychain (`TokenStorage`) | Session restore via `AppSession.restoreSession()` |
| Language preference | UserDefaults | UI locale |
| Push token | UserDefaults (`PushTokenStorage`) | Device registration |
| Service/booking cache | **None** | Always network fetch |
| Core Data / SwiftData | **Not used** | — |
| Offline queue | **Not implemented** | — |

### 3.4 Web gaps (iOS missing)

- `/dashboard`, `/verify-email`, `/sessions`, `/change-password`
- `/pay` standalone checkout
- Home advertisements
- Coupon validation on subscribe (`couponCode` hardcoded `nil` in SubscriptionViewModel)
- Locations catalog for addresses
- Admin portal
- Craftsman working-hours, store product update/delete (partial portal parity)
- `identityApi` `/profile` (uses `users/me`)

---

## 4. Cross-Client Feature Parity Matrix

| Feature | Web | Android | iOS | Notes |
|---------|:---:|:-------:|:---:|-------|
| Home / marketing | ✅ | ✅ | ✅ | Web shows ads; mobile does not |
| Service catalog | ✅ | ✅ | ✅ | Android caches locally |
| Booking wizard | ✅ | ✅ | ✅ | |
| Booking detail actions | ✅ | ✅ | ✅ | Role-based actions |
| Payment flow | ✅ | ✅ | ✅ | Web has dedicated `/pay` route |
| Subscriptions | ✅ | ✅ | ✅ | |
| Coupon on subscribe | ✅ | ❌ | ❌ | |
| My subscription mgmt | ✅ | ✅ | ✅ | |
| Notifications | ✅ | ✅ | ✅ | |
| Chat | ✅ | ✅ | ✅ | |
| Addresses | ✅ | ✅ | ✅ | Web uses region/city catalog |
| Craftsman portal | ✅ | ✅ | ✅ | Web richer (working hours) |
| Store portal | ✅ | ✅ | ✅ | Web full product CRUD |
| Support / tickets | ✅ | ✅ | ✅ | |
| Forgot / reset password | ✅ | ✅ | ✅ | |
| Email verification | ✅ | ❌ | ❌ | Web only |
| Change password | ✅ | ❌ | ❌ | Web only |
| Session management | ✅ | ❌ | ❌ | Web only |
| User dashboard | ✅ | ❌ | ❌ | Web only |
| Admin console | ✅ | ❌ | ❌ | Web only |
| Push notifications | — | ✅ | ✅ | |
| Offline catalog browse | ❌ | ⚠️ | ❌ | Android partial |
| i18n parity (ar/en) | ✅ | ✅ | ✅ | 100% key parity each platform |

**Legend:** ✅ Full · ⚠️ Partial · ❌ Missing · — N/A

---

## 5. i18n Coverage (Web)

| File | Keys | Parity |
|------|------|--------|
| `src/i18n/locales/en.json` | 276 | — |
| `src/i18n/locales/ar.json` | 276 | **100% matched** |

**Gaps (non-key):**
- Admin module pages use mixed EN/AR (`title` / `titleAr` in `moduleConfig.ts`, `AdminLayout` nav sections)
- `useLocationCatalog` error string hardcoded English: `"Failed to load regions"`
- Some admin form labels hardcoded English (e.g. coupon/advertisement pages)

---

## 6. Service / API File Index

### Web `src/services/`

| File | Exports |
|------|---------|
| `api.ts` | `authApi`, `identityApi`, `servicesApi`, `usersApi`, `bookingsApi`, `notificationsApi`, `chatApi`, `craftsmanApi`, `storeApi`, `supportApi` |
| `subscriptionsApi.ts` | `subscriptionPlansApi`, `userSubscriptionApi` |
| `locationsApi.ts` | `locationsApi` |

### Android

| File | Role |
|------|------|
| `data/remote/ApiService.kt` | Retrofit interface (all mobile endpoints) |
| `data/repository/*.kt` | Per-domain wrappers |

### iOS

| File | Role |
|------|------|
| `Core/Network/APIEndpoints.swift` | URL builders |
| `Core/Network/APIClient.swift` | HTTP + token refresh |
| `Core/Network/AuthService.swift` | Auth operations |
| `Features/*/*ViewModel.swift` | Per-feature API calls |
