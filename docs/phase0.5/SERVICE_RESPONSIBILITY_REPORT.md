# Service Responsibility Report — Phase 0.5 Discovery

**Generated:** 2026-07-22  
**Location:** `docs/phase0.5/`  
**Source:** `Khadamati.Infrastructure/Services/`  
**Total service LOC:** ~7,362 (34 `.cs` files)

---

## Executive Summary

| Service | LOC | Responsibility breadth | Risk |
|---------|-----|------------------------|------|
| **AdminService** | 1,455 | Dashboard, generic admin modules, catalog, locations, marketing, ops, RBAC, settings, backups | **High** — monolith |
| **BookingService** | 653 | Full booking lifecycle + payments + notifications | Medium |
| **AuthService** | 542 | Registration, login, tokens, password, email/phone verify | Medium |
| **UserManagementService** | 401 | Admin user CRUD, suspend, roles, permission overrides | Low |
| **SubscriptionPlanService** | 318 | Plan CRUD, clone, lifecycle (activate/deactivate/suspend/archive) | Low |
| **UserSubscriptionService** | 256 | User subscribe/cancel/auto-renew + admin grant/cancel | Low |
| **CraftsmanPortalService** | 227 | Craftsman profile, services, working hours | Low |
| **ChatService** | 236 | Booking-scoped chat conversations and messages | Low |
| **PaymentWebhookService** | 86 | Moyasar webhook signature + payment confirmation | Low |

---

## Priority Services (Detailed)

### AdminService — `Infrastructure/Services/AdminService.cs` (1,455 LOC)

**Interface:** `IAdminService`  
**MediatR consumers:** All handlers in `Features/Admin/Commands/AdminCommands.cs` + `DownloadAdminBackupQuery`

| Public method | Domain area |
|---------------|-------------|
| `GetDashboardAsync` | Administration — KPIs |
| `ListModuleAsync` | Administration — generic module listing |
| `BulkActionAsync` | Administration — bulk operations |
| `ExportAsync` | Administration — Excel/PDF export |
| `GetAnalyticsAsync` | Administration — charts |
| `GetReportsAsync` / `GenerateReportAsync` | Administration — reports |
| `GetSystemHealthAsync` | Administration — health |
| `ListBackupsAsync` / `CreateBackupAsync` / `RestoreBackupAsync` / `GetBackupJobAsync` | Administration — backup |
| `ListSettingsAsync` / `UpdateSettingAsync` | Administration — settings |
| `ListRbacRolesAsync` / `GetRolePermissionMatrixAsync` / `UpdateRolePermissionsAsync` | Administration — RBAC |
| `ListCategoriesAsync` … `DeleteCategoryAsync` (5) | Services catalog |
| `ListServicesAsync` … `DeleteServiceAsync` (5) | Services catalog |
| `ListRegionsAsync` … `DeleteRegionAsync` (5) | Locations |
| `ListCitiesAsync` … `DeleteCityAsync` (5) | Locations |
| `ListCouponsAdminAsync` … `DeleteCouponAsync` (5) | Marketing |
| `ListAdvertisementsAdminAsync` … `DeleteAdvertisementAsync` (5) | Marketing |
| `GetComplaintDetailAsync` / `ResolveComplaintAsync` | Support (admin) |
| `GetSupportTicketDetailAsync` / `CloseSupportTicketAsync` | Support (admin) |
| `GetPaymentDetailAsync` | Payments (admin read) |

**Dependencies:** `ApplicationDbContext`, `AdminExportService`, `IDatabaseBackupService`, `IConfiguration`  
**Recommendation:** Split into `AdminDashboardService`, `AdminCatalogService`, `AdminLocationService`, `AdminMarketingService`, `AdminOperationsService`.

---

### BookingService — `Infrastructure/Services/BookingService.cs` (653 LOC)

**Interface:** `IBookingService`  
**MediatR consumers:** All handlers in `Features/Bookings/Commands/BookingCommands.cs`

| Public method | Responsibility |
|---------------|----------------|
| `GetCraftsmenForServiceAsync` | Provider discovery |
| `GetNearbyCraftsmenForServiceAsync` | Geo-based provider discovery |
| `GetAvailableSlotsAsync` | Slot availability / double-booking prevention |
| `CreateBookingAsync` | Create booking |
| `ConfirmBookingAsync` | Customer confirms → Awaiting Payment |
| `InitiatePaymentAsync` | Payment session creation |
| `ConfirmPaymentAsync` | Client-side payment confirmation |
| `ConfirmPaymentFromWebhookAsync` | Webhook-driven payment confirmation |
| `AcceptBookingAsync` / `RejectBookingAsync` | Craftsman actions |
| `CancelBookingAsync` | Customer/craftsman cancel |
| `CompleteBookingAsync` / `MarkNoShowAsync` | Completion flows |
| `RescheduleBookingAsync` | Reschedule |
| `GetBookingAsync` / `ListBookingsAsync` | User-scoped reads |
| `AdminListBookingsAsync` / `AdminGetBookingAsync` / `GetAdminStatsAsync` | Admin reads |
| `GetNotificationsAsync` / `MarkNotificationReadAsync` | In-app notifications |

**Dependencies:** `IUnitOfWork`, `IPaymentGateway`, `IPushNotificationService`, `IConfiguration`  
**Recommendation:** Extract `NotificationService` and `BookingPaymentService`.

---

### AuthService — `Infrastructure/Services/Identity/AuthService.cs` (542 LOC)

**Interface:** `IAuthService`  
**MediatR consumers:** `Features/Auth/Commands/AuthCommands.cs`, `AdminVerifyEmailCommand`, `AdminVerifyUserEmailCommand`

| Public method | Responsibility |
|---------------|----------------|
| `RegisterAsync` | User registration + role assignment |
| `LoginAsync` | Credential auth + session + JWT |
| `RefreshTokenAsync` | Token rotation |
| `RevokeTokenAsync` | Logout |
| `ForgotPasswordAsync` / `ResetPasswordAsync` / `ChangePasswordAsync` | Password flows |
| `VerifyEmailAsync` / `ResendEmailVerificationAsync` | Email verification |
| `AdminVerifyEmailAsync` | Admin manual email verify (by email) |
| `SendPhoneOtpAsync` / `VerifyPhoneOtpAsync` | Phone OTP |

**Dependencies:** `IIdentityRepository`, `ITokenService`, `ISessionRepository`, `IPasswordPolicyService`, `IAuditService`, `IEmailSender`, `ISmsSender`, `IConfiguration`  
**Co-located in same file:** `ProfileService`, `LoginHistoryService` (via `SessionService.cs` file)

---

### PaymentWebhookService — `Infrastructure/Services/Payments/PaymentWebhookService.cs` (86 LOC)

**Interface:** `IPaymentWebhookService`  
**MediatR consumer:** `ProcessMoyasarWebhookCommandHandler`

| Public method | Responsibility |
|---------------|----------------|
| `ProcessMoyasarWebhookAsync` | Validate signature, parse payload, delegate to `BookingService.ConfirmPaymentFromWebhookAsync` |

**Dependencies:** `IBookingService`, `IConfiguration`, `ILogger`

---

### SubscriptionPlanService — `Infrastructure/Services/SubscriptionPlanService.cs` (318 LOC)

**Interface:** `ISubscriptionPlanService`  
**MediatR consumers:** `Features/Subscriptions/Commands/SubscriptionPlanCommands.cs`, `Features/Subscriptions/Queries/SubscriptionPlanQueries.cs`

| Public method | Responsibility |
|---------------|----------------|
| `CreateAsync` / `UpdateAsync` / `DeleteAsync` | Plan CRUD |
| `GetByIdAsync` / `SearchAsync` / `GetPublicPlansAsync` | Reads |
| `CloneAsync` | Plan duplication |
| `ActivateAsync` / `DeactivateAsync` / `SuspendAsync` / `ArchiveAsync` | Lifecycle |

**Dependencies:** `ApplicationDbContext`, `IMapper`

---

### UserSubscriptionService — `Infrastructure/Services/UserSubscriptionService.cs` (256 LOC)

**Interface:** `IUserSubscriptionService`  
**MediatR consumers:** `Features/Subscriptions/Commands/UserSubscriptionCommands.cs`, `Features/Subscriptions/Queries/UserSubscriptionQueries.cs`

| Public method | Responsibility |
|---------------|----------------|
| `GetCurrentAsync` / `GetHistoryAsync` | User reads |
| `SubscribeAsync` / `CancelAsync` / `UpdateAutoRenewAsync` | User actions |
| `SearchAdminAsync` / `GetByIdAdminAsync` | Admin reads |
| `GrantAsync` / `CancelAdminAsync` | Admin actions |

**Dependencies:** `ApplicationDbContext`, `IMapper`

---

## All Infrastructure Services

| Service file | LOC | Interface | Primary responsibility |
|--------------|-----|-----------|------------------------|
| `AdminService.cs` | 1,455 | `IAdminService` | Admin monolith (see above) |
| `DatabaseSeeder.cs` | 715 | — | Seed data (not runtime API) |
| `BookingService.cs` | 653 | `IBookingService` | Bookings + payments + notifications |
| `Identity/AuthService.cs` | 542 | `IAuthService` | Authentication |
| `UserManagementService.cs` | 401 | `IUserManagementService` | Admin user management |
| `SubscriptionPlanService.cs` | 318 | `ISubscriptionPlanService` | Subscription plans |
| `UserSubscriptionService.cs` | 256 | `IUserSubscriptionService` | User subscriptions |
| `ChatService.cs` | 236 | `IChatService` | Booking chat |
| `CraftsmanPortalService.cs` | 227 | `ICraftsmanService` | Craftsman portal |
| `IdentitySeeder.cs` | 222 | — | Identity seed |
| `Integrations/IntegrationReadinessService.cs` | 195 | `IIntegrationReadinessService` | Integration health report |
| `Payments/MoyasarPaymentGateway.cs` | 177 | `IPaymentGateway` | Moyasar payment sessions |
| `StoreService.cs` | 172 | `IStoreService` | Store portal |
| `Identity/SessionService.cs` | 169 | `ISessionService` | Session management |
| `Backup/DatabaseBackupService.cs` | 149 | `IDatabaseBackupService` | DB backup/restore |
| `SupportService.cs` | 148 | `ISupportService` | Complaints, tickets, reviews, ads |
| `VerificationDocumentService.cs` | 142 | `IVerificationDocumentService` | KYC documents |
| `Push/FirebasePushNotificationService.cs` | 135 | `IPushNotificationService` | FCM push |
| `Push/ApnsPushNotificationSender.cs` | 133 | — | APNs sender |
| `Identity/Email/EmailProviders.cs` | 121 | `IEmailSender` | Email providers |
| `Identity/TokenService.cs` | 80 | `ITokenService` | JWT generation |
| `Identity/Sms/SmsProviders.cs` | 73 | `ISmsSender` | SMS providers |
| `Identity/AuditService.cs` | 54 | `IAuditService` | Security audit log |
| `TokenCleanupService.cs` | 54 | — | Background token cleanup |
| `Identity/PasswordPolicyService.cs` | 52 | `IPasswordPolicyService` | Password rules |
| `Payments/DevelopmentPaymentGateway.cs` | 47 | `IPaymentGateway` | Dev payment stub |
| `Push/DevelopmentPushNotificationService.cs` | 45 | `IPushNotificationService` | Dev push stub |
| `Identity/CurrentUserService.cs` | 43 | `ICurrentUserService` | HTTP context user |
| `LocationCatalogService.cs` | 42 | `ILocationCatalogService` | Region/city validation |
| `Identity/PermissionService.cs` | 44 | `IPermissionService` | Permission checks |
| `Payments/PaymentWebhookService.cs` | 86 | `IPaymentWebhookService` | Moyasar webhooks |
| `CouponService.cs` | 57 | `ICouponService` | Coupon validation |
| `DeviceTokenService.cs` | 55 | `IDeviceTokenService` | Push token registry |
| `AdminExportService.cs` | 64 | — | Excel/PDF generation |

### Co-located identity services (in `SessionService.cs`)
| Class | Interface | Responsibility |
|-------|-----------|----------------|
| `ProfileService` | `IProfileService` | Identity profile get/update |
| `LoginHistoryService` | `ILoginHistoryService` | Login history pagination |

---

## Handler → Service Dependency Matrix

| Handler file | Injected dependency | Bypasses service layer? |
|--------------|---------------------|-------------------------|
| `AuthCommands.cs` | `IAuthService` | No |
| `IdentityCommands.cs` | `ISessionService`, `IProfileService`, `ILoginHistoryService`, `IAuthService`, `IPermissionService` | No |
| `UserQueries.cs` | `IUnitOfWork`, `IMapper` | **Yes** — direct repository |
| `AdminUserCommands.cs` | `IUserManagementService`, `IAuthService` | No |
| `AdminUserQueries.cs` | `IUserManagementService` | No |
| `ServiceQueries.cs` | `IUnitOfWork`, `IMapper` | **Yes** — direct repository |
| `BookingCommands.cs` | `IBookingService` | No |
| `CraftsmanCommands.cs` | `ICraftsmanService` | No |
| `StoreCommands.cs` | `IStoreService` | No |
| `SubscriptionPlanCommands.cs` | `ISubscriptionPlanService` | No |
| `SubscriptionPlanQueries.cs` | `ISubscriptionPlanService` | No |
| `UserSubscriptionCommands.cs` | `IUserSubscriptionService` | No |
| `UserSubscriptionQueries.cs` | `IUserSubscriptionService` | No |
| `VerificationCommands.cs` | `IVerificationDocumentService`, `IAdminService`, `IDatabaseBackupService` | No |
| `SupportCommands.cs` | `ISupportService`, `ICouponService` | No |
| `MessagingCommands.cs` | `IDeviceTokenService`, `IChatService` | No |
| `PaymentWebhookCommands.cs` | `IPaymentWebhookService` | No |
| `AdminCommands.cs` | `IAdminService` | No |
| `HealthController` | `IIntegrationReadinessService`, `ApplicationDbContext` | **Yes** — no MediatR |

---

## Split Recommendations (Phase 1+)

1. **Decompose AdminService** into 5–6 focused services aligned with bounded contexts.
2. **Unify profile paths** — consolidate `ProfileService` and `UserQueries` handlers behind one `IUserProfileService`.
3. **Extract NotificationService** from `BookingService`.
4. **Add validators** for all command DTOs currently missing FluentValidation.
5. **Standardize authorization** — prefer `[HasPermission]` over runtime `EnsurePermissionAsync` where possible.
