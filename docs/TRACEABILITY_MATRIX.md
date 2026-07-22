# KHADAMATI — Traceability Matrix (Phase 0)

**Document version:** 1.0  
**Date:** 2026-07-22  
**Legend:** ✅ Implemented · ⚠️ Partial · ❌ Missing · — Not applicable

---

## 1. Traceability Model

```
Requirement (REQ-xxx)
    ↓
Backend (Controller / Service / Handler)
    ↓
Database (EF Table / Entity)
    ↓
Web (Route / Page)
    ↓
Mobile (Android / iOS Screen)
    ↓
Tests (Unit / Integration)
```

---

## 2. Identity & Authentication

| Requirement | Backend | Database | Web | Android | iOS | Tests |
|-------------|---------|----------|-----|---------|-----|-------|
| REQ-AUTH-001 Register | `AuthController`, `AuthService`, `RegisterCommand` | `Users`, `UserProfiles`, `RefreshTokens` | `/register` `RegisterPage` | `RegisterScreen` | `RegisterView` | `AuthValidatorTests`, `AuthServiceTests` |
| REQ-AUTH-002 Login | `AuthController`, `AuthService` | `Users`, `RefreshTokens`, `LoginHistory` | `/login` `LoginPage` | `LoginScreen` | `LoginView` | `AuthServiceTests`, `ApiIntegrationTests.Login_*` |
| REQ-AUTH-003 Refresh/Revoke | `AuthController` | `RefreshTokens` | `api.ts` interceptors | Auth repository | `AuthViewModel` | `AuthServiceTests` |
| REQ-AUTH-004 Forgot password | `AuthController` | `PasswordResetTokens` | `/forgot-password` | `ForgotPasswordScreen` | `PasswordRecoveryView` | `AuthValidatorTests` |
| REQ-AUTH-005 Reset password | `AuthController` | `PasswordResetTokens`, `PasswordHistory` | `/reset-password` | `ResetPasswordScreen` | `PasswordRecoveryView` | `AuthValidatorTests` |
| REQ-AUTH-006 Change password | `AuthController` | `PasswordHistory`, `RefreshTokens` | `/change-password`, `ProfilePage` | ❌ | ❌ | `AuthValidatorTests` |
| REQ-AUTH-007 Email verify | `AuthController` | `EmailVerificationTokens` | `/verify-email` | — | — | `AuthServiceTests` |
| REQ-AUTH-008 Phone OTP | `AuthController` | `PhoneOtpTokens` | — | — | — | `SecurityServiceTests` |
| REQ-AUTH-009 Lockout | `AuthService` | `Users` (failed attempts) | — | — | — | `SecurityServiceTests` |
| REQ-AUTH-010 Password policy | `AuthValidators`, `SecurityService` | `PasswordHistory` | Form validation | Register validation | Register validation | `AuthValidatorTests`, `SecurityServiceTests` |
| REQ-AUTH-011 Admin verify email | `AuthController` | `Users` | Admin user actions | — | — | — |

---

## 3. User Profile & Addresses

| Requirement | Backend | Database | Web | Android | iOS | Tests |
|-------------|---------|----------|-----|---------|-----|-------|
| REQ-USER-001 Profile | `ProfileController`, `UsersController` | `Users`, `UserProfiles` | `/profile` `ProfilePage` | `ProfileScreen`, `ProfileEditScreen` | `ProfileView`, `ProfileEditView` | `IdentityServiceTests` |
| REQ-USER-002 Addresses | `UsersController` | `Addresses` | `/addresses` `AddressesPage` | `AddressesScreen` | `AddressesView` | — |
| REQ-USER-003 Login history | `LoginHistoryController` | `LoginHistory` | — | — | — | `IdentityServiceTests` |
| REQ-USER-004 Sessions | `SessionsController`, `AuthController` | `RefreshTokens` | `/sessions` `SessionsPage` | — | — | `IdentityServiceTests` |

---

## 4. Service Catalog

| Requirement | Backend | Database | Web | Android | iOS | Tests |
|-------------|---------|----------|-----|---------|-----|-------|
| REQ-CAT-001 Categories | `ServicesController` | `ServiceCategories` | `/services` `ServicesPage` | `ServicesScreen` | `ServicesView` | — |
| REQ-CAT-002 Services list | `ServicesController` | `Services` | `/services` | `ServicesScreen` | `ServicesView` | `ApiIntegrationTests` (nearby uses services) |
| REQ-CAT-003 Admin categories | `AdminCatalogController` | `ServiceCategories` | `/admin/categories` | — | — | `AdminServiceTests` |
| REQ-CAT-004 Admin services | `AdminCatalogController` | `Services` | `/admin/services` | — | — | `AdminServiceTests` |

---

## 5. Booking & Scheduling

| Requirement | Backend | Database | Web | Android | iOS | Tests |
|-------------|---------|----------|-----|---------|-----|-------|
| REQ-BOOK-001 Search craftsmen | `BookingsController` | `CraftsmanProfiles`, `CraftsmanServices` | `BookingWizardPage` | `BookingWizardScreen` | `BookingWizardView` | — |
| REQ-BOOK-002 Nearby craftsmen | `BookingsController` | `CraftsmanProfiles`, `Addresses` | `BookingWizardPage` | `BookingWizardScreen` | `BookingWizardView` | `ApiIntegrationTests.NearbyCraftsmen_*` ⚠️ |
| REQ-BOOK-003 Availability | `BookingsController` | `CraftsmanWorkingHours`, `BookingSlotReservations` | `BookingWizardPage` | `BookingWizardScreen` | `BookingWizardView` | `BookingValidatorTests` |
| REQ-BOOK-004 Create booking | `BookingsController`, `BookingService` | `ServiceRequests`, `ServiceRequestStatusHistories` | `/bookings/new` | `BookingWizardScreen` | `BookingWizardView` | `BookingValidatorTests` |
| REQ-BOOK-005 List/detail | `BookingsController` | `ServiceRequests` | `/bookings`, `/bookings/:id` | `BookingsScreen`, `BookingDetailScreen` | `BookingsView`, `BookingDetailView` | — |
| REQ-BOOK-006 Accept/reject | `BookingsController` | `ServiceRequests` | `BookingDetailPage`, `CraftsmanPortalPage` | `PortalScreens` | `CraftsmanPortalView` | — |
| REQ-BOOK-007 Cancel | `BookingsController` | `ServiceRequests` | `BookingDetailPage` | `BookingDetailScreen` | `BookingDetailView` | — |
| REQ-BOOK-008 Complete/no-show | `BookingsController` | `ServiceRequests` | `CraftsmanPortalPage` | `PortalScreens` | `CraftsmanPortalView` | — |
| REQ-BOOK-009 Reschedule | `BookingsController` | `ServiceRequests` | `BookingDetailPage` | — | — | — |
| REQ-BOOK-010 Confirm | `BookingsController` | `ServiceRequests` | `BookingDetailPage` | — | — | — |
| REQ-BOOK-011 Payment | `BookingsController` | `BookingPayments` | `/bookings/:id/payment`, `/pay` | — | — | — |
| REQ-BOOK-012 Payment confirm | `BookingsController`, webhook | `BookingPayments` | `PaymentCheckoutPage` | — | — | `PaymentWebhookServiceTests` |
| REQ-BOOK-013 Review | `SupportController` | `ServiceRequests` | `BookingDetailPage` | — | — | — |
| REQ-BOOK-014 Admin bookings | `AdminBookingsController` | `ServiceRequests` | `/admin/bookings` | — | — | `AdminServiceTests` |

---

## 6. Craftsman & Store Portals

| Requirement | Backend | Database | Web | Android | iOS | Tests |
|-------------|---------|----------|-----|---------|-----|-------|
| REQ-CRAFT-001 Profile | `MeCraftsmanController` | `CraftsmanProfiles` | `/craftsman` | `PortalScreens` | `CraftsmanPortalView` | — |
| REQ-CRAFT-002 Services link | `MeCraftsmanController` | `CraftsmanServices` | `/craftsman` | `PortalScreens` | `CraftsmanPortalView` | — |
| REQ-CRAFT-003 Working hours | `MeCraftsmanController` | `CraftsmanWorkingHours` | `/craftsman` | `PortalScreens` | `CraftsmanPortalView` | — |
| REQ-STORE-001 Store profile | `MeStoreController` | `StoreProfiles` | `/store` | `PortalScreens` | `StorePortalView` | — |
| REQ-STORE-002 Products | `MeStoreController` | `StoreProducts` | `/store` | `PortalScreens` | `StorePortalView` | — |

---

## 7. Subscriptions

| Requirement | Backend | Database | Web | Android | iOS | Tests |
|-------------|---------|----------|-----|---------|-----|-------|
| REQ-SUB-001 Plan listing | `SubscriptionPlansController` | `SubscriptionPlans`, `PlanBillingOptions` | `/subscriptions` | `SubscriptionScreens` | `SubscriptionsView` | `SubscriptionPlanServiceTests` |
| REQ-SUB-002 Subscribe | `MeSubscriptionController` | `UserSubscriptions` | `/subscriptions/:planId` | `SubscriptionScreens` | `SubscriptionsView` | `UserSubscriptionServiceTests` |
| REQ-SUB-003 My subscription | `MeSubscriptionController` | `UserSubscriptions` | `/subscription` | `MY_SUBSCRIPTION` route | `SubscriptionsView` | `UserSubscriptionServiceTests` |
| REQ-SUB-004 Cancel | `MeSubscriptionController` | `UserSubscriptions` | `/subscription` | `SubscriptionScreens` | `SubscriptionsView` | `UserSubscriptionServiceTests` |
| REQ-SUB-005 Auto-renew | `MeSubscriptionController` | `UserSubscriptions` | `/subscription` | — | — | — |
| REQ-SUB-006 Admin plans | `AdminSubscriptionPlansController` | `SubscriptionPlans` | `/admin/subscriptions` | — | — | `SubscriptionPlanServiceTests`, `SubscriptionPlanValidatorTests` |
| REQ-SUB-007 Admin user subs | `AdminUserSubscriptionsController` | `UserSubscriptions` | `/admin/user-subscriptions` | — | — | `UserSubscriptionServiceTests` |

---

## 8. Payments, Coupons & Marketing

| Requirement | Backend | Database | Web | Android | iOS | Tests |
|-------------|---------|----------|-----|---------|-----|-------|
| REQ-PAY-001 Booking payment | `BookingsController` | `BookingPayments` | `BookingPaymentPage` | — | — | — |
| REQ-PAY-002 Moyasar webhook | `AdminLocationsController` (webhook route) | `BookingPayments` | — | — | — | `PaymentWebhookServiceTests` |
| REQ-PAY-003 Validate coupon | `SupportController` | `Coupons` | `BookingWizardPage` | — | — | — |
| REQ-PAY-004 Admin coupons | `AdminMarketingController` | `Coupons` | `/admin/coupons` | — | — | `AdminServiceTests` |
| REQ-PAY-005 Admin payments | `AdminPaymentsController` | `BookingPayments` | `/admin/payments` | — | — | `AdminOperationsServiceTests` |
| REQ-MKT-001 Public ads | `SupportController` | `Advertisements` | `HomePage` | `HomeScreen` | `HomeView` | — |
| REQ-MKT-002 Admin ads | `AdminMarketingController` | `Advertisements` | `/admin/advertisements` | — | — | `AdminServiceTests` |

---

## 9. Messaging, Support & Verification

| Requirement | Backend | Database | Web | Android | iOS | Tests |
|-------------|---------|----------|-----|---------|-----|-------|
| REQ-MSG-001 Push tokens | `DevicesController` | `DevicePushTokens` | — | FCM integration | APNs scaffold | — |
| REQ-MSG-002 Notifications | `NotificationsController` | `Notifications` | `/notifications` | `NotificationsScreen` | `NotificationsView` | — |
| REQ-MSG-003 Chat | `ChatController` | `ChatConversations`, `ChatMessages` | `/chat`, `/chat/:bookingId` | `ChatScreens` | `ChatView` | — |
| REQ-MSG-004 Admin notifications | `AdminController` | `Notifications` | `/admin/notifications` | — | — | `AdminServiceTests` |
| REQ-SUP-001 Complaints | `SupportController` | `Complaints` | `/support` | `SupportScreens` | `SupportView` | — |
| REQ-SUP-002 Support tickets | `SupportController` | `SupportTickets` | `/support` | `SupportScreens` | `SupportView` | — |
| REQ-SUP-003 View mine | `SupportController` | `Complaints`, `SupportTickets` | `/support` | `SupportScreens` | `SupportView` | — |
| REQ-SUP-004 Resolve complaints | `AdminComplaintsController` | `Complaints` | `/admin/complaints` | — | — | `AdminOperationsServiceTests` |
| REQ-SUP-005 Close tickets | `AdminSupportTicketsController` | `SupportTickets` | `/admin/support-tickets` | — | — | `AdminOperationsServiceTests` |
| REQ-VER-001 Upload docs | `VerificationController` | `VerificationDocuments` | — | — | — | `VerificationDocumentServiceTests` |
| REQ-VER-002 Admin review | `AdminVerificationDocumentsController` | `VerificationDocuments` | `/admin/verification-documents` | — | — | `VerificationDocumentServiceTests` |

---

## 10. Locations & Administration

| Requirement | Backend | Database | Web | Android | iOS | Tests |
|-------------|---------|----------|-----|---------|-----|-------|
| REQ-LOC-001 Public regions/cities | `LocationsController` | `Regions`, `Cities` | Register/booking forms | Register | Register | — |
| REQ-LOC-002 Admin CRUD | `AdminLocationsController` | `Regions`, `Cities` | `/admin/regions`, `/admin/cities` | — | — | `AdminServiceTests` |
| REQ-LOC-003 Seed locations | `AdminLocationsController` POST seed | `Regions`, `Cities` | — | — | — | — |
| REQ-ADM-001 Dashboard | `AdminController` | Multiple aggregates | `/admin` | — | — | `ApiIntegrationTests.AdminDashboard_*`, `AdminServiceTests` |
| REQ-ADM-002 Generic modules | `AdminController` | Per module | `AdminDataTable` | — | — | `AdminServiceTests` |
| REQ-ADM-003 User mgmt | `AdminUsersController` | `Users`, `UserRoles`, `UserPermissions` | `/admin/users` (+ role filters) | — | — | `UserManagementServiceTests`, `AdminUserValidatorTests` |
| REQ-ADM-004 RBAC | `AdminRbacController` | `Roles`, `Permissions`, `RolePermissions` | `/admin/roles`, `/admin/permissions` | — | — | `AdminServiceTests` |
| REQ-ADM-005 Settings | `AdminSettingsController` | `SystemSettings` | `/admin/settings` | — | — | `AdminServiceTests` |
| REQ-ADM-006 Analytics/reports | `AdminController` | Aggregates | `/admin/analytics`, `/admin/reports` | — | — | `AdminServiceTests` |
| REQ-ADM-007 System health | `AdminController`, `HealthController` | — | `/admin/system-health` | — | — | `IntegrationReadinessServiceTests`, `ApiIntegrationTests.Health_*` |
| REQ-ADM-008 Backup/restore | `AdminController` | `BackupJobs` | `/admin/backup`, `/admin/restore` | — | — | `DatabaseBackupServiceTests` |
| REQ-ADM-009 Audit/activity | `AdminController` | `AuditLogs`, `ActivityLogs` | `/admin/audit-logs`, `/admin/activity-logs` | — | — | — |
| REQ-ADM-010 Permission gates | `HasPermissionAttribute` | `Permissions` | `AdminPageGuard`, `permissions.ts` | — | — | `ApiIntegrationTests.AdminAnalytics_*` |

---

## 11. Platform Operations

| Requirement | Backend | Database | Web | Android | iOS | Tests |
|-------------|---------|----------|-----|---------|-----|-------|
| REQ-OPS-001 Health | `HealthController` | Connection check | — | — | — | `ApiIntegrationTests.Health_*` |
| REQ-OPS-002 Integrations | `HealthController` | — | Admin system health | — | — | `IntegrationReadinessServiceTests` |
| REQ-OPS-003 Rate limiting | `Program.cs`, AspNetCoreRateLimit | — | — | — | — | — |
| REQ-OPS-004 CORS | `Program.cs` | — | `vite.config.ts` proxy / direct API | — | — | — |
| REQ-OPS-005 i18n | — | — | `i18next` ar/en | strings.xml | `L10n` | — |
| REQ-OPS-006 Migrations/seed | `Program.cs`, `DatabaseSeeder` | All tables | — | — | — | Integration test factory |
| REQ-OPS-007 Soft delete | `ApplicationDbContext` filters | `IsDeleted` on entities | — | — | — | — |

---

## 12. Coverage Summary

| Layer | Coverage | Gaps |
|-------|----------|------|
| **Backend services** | 14 service test classes | `BookingService` has no dedicated unit tests |
| **Validators** | Auth, Booking, Admin, Subscription | — |
| **Integration** | 6 API tests | `NearbyCraftsmen_*` fails intermittently |
| **Web E2E** | None | No Playwright/Cypress suite |
| **Mobile** | None | No instrumented/UI tests |

---

## 13. Related Documents

- [BUSINESS_REQUIREMENTS.md](./BUSINESS_REQUIREMENTS.md)
- [API_INVENTORY.md](./API_INVENTORY.md)
- [MODULE_INVENTORY.md](./MODULE_INVENTORY.md)
- [DATABASE_BASELINE.md](./DATABASE_BASELINE.md)
