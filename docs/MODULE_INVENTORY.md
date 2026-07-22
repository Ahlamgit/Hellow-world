# KHADAMATI — Module Inventory (Phase 0)

**Document version:** 1.0  
**Date:** 2026-07-22

---

## 1. Solution Structure

```
workspace/
├── src/backend/          # .NET 8 API (Clean Architecture)
│   ├── Khadamati.API/
│   ├── Khadamati.Application/
│   ├── Khadamati.Domain/
│   ├── Khadamati.Infrastructure/
│   ├── Khadamati.Tests/
│   └── Khadamati.IntegrationTests/
├── src/web/              # React 19 + Vite + MUI 9
├── src/android/          # Kotlin + Jetpack Compose
├── src/ios/              # SwiftUI
├── src/database/         # Legacy SQL scripts (see DATABASE_BASELINE.md)
├── docs/                 # Documentation
├── scripts/              # Dev helper scripts (Windows)
└── docker-compose*.yml   # Container orchestration
```

---

## 2. Backend Modules

### 2.1 Layer Responsibilities

| Layer | Project | Responsibility |
|-------|---------|----------------|
| API | `Khadamati.API` | Controllers, middleware, DI bootstrap |
| Application | `Khadamati.Application` | Commands/queries (MediatR), DTOs, validators (FluentValidation), interfaces |
| Domain | `Khadamati.Domain` | Entities, enums, constants, domain rules |
| Infrastructure | `Khadamati.Infrastructure` | EF Core, services, external providers |

### 2.2 Application Feature Areas

| Module | Key Services / Handlers |
|--------|-------------------------|
| **Auth** | `AuthService`, `SecurityService`, register/login/reset flows |
| **Identity** | `IdentityService`, sessions, login history |
| **Users** | `UserManagementService`, profile, addresses |
| **Catalog** | Service/category queries, admin catalog |
| **Bookings** | `BookingService` (monolithic), slot reservation, status workflow |
| **Subscriptions** | `SubscriptionPlanService`, `UserSubscriptionService` |
| **Payments** | `PaymentWebhookService`, Moyasar provider |
| **Messaging** | Chat, notifications, push token management |
| **Support** | Complaints, tickets, reviews |
| **Verification** | `VerificationDocumentService` |
| **Admin** | `AdminService`, `AdminOperationsService`, analytics, backup |
| **Integrations** | `IntegrationReadinessService`, email/SMS/payment/push providers |

### 2.3 External Provider Abstractions

| Category | Dev Provider | Production Provider (configurable) |
|----------|--------------|-----------------------------------|
| Email | `Development` (logs links) | SendGrid |
| SMS | `Development` | Twilio |
| Payment | `Development` | Moyasar |
| Push | `Development` | Firebase (FCM) + APNS |

---

## 3. Web Application

### 3.1 Technology Stack

| Dependency | Version | Purpose |
|------------|---------|---------|
| React | 19.2 | UI framework |
| Vite | 8.1 | Build tool / dev server |
| MUI | 9.2 | Component library |
| React Router | 7.18 | Client routing |
| Axios | 1.18 | HTTP client |
| i18next | 26.3 | Arabic/English i18n |

### 3.2 Public Routes

| Path | Page Component | Feature |
|------|----------------|---------|
| `/` | `HomePage` | Landing, advertisements |
| `/services` | `ServicesPage` | Service catalog |
| `/login` | `LoginPage` | Authentication |
| `/register` | `RegisterPage` | Registration + confirm password |
| `/forgot-password` | `ForgotPasswordPage` | Password recovery |
| `/reset-password` | `ResetPasswordPage` | Reset + confirm password |
| `/verify-email` | `VerifyEmailPage` | Email verification |
| `/pay` | `PaymentCheckoutPage` | Payment checkout redirect |

### 3.3 Authenticated Customer Routes

| Path | Page Component | Feature |
|------|----------------|---------|
| `/dashboard` | `DashboardPage` | User dashboard |
| `/profile` | `ProfilePage` | Profile + inline change password |
| `/change-password` | `ChangePasswordPage` | Dedicated change password |
| `/sessions` | `SessionsPage` | Active sessions management |
| `/bookings` | `MyBookingsPage` | Booking list |
| `/bookings/new` | `BookingWizardPage` | Multi-step booking |
| `/bookings/:id` | `BookingDetailPage` | Booking detail + actions |
| `/bookings/:id/payment` | `BookingPaymentPage` | Payment initiation |
| `/subscriptions` | `SubscriptionPlansPage` | Plan browsing |
| `/subscriptions/:planId` | `SubscribePage` | Subscribe flow |
| `/subscription` | `MySubscriptionPage` | Current subscription |
| `/notifications` | `NotificationsPage` | Notification inbox |
| `/chat` | `ChatListPage` | Conversation list |
| `/chat/:bookingId` | `BookingChatPage` | Booking chat |
| `/addresses` | `AddressesPage` | Address CRUD |
| `/craftsman` | `CraftsmanPortalPage` | Craftsman self-service |
| `/store` | `StorePortalPage` | Store self-service |
| `/support` | `SupportPage` | Complaints + tickets |

### 3.4 Admin Portal Routes

Base: `/admin/*` — requires `AdminRoute` + per-page `AdminPageGuard`

| Path | Page | API Module Key |
|------|------|----------------|
| `/admin` | `AdminDashboardPage` | dashboard |
| `/admin/analytics` | `AdminAnalyticsPage` | analytics |
| `/admin/reports` | `AdminReportsPage` | reports |
| `/admin/system-health` | `AdminSystemHealthPage` | system-health |
| `/admin/users` | `AdminUsersPage` | users |
| `/admin/customers` | `AdminUsersPage` (Customer filter) | customers |
| `/admin/craftsmen` | `AdminUsersPage` (Craftsman filter) | craftsmen |
| `/admin/stores` | `AdminUsersPage` (StoreOwner filter) | stores |
| `/admin/roles` | `AdminRbacMatrixPage` | roles |
| `/admin/permissions` | `AdminDataTable` | permissions |
| `/admin/categories` | `AdminCategoriesPage` | categories |
| `/admin/services` | `AdminServicesPage` | services |
| `/admin/bookings` | `AdminBookingsPage` | bookings |
| `/admin/subscriptions` | `AdminSubscriptionPlansPage` | subscriptions |
| `/admin/user-subscriptions` | `AdminUserSubscriptionsPage` | user-subscriptions |
| `/admin/advertisements` | `AdminAdvertisementsPage` | advertisements |
| `/admin/coupons` | `AdminCouponsPage` | coupons |
| `/admin/notifications` | `AdminDataTable` | notifications |
| `/admin/payments` | `AdminPaymentsPage` | payments |
| `/admin/complaints` | `AdminComplaintsPage` | complaints |
| `/admin/verification-documents` | `AdminVerificationDocumentsPage` | verification-documents |
| `/admin/support-tickets` | `AdminSupportTicketsPage` | support-tickets |
| `/admin/cities` | `AdminCitiesPage` | cities |
| `/admin/regions` | `AdminRegionsPage` | regions |
| `/admin/settings` | `AdminSettingsPage` | settings |
| `/admin/audit-logs` | `AdminDataTable` | audit-logs |
| `/admin/activity-logs` | `AdminDataTable` | activity-logs |
| `/admin/backup` | `AdminBackupPage` | backup |
| `/admin/restore` | `AdminRestorePage` | restore |

### 3.5 Shared Web Components (Key)

| Component | Purpose |
|-----------|---------|
| `Navbar` | Main navigation |
| `ProtectedRoute` / `PublicRoute` | Auth guards |
| `PasswordConfirmFields` | Confirm password UI |
| `ChangePasswordForm` | Reusable change password |
| `AdminLayout` | Admin sidebar navigation |
| `AdminDataTable` | Generic admin list/bulk/export |

---

## 4. Android Application

### 4.1 Technology

- Kotlin, Jetpack Compose, Navigation Compose
- Package: `com.khadamati.app`
- Firebase placeholder in `google-services.json`

### 4.2 Screens & Routes

| Route constant | Screen | Feature |
|----------------|--------|---------|
| `SPLASH` | `SplashScreen` | App launch |
| `LOGIN` | `LoginScreen` | Login |
| `REGISTER` | `RegisterScreen` | Register + confirm password |
| `FORGOT_PASSWORD` | `ForgotPasswordScreen` | Forgot password |
| `RESET_PASSWORD` | `ResetPasswordScreen` | Reset password |
| `HOME` | `HomeScreen` | Home |
| `SERVICES` | `ServicesScreen` | Catalog |
| `BOOKING_WIZARD` | `BookingWizardScreen` | Booking flow |
| `BOOKING_WIZARD_WITH_SERVICE` | `BookingWizardScreen` | Deep link with service |
| `BOOKINGS` | `BookingsScreen` | My bookings |
| `BOOKING_DETAIL` | `BookingDetailScreen` | Booking detail |
| `PROFILE` | `ProfileScreen` | Profile hub |
| `PROFILE_EDIT` | `ProfileEditScreen` | Edit profile |
| `ADDRESSES` | `AddressesScreen` | Addresses |
| `CRAFTSMAN_PORTAL` | `PortalScreens` | Craftsman portal |
| `STORE_PORTAL` | `PortalScreens` | Store portal |
| `NOTIFICATIONS` | `NotificationsScreen` | Notifications |
| `SUBSCRIPTION_PLANS` | `SubscriptionScreens` | Plans |
| `SUBSCRIBE` | `SubscriptionScreens` | Subscribe |
| `MY_SUBSCRIPTION` | `SubscriptionScreens` | My subscription |
| `CHAT_LIST` | `ChatScreens` | Chat list |
| `booking/{id}/chat` | `ChatScreens` | Booking chat |
| `SUPPORT` | `SupportScreens` | Support |

**Gaps vs Web:** No dedicated change-password screen; no admin app.

---

## 5. iOS Application

### 5.1 Technology

- SwiftUI, `NavigationStack`, `TabView`
- Bundle: `com.khadamati.app`

### 5.2 Navigation Structure

**Auth flow (unauthenticated):**
- `LoginView` ↔ `RegisterView` (confirm password)
- `PasswordRecoveryView` (forgot/reset)

**Main tabs (authenticated):**
| Tab | View | Feature |
|-----|------|---------|
| Home | `HomeView` | Landing |
| Services | `ServicesView` | Catalog |
| Bookings | `BookingsView` / `BookingDetailView` / `BookingWizardView` | Booking |
| Profile | `ProfileView` | Profile hub |

**Profile sub-flows:**
- `ProfileEditView`
- `AddressesView`
- `CraftsmanPortalView`
- `StorePortalView`
- `NotificationsView`
- `SubscriptionsView`
- `ChatView`
- `SupportView`

**Gaps vs Web:** No dedicated change-password screen; no admin app.

---

## 6. Test Modules

### 6.1 Unit Tests (`Khadamati.Tests`)

| Area | Test Files | ~Facts |
|------|------------|--------|
| Validators | `AuthValidatorTests`, `BookingValidatorTests`, `AdminValidatorTests`, `AdminUserValidatorTests`, `SubscriptionPlanValidatorTests` | 26 |
| Auth/Security | `AuthServiceTests`, `SecurityServiceTests`, `IdentityServiceTests` | 16 |
| Admin | `AdminServiceTests`, `AdminOperationsServiceTests`, `UserManagementServiceTests` | 14 |
| Subscriptions | `SubscriptionPlanServiceTests`, `UserSubscriptionServiceTests` | 7 |
| Other | `VerificationDocumentServiceTests`, `PaymentWebhookServiceTests`, `IntegrationReadinessServiceTests`, `DatabaseBackupServiceTests` | 10 |

**Total:** ~73 unit tests. **Not covered:** `BookingService` (largest service).

### 6.2 Integration Tests (`Khadamati.IntegrationTests`)

| Test | Status |
|------|--------|
| `Health_ReturnsHealthy` | ✅ |
| `HealthReady_ReturnsReadyWithDatabase` | ✅ |
| `AdminDashboard_WithoutAuth_ReturnsUnauthorized` | ✅ |
| `Login_WithSeededAdmin_ReturnsTokenWithPermissions` | ✅ |
| `AdminDashboard_WithAdminToken_ReturnsOk` | ✅ |
| `AdminAnalytics_WithoutReportsPermission_ReturnsForbidden` | ✅ |
| `NearbyCraftsmen_WithCoordinates_ReturnsOrderedResults` | ⚠️ Failing |

### 6.3 Frontend / Mobile Tests

| Platform | Status |
|----------|--------|
| Web (E2E) | ❌ None |
| Web (unit) | ❌ None |
| Android | ❌ None |
| iOS | ❌ None |

---

## 7. Infrastructure & Scripts

| Asset | Purpose |
|-------|---------|
| `docker-compose.yml` | Dev: SQL Server + API + web |
| `docker-compose.staging.yml` | Staging stack |
| `docker-compose.prod.yml` | Production stack |
| `scripts/start-api-windows.ps1` | Start API on Windows |
| `scripts/start-web-windows.ps1` | Start Vite on Windows |
| `scripts/stop-web-windows.ps1` | Kill stuck Node/Vite processes |

---

## 8. Documentation Modules (Existing)

| Doc | Topic |
|-----|-------|
| `ARCHITECTURE.md` | High-level architecture |
| `PLATFORM_ARCHITECTURE.md` | Platform overview |
| `AUTHENTICATION.md` | Auth module |
| `BOOKING.md` | Booking module |
| `SUBSCRIPTION_MANAGEMENT.md` | Subscriptions |
| `USER_MANAGEMENT.md` | Users |
| `ADMIN_DASHBOARD.md` | Admin |
| `DATABASE.md` | Legacy DB doc |
| `LOCAL_DEV_WINDOWS.md` | Windows dev setup |
| `STAGING.md` / `PRODUCTION.md` | Deploy guides |

---

## 9. Feature Parity Matrix (Web vs Mobile)

| Feature | Web | Android | iOS |
|---------|-----|---------|-----|
| Register + confirm password | ✅ | ✅ | ✅ |
| Login | ✅ | ✅ | ✅ |
| Forgot/reset password | ✅ | ✅ | ✅ |
| Change password | ✅ | ❌ | ❌ |
| Email verify | ✅ | — | — |
| Sessions management | ✅ | — | — |
| Service catalog | ✅ | ✅ | ✅ |
| Booking wizard | ✅ | ✅ | ✅ |
| Booking management | ✅ | ✅ | ✅ |
| Payments | ✅ | — | — |
| Subscriptions | ✅ | ✅ | ✅ |
| Notifications | ✅ | ✅ | ✅ |
| Chat | ✅ | ✅ | ✅ |
| Addresses | ✅ | ✅ | ✅ |
| Craftsman portal | ✅ | ✅ | ✅ |
| Store portal | ✅ | ✅ | ✅ |
| Support | ✅ | ✅ | ✅ |
| Admin portal | ✅ | — | — |

---

## 10. Related Documents

- [BUSINESS_REQUIREMENTS.md](./BUSINESS_REQUIREMENTS.md)
- [TRACEABILITY_MATRIX.md](./TRACEABILITY_MATRIX.md)
- [API_INVENTORY.md](./API_INVENTORY.md)
- [DEPLOYMENT_RUNBOOK.md](./DEPLOYMENT_RUNBOOK.md)
