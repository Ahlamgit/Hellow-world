# Backend Module Architecture — Phase 0.5 Discovery

**Generated:** 2026-07-22  
**Location:** `docs/phase0.5/`  
**Scope:** `/workspace/src/backend`  
**Summary:** 36 controllers · 169 endpoints · 18 Feature folders · 34 Infrastructure services · 12 FluentValidation files

---

## 1. Controllers & Endpoints

> Pattern: Almost all controllers use **MediatR** (`IMediator.Send`). Exception: `HealthController` injects `IIntegrationReadinessService` + `ApplicationDbContext` directly.

### AuthController — `api/v1/auth`
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| POST | `/api/v1/auth/register` | AllowAnonymous | — | RegisterCommand |
| POST | `/api/v1/auth/login` | AllowAnonymous | — | LoginCommand |
| POST | `/api/v1/auth/refresh` | AllowAnonymous | — | RefreshTokenCommand |
| POST | `/api/v1/auth/revoke` | Authorize | — | RevokeTokenCommand |
| POST | `/api/v1/auth/forgot-password` | AllowAnonymous | — | ForgotPasswordCommand |
| POST | `/api/v1/auth/reset-password` | AllowAnonymous | — | ResetPasswordCommand |
| POST | `/api/v1/auth/change-password` | Authorize | — | ChangePasswordCommand |
| POST | `/api/v1/auth/verify-email` | AllowAnonymous | — | VerifyEmailCommand |
| POST | `/api/v1/auth/resend-email-verification` | AllowAnonymous | — | ResendEmailVerificationCommand |
| POST | `/api/v1/auth/admin/verify-email` | Authorize | — | AdminVerifyEmailCommand |
| POST | `/api/v1/auth/phone/send-otp` | Authorize | — | SendPhoneOtpCommand |
| POST | `/api/v1/auth/phone/verify-otp` | Authorize | — | VerifyPhoneOtpCommand |
| GET | `/api/v1/auth/me` | Authorize | — | GetMyPermissionsQuery (inline) |
| GET | `/api/v1/auth/permissions` | Authorize | — | GetMyPermissionsQuery |

### UsersController — `api/v1/users` (class: Authorize)
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| GET | `/api/v1/users/me` | Authorize | — | GetUserProfileQuery |
| PUT | `/api/v1/users/me` | Authorize | — | UpdateUserProfileCommand |
| POST | `/api/v1/users/me/addresses` | Authorize | — | AddAddressCommand |
| GET | `/api/v1/users/me/addresses` | Authorize | — | GetMyAddressesQuery |
| PUT | `/api/v1/users/me/addresses/{id}` | Authorize | — | UpdateAddressCommand |
| DELETE | `/api/v1/users/me/addresses/{id}` | Authorize | — | DeleteAddressCommand |

### ProfileController — `api/v1/profile` (class: Authorize)
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| GET | `/api/v1/profile` | Authorize | — | GetProfileQuery |
| PUT | `/api/v1/profile` | Authorize | — | UpdateProfileCommand |

### SessionsController — `api/v1/sessions` (class: Authorize)
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| GET | `/api/v1/sessions` | Authorize | SessionsView | GetSessionsQuery |
| GET | `/api/v1/sessions/user/{userId}` | Authorize | SessionsView | GetSessionsQuery |
| DELETE | `/api/v1/sessions/{sessionId}` | Authorize | — | RevokeSessionCommand |
| DELETE | `/api/v1/sessions/others` | Authorize | — | RevokeAllOtherSessionsCommand |
| DELETE | `/api/v1/sessions` | Authorize | SessionsRevokeAll | RevokeAllSessionsCommand |

### LoginHistoryController — `api/v1/login-history` (class: Authorize)
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| GET | `/api/v1/login-history` | Authorize | LoginHistoryView | GetLoginHistoryQuery |

### ServicesController — `api/v1/services`
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| GET | `/api/v1/services/categories` | AllowAnonymous | — | GetServiceCategoriesQuery |
| GET | `/api/v1/services` | AllowAnonymous | — | GetServicesQuery |

### BookingsController — `api/v1/bookings` (class: Authorize)
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| GET | `/api/v1/bookings/craftsmen` | AllowAnonymous | — | GetCraftsmenForServiceQuery |
| GET | `/api/v1/bookings/craftsmen/nearby` | AllowAnonymous | — | GetNearbyCraftsmenForServiceQuery |
| GET | `/api/v1/bookings/availability` | AllowAnonymous | — | GetAvailableSlotsQuery |
| POST | `/api/v1/bookings` | Authorize | BookingsCreate | CreateBookingCommand |
| GET | `/api/v1/bookings/{id}` | Authorize | — | GetBookingQuery |
| GET | `/api/v1/bookings` | Authorize | — | ListBookingsQuery |
| POST | `/api/v1/bookings/{id}/confirm` | Authorize | BookingsCreate | ConfirmBookingCommand |
| POST | `/api/v1/bookings/{id}/payment` | Authorize | BookingsCreate | InitiatePaymentCommand |
| POST | `/api/v1/bookings/{id}/payment/confirm` | Authorize | BookingsCreate | ConfirmPaymentCommand |
| POST | `/api/v1/bookings/{id}/accept` | Authorize | BookingsApprove | AcceptBookingCommand |
| POST | `/api/v1/bookings/{id}/reject` | Authorize | BookingsApprove | RejectBookingCommand |
| POST | `/api/v1/bookings/{id}/cancel` | Authorize | BookingsCancel | CancelBookingCommand |
| POST | `/api/v1/bookings/{id}/complete` | Authorize | BookingsApprove | CompleteBookingCommand |
| POST | `/api/v1/bookings/{id}/no-show` | Authorize | BookingsApprove | MarkNoShowCommand |
| POST | `/api/v1/bookings/{id}/reschedule` | Authorize | BookingsEdit | RescheduleBookingCommand |

### AdminBookingsController — `api/v1/admin/bookings` (class: Authorize)
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| GET | `/api/v1/admin/bookings` | Authorize | BookingsView | AdminListBookingsQuery |
| GET | `/api/v1/admin/bookings/stats` | Authorize | BookingsView | GetAdminBookingStatsQuery |
| GET | `/api/v1/admin/bookings/{id}` | Authorize | BookingsView | AdminGetBookingQuery |

### NotificationsController — `api/v1/notifications` (class: Authorize)
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| GET | `/api/v1/notifications` | Authorize | — | GetNotificationsQuery |
| POST | `/api/v1/notifications/{id}/read` | Authorize | — | MarkNotificationReadCommand |

### MeCraftsmanController — `api/v1/me/craftsman` (class: Authorize Roles=Craftsman)
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| GET | `/api/v1/me/craftsman` | Authorize(Craftsman) | — | GetMyCraftsmanProfileQuery |
| PUT | `/api/v1/me/craftsman` | Authorize(Craftsman) | — | UpdateMyCraftsmanProfileCommand |
| POST | `/api/v1/me/craftsman/services` | Authorize(Craftsman) | — | UpsertCraftsmanServiceCommand |
| DELETE | `/api/v1/me/craftsman/services/{id}` | Authorize(Craftsman) | — | DeleteCraftsmanServiceCommand |
| POST | `/api/v1/me/craftsman/working-hours` | Authorize(Craftsman) | — | UpsertCraftsmanWorkingHourCommand |
| DELETE | `/api/v1/me/craftsman/working-hours/{id}` | Authorize(Craftsman) | — | DeleteCraftsmanWorkingHourCommand |

### MeStoreController — `api/v1/me/store` (class: Authorize Roles=StoreOwner,StoreEmployee)
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| GET | `/api/v1/me/store` | Authorize(StoreOwner,StoreEmployee) | — | GetMyStoreProfileQuery |
| PUT | `/api/v1/me/store` | Authorize(StoreOwner,StoreEmployee) | — | UpdateMyStoreProfileCommand |
| POST | `/api/v1/me/store/products` | Authorize(StoreOwner,StoreEmployee) | — | UpsertStoreProductCommand |
| PUT | `/api/v1/me/store/products/{id}` | Authorize(StoreOwner,StoreEmployee) | — | UpsertStoreProductCommand |
| DELETE | `/api/v1/me/store/products/{id}` | Authorize(StoreOwner,StoreEmployee) | — | DeleteStoreProductCommand |

### MeSubscriptionController — `api/v1/me` (class: Authorize)
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| GET | `/api/v1/me/subscription` | Authorize | — | GetCurrentUserSubscriptionQuery |
| GET | `/api/v1/me/subscriptions` | Authorize | — | GetUserSubscriptionHistoryQuery |
| POST | `/api/v1/me/subscription` | Authorize(Craftsman,StoreOwner) | — | SubscribeCommand |
| POST | `/api/v1/me/subscription/{id}/cancel` | Authorize | — | CancelUserSubscriptionCommand |
| PATCH | `/api/v1/me/subscription/auto-renew` | Authorize | — | UpdateAutoRenewCommand |

### SubscriptionPlansController — `api/v1/subscription-plans`
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| GET | `/api/v1/subscription-plans` | AllowAnonymous | — | GetPublicSubscriptionPlansQuery |
| GET | `/api/v1/subscription-plans/{id}` | AllowAnonymous | — | GetSubscriptionPlanByIdQuery |

### AdminSubscriptionPlansController — `api/v1/admin/subscription-plans` (class: Authorize)
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| GET | `/api/v1/admin/subscription-plans` | Authorize | SubscriptionsView | SearchSubscriptionPlansQuery |
| GET | `/api/v1/admin/subscription-plans/{id}` | Authorize | SubscriptionsView | GetSubscriptionPlanByIdQuery |
| POST | `/api/v1/admin/subscription-plans` | Authorize | SubscriptionsCreate | CreateSubscriptionPlanCommand |
| PUT | `/api/v1/admin/subscription-plans/{id}` | Authorize | SubscriptionsEdit | UpdateSubscriptionPlanCommand |
| DELETE | `/api/v1/admin/subscription-plans/{id}` | Authorize | SubscriptionsEdit | DeleteSubscriptionPlanCommand |
| POST | `/api/v1/admin/subscription-plans/{id}/clone` | Authorize | SubscriptionsCreate | CloneSubscriptionPlanCommand |
| POST | `/api/v1/admin/subscription-plans/{id}/activate` | Authorize | SubscriptionsEdit | ActivateSubscriptionPlanCommand |
| POST | `/api/v1/admin/subscription-plans/{id}/deactivate` | Authorize | SubscriptionsEdit | DeactivateSubscriptionPlanCommand |
| POST | `/api/v1/admin/subscription-plans/{id}/suspend` | Authorize | SubscriptionsEdit | SuspendSubscriptionPlanCommand |
| POST | `/api/v1/admin/subscription-plans/{id}/archive` | Authorize | SubscriptionsEdit | ArchiveSubscriptionPlanCommand |

### AdminUserSubscriptionsController — `api/v1/admin/user-subscriptions` (class: Authorize)
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| GET | `/api/v1/admin/user-subscriptions` | Authorize | SubscriptionsView | SearchAdminUserSubscriptionsQuery |
| GET | `/api/v1/admin/user-subscriptions/{id}` | Authorize | SubscriptionsView | GetAdminUserSubscriptionByIdQuery |
| POST | `/api/v1/admin/user-subscriptions/users/{userId}` | Authorize | SubscriptionsCreate | GrantUserSubscriptionCommand |
| POST | `/api/v1/admin/user-subscriptions/{id}/cancel` | Authorize | SubscriptionsEdit | CancelAdminUserSubscriptionCommand |

### VerificationController — `api/v1/verification/documents` (class: Authorize Roles=Craftsman,StoreOwner,Store)
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| POST | `/api/v1/verification/documents` | Authorize(Craftsman,StoreOwner,Store) | — | SubmitVerificationDocumentCommand |

### AdminVerificationDocumentsController — `api/v1/admin/verification-documents` (class: Authorize)
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| GET | `/api/v1/admin/verification-documents` | Authorize | AdminAuthorization.EnsurePermissionAsync(UsersView) | ListVerificationDocumentsQuery |
| GET | `/api/v1/admin/verification-documents/{id}` | Authorize | AdminAuthorization.EnsurePermissionAsync(UsersView) | GetVerificationDocumentQuery |
| POST | `/api/v1/admin/verification-documents/{id}/approve` | Authorize | AdminAuthorization.EnsurePermissionAsync(UsersEdit) | ApproveVerificationDocumentCommand |
| POST | `/api/v1/admin/verification-documents/{id}/reject` | Authorize | AdminAuthorization.EnsurePermissionAsync(UsersEdit) | RejectVerificationDocumentCommand |

### LocationsController — `api/v1/locations` (class: AllowAnonymous)
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| GET | `/api/v1/locations/regions` | AllowAnonymous | — | ListRegionsQuery |
| GET | `/api/v1/locations/cities` | AllowAnonymous | — | ListCitiesQuery |

### SupportController — `api/v1`
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| POST | `/api/v1/complaints` | Authorize | — | CreateComplaintCommand |
| GET | `/api/v1/complaints/mine` | Authorize | — | GetMyComplaintsQuery |
| POST | `/api/v1/support-tickets` | Authorize | — | CreateSupportTicketCommand |
| GET | `/api/v1/support-tickets/mine` | Authorize | — | GetMySupportTicketsQuery |
| POST | `/api/v1/bookings/{id}/review` | Authorize(Customer) | — | SubmitBookingReviewCommand |
| GET | `/api/v1/coupons/validate` | AllowAnonymous | — | ValidateCouponQuery |
| GET | `/api/v1/advertisements` | AllowAnonymous | — | GetActiveAdsQuery |

### DevicesController — `api/v1/devices` (class: Authorize)
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| POST | `/api/v1/devices/push-token` | Authorize | — | RegisterPushTokenCommand |
| DELETE | `/api/v1/devices/push-token` | Authorize | — | UnregisterPushTokenCommand |

### ChatController — `api/v1/chat` (class: Authorize)
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| GET | `/api/v1/chat/conversations` | Authorize | — | GetMyChatConversationsQuery |
| GET | `/api/v1/chat/bookings/{bookingId}` | Authorize | — | GetBookingChatConversationQuery |
| GET | `/api/v1/chat/conversations/{conversationId}/messages` | Authorize | — | GetChatMessagesQuery |
| POST | `/api/v1/chat/conversations/{conversationId}/messages` | Authorize | — | SendChatMessageCommand |

### AdminController — `api/v1/admin` (class: Authorize; runtime `AdminAuthorization.Ensure*`)
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| GET | `/api/v1/admin/dashboard` | Authorize | PortalAccess (runtime) | GetAdminDashboardQuery |
| GET | `/api/v1/admin/{module}` | Authorize | Module view (runtime) | ListAdminModuleQuery |
| POST | `/api/v1/admin/{module}/bulk` | Authorize | Module bulk (runtime) | AdminBulkActionCommand |
| GET | `/api/v1/admin/{module}/export` | Authorize | ReportsExport (runtime) | ExportAdminModuleQuery |
| GET | `/api/v1/admin/analytics/data` | Authorize | ReportsView (runtime) | GetAdminAnalyticsQuery |
| GET | `/api/v1/admin/reports/list` | Authorize | ReportsView (runtime) | GetAdminReportsQuery |
| POST | `/api/v1/admin/reports/generate` | Authorize | ReportsExport (runtime) | GenerateAdminReportCommand |
| GET | `/api/v1/admin/system/health` | Authorize | SettingsManage (runtime) | GetAdminSystemHealthQuery |
| GET | `/api/v1/admin/backup/list` | Authorize | SettingsManage (runtime) | ListAdminBackupsQuery |
| POST | `/api/v1/admin/backup/create` | Authorize | SettingsManage (runtime) | CreateAdminBackupCommand |
| POST | `/api/v1/admin/backup/restore` | Authorize | SettingsManage (runtime) | RestoreAdminBackupCommand |
| GET | `/api/v1/admin/backup/{id}/download` | Authorize | SettingsManage (runtime) | DownloadAdminBackupQuery |

### AdminUsersController — `api/v1/admin/users` (class: Authorize)
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| GET | `/api/v1/admin/users` | Authorize | UsersView | SearchAdminUsersQuery |
| GET | `/api/v1/admin/users/{id}` | Authorize | UsersView | GetAdminUserByIdQuery |
| POST | `/api/v1/admin/users` | Authorize | UsersCreate | CreateAdminUserCommand |
| PUT | `/api/v1/admin/users/{id}` | Authorize | UsersEdit | UpdateAdminUserCommand |
| DELETE | `/api/v1/admin/users/{id}` | Authorize | UsersDelete | DeleteAdminUserCommand |
| POST | `/api/v1/admin/users/{id}/suspend` | Authorize | UsersSuspend | SuspendUserCommand |
| POST | `/api/v1/admin/users/{id}/activate` | Authorize | UsersEdit | ActivateUserCommand |
| PUT | `/api/v1/admin/users/{id}/roles` | Authorize | UsersEdit | AssignUserRolesCommand |
| POST | `/api/v1/admin/users/{id}/verify-email` | Authorize | UsersVerifyEmail | AdminVerifyUserEmailCommand |
| GET | `/api/v1/admin/users/{id}/permissions` | Authorize | UsersView | GetUserPermissionMatrixQuery |
| PUT | `/api/v1/admin/users/{id}/permissions` | Authorize | PermissionsManage | UpdateUserPermissionsCommand |

### AdminRbacController — `api/v1/admin/rbac` (class: Authorize)
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| GET | `/api/v1/admin/rbac/roles` | Authorize | RolesView | ListRbacRolesQuery |
| GET | `/api/v1/admin/rbac/roles/{roleId}/permissions` | Authorize | RolesView | GetRolePermissionMatrixQuery |
| PUT | `/api/v1/admin/rbac/roles/{roleId}/permissions` | Authorize | RolesManage | UpdateRolePermissionsCommand |

### AdminSettingsController — `api/v1/admin/settings` (class: Authorize)
| Method | Route | Auth | HasPermission | MediatR |
|--------|-------|------|---------------|---------|
| GET | `/api/v1/admin/settings` | Authorize | SettingsManage | ListSystemSettingsQuery |
| PUT | `/api/v1/admin/settings/{id}` | Authorize | SettingsManage | UpdateSystemSettingCommand |

### AdminCatalogController (2 classes)
**AdminCategoriesController** — `api/v1/admin/categories` · **AdminServicesController** — `api/v1/admin/services`  
All endpoints: Authorize + `SettingsManage` → List/Get/Create/Update/Delete category/service MediatR commands.

### AdminLocationsController (3 classes)
**AdminRegionsController** — `api/v1/admin/regions` · **AdminCitiesController** — `api/v1/admin/cities`  
All CRUD: Authorize + `SettingsManage` → region/city MediatR commands.

**MoyasarWebhookController** — `api/v1/webhooks/moyasar` (AllowAnonymous)  
POST → ProcessMoyasarWebhookCommand

### AdminMarketingController (2 classes)
**AdminCouponsController** — `api/v1/admin/coupons` · **AdminAdvertisementsController** — `api/v1/admin/advertisements`  
Coupons: `SettingsManage` · Ads: `AdvertisementsManage`

### AdminOperationsControllers (3 classes)
| Controller | Route prefix | Auth pattern | MediatR |
|------------|--------------|--------------|---------|
| AdminComplaintsController | `/api/v1/admin/complaints` | EnsurePermissionAsync UsersView/Edit | GetAdminComplaintQuery, ResolveAdminComplaintCommand |
| AdminSupportTicketsController | `/api/v1/admin/support-tickets` | EnsurePermissionAsync UsersView/Edit | GetAdminSupportTicketQuery, CloseAdminSupportTicketCommand |
| AdminPaymentsController | `/api/v1/admin/payments` | EnsurePermissionAsync PaymentsView | GetAdminPaymentQuery |

### HealthController — `api/v1/health` (no auth; **direct service injection**)
| Method | Route | Dependency |
|--------|-------|------------|
| GET | `/api/v1/health` | inline |
| GET | `/api/v1/health/ready` | ApplicationDbContext |
| GET | `/api/v1/health/integrations` | IIntegrationReadinessService |

---

## 2. MediatR Registration

- **Assembly scan:** `Khadamati.Application` via `AddMediatR` in `Khadamati.Application/DependencyInjection.cs`
- **Pipeline:** `ValidationBehavior<,>` (FluentValidation) on all requests
- **Handler location:** Co-located with commands/queries in `Features/**/` (18 files)
- **Direct repository (no service):** `UserQueries` handlers → `IUnitOfWork`; `ServiceQueries` handlers → `IUnitOfWork`

---

## 3. Bounded Context Groupings

### Identity
| Layer | Paths |
|-------|-------|
| Controllers | `AuthController.cs`, `SessionsController` + `ProfileController` + `LoginHistoryController` in `IdentityControllers.cs` |
| Commands/Queries | `Features/Auth/Commands/AuthCommands.cs`, `Features/Identity/Commands/IdentityCommands.cs` |
| Services | `Infrastructure/Services/Identity/AuthService.cs`, `SessionService.cs`, `TokenService.cs`, `PermissionService.cs`, `AuditService.cs`, `PasswordPolicyService.cs`, `CurrentUserService.cs`, `Email/EmailProviders.cs`, `Sms/SmsProviders.cs` |
| Entities | `User.cs`, `RefreshToken.cs`, `EmailVerificationToken.cs`, `PasswordResetToken.cs`, `PhoneOtpToken.cs`, `AuditLog.cs`, `Entities/Identity/*` (Role, LoginHistory, SecurityLog, PasswordHistory, UserPermission, UserRoleAssignment) |
| DTOs | `DTOs/Identity/IdentityDtos.cs`, `DTOs/Auth/AuthDtos.cs` |

### Users
| Layer | Paths |
|-------|-------|
| Controllers | `UsersController.cs`, `AdminUsersController.cs` |
| Commands/Queries | `Features/Users/Queries/UserQueries.cs`, `Features/Users/Queries/AdminUserQueries.cs`, `Features/Users/Commands/AdminUserCommands.cs` |
| Services | `UserManagementService.cs` (401 LOC) |
| Entities | `User.cs`, `UserProfile.cs`, `Address.cs` |
| DTOs | `DTOs/Users/UserDtos.cs`, `DTOs/Users/AdminUserDtos.cs`, `DTOs/Users/UserPermissionDtos.cs` |

### Providers (Craftsmen)
| Layer | Paths |
|-------|-------|
| Controllers | `MeCraftsmanController.cs`, craftsman discovery in `BookingsController.cs` |
| Commands/Queries | `Features/Craftsman/CraftsmanCommands.cs`, booking craftsman queries in `Features/Bookings/Commands/BookingCommands.cs` |
| Services | `CraftsmanPortalService.cs` (227 LOC), `BookingService.cs` (discovery) |
| Entities | `CraftsmanProfile.cs`, `CraftsmanService.cs`, `CraftsmanWorkingHour.cs` |
| DTOs | `DTOs/Craftsman/CraftsmanDtos.cs`, craftsman fields in `DTOs/Bookings/BookingDtos.cs` |

### Stores
| Layer | Paths |
|-------|-------|
| Controllers | `MeStoreController.cs` |
| Commands/Queries | `Features/Store/StoreCommands.cs` |
| Services | `StoreService.cs` (172 LOC) |
| Entities | `StoreProfile.cs`, `StoreProduct.cs` |
| DTOs | `DTOs/Store/StoreDtos.cs` |

### Services (Catalog)
| Layer | Paths |
|-------|-------|
| Controllers | `ServicesController.cs`, `AdminCategoriesController` + `AdminServicesController` in `AdminCatalogController.cs` |
| Commands/Queries | `Features/Services/Queries/ServiceQueries.cs`, admin catalog commands in `Features/Admin/Commands/AdminCommands.cs` |
| Services | `AdminService.cs` (catalog CRUD), public queries via `IUnitOfWork` |
| Entities | `Service.cs`, `ServiceCategory.cs` |
| DTOs | `DTOs/Services/ServiceDtos.cs`, `DTOs/Admin/AdminCatalogDtos.cs` |

### Bookings
| Layer | Paths |
|-------|-------|
| Controllers | `BookingsController.cs`, `AdminBookingsController` in `BookingsController.cs` |
| Commands/Queries | `Features/Bookings/Commands/BookingCommands.cs` |
| Services | `BookingService.cs` (653 LOC) |
| Entities | `ServiceRequest.cs`, `ServiceRequestStatusHistory.cs`, `BookingPayment.cs`, `BookingSlotReservation.cs` |
| DTOs | `DTOs/Bookings/BookingDtos.cs` |

### Payments
| Layer | Paths |
|-------|-------|
| Controllers | booking payment actions in `BookingsController.cs`, `MoyasarWebhookController`, `AdminPaymentsController` |
| Commands/Queries | payment commands in `BookingCommands.cs`, `Features/Payments/Commands/PaymentWebhookCommands.cs`, `GetAdminPaymentQuery` in `AdminCommands.cs` |
| Services | `Payments/PaymentWebhookService.cs` (86 LOC), `Payments/MoyasarPaymentGateway.cs` (177 LOC), `Payments/DevelopmentPaymentGateway.cs` (47 LOC) |
| Entities | `BookingPayment.cs` |
| DTOs | `DTOs/Payments/PaymentDtos.cs`, `DTOs/Payments/PaymentWebhookDtos.cs` |

### Subscriptions
| Layer | Paths |
|-------|-------|
| Controllers | `SubscriptionPlansController.cs`, `MeSubscriptionController.cs`, `AdminSubscriptionPlansController.cs`, `AdminUserSubscriptionsController.cs` |
| Commands/Queries | `Features/Subscriptions/Commands/*`, `Features/Subscriptions/Queries/*` |
| Services | `SubscriptionPlanService.cs` (318 LOC), `UserSubscriptionService.cs` (256 LOC) |
| Entities | `SubscriptionPlan.cs`, `PlanBillingOption.cs`, `UserSubscription.cs` |
| DTOs | `DTOs/Subscriptions/SubscriptionPlanDtos.cs`, `DTOs/Subscriptions/UserSubscriptionDtos.cs` |

### Marketing
| Layer | Paths |
|-------|-------|
| Controllers | `AdminCouponsController` + `AdminAdvertisementsController` in `AdminMarketingController.cs`, public ads/coupons in `SupportController.cs` |
| Commands/Queries | marketing commands in `AdminCommands.cs`, `ValidateCouponQuery` + `GetActiveAdsQuery` in `SupportCommands.cs` |
| Services | `CouponService.cs` (57 LOC), `AdminService.cs` (coupon/ad CRUD), `SupportService.cs` (ads) |
| Entities | `Coupon.cs`, `Advertisement.cs` in `AdminEntities.cs` |
| DTOs | `DTOs/Admin/AdminMarketingDtos.cs`, coupon DTOs in `DTOs/Support/SupportDtos.cs` |

### Notifications
| Layer | Paths |
|-------|-------|
| Controllers | `NotificationsController` in `BookingsController.cs`, `DevicesController` in `MessagingControllers.cs` |
| Commands/Queries | notification commands in `BookingCommands.cs`, `Features/Messaging/MessagingCommands.cs` |
| Services | `BookingService.cs` (in-app notifications), `DeviceTokenService.cs` (55 LOC), `Push/FirebasePushNotificationService.cs` (135 LOC), `Push/DevelopmentPushNotificationService.cs` (45 LOC), `Push/ApnsPushNotificationSender.cs` (133 LOC) |
| Entities | `Notification.cs`, `DevicePushToken.cs` |
| DTOs | `DTOs/Messaging/PushDtos.cs`, `NotificationDto` in `DTOs/Bookings/BookingDtos.cs` |

### Locations
| Layer | Paths |
|-------|-------|
| Controllers | `LocationsController.cs`, `AdminRegionsController` + `AdminCitiesController` in `AdminLocationsController.cs` |
| Commands/Queries | location commands in `AdminCommands.cs` |
| Services | `AdminService.cs` (region/city CRUD), `LocationCatalogService.cs` (42 LOC) |
| Entities | `Region.cs`, `City.cs` in `AdminEntities.cs` |
| DTOs | `DTOs/Admin/AdminLocationDtos.cs` |

### Verification
| Layer | Paths |
|-------|-------|
| Controllers | `VerificationController.cs`, `AdminVerificationDocumentsController.cs` |
| Commands/Queries | `Features/Verification/VerificationCommands.cs` |
| Services | `VerificationDocumentService.cs` (142 LOC) |
| Entities | `VerificationDocument.cs` |
| DTOs | `DTOs/Verification/VerificationDocumentDtos.cs` |

### Administration
| Layer | Paths |
|-------|-------|
| Controllers | `AdminController.cs`, `AdminRbacController.cs`, `AdminSettingsController.cs`, `AdminOperationsControllers.cs`, plus all `Admin*` resource controllers |
| Commands/Queries | `Features/Admin/Commands/AdminCommands.cs` (~475 LOC) |
| Services | `AdminService.cs` (1455 LOC), `AdminExportService.cs` (64 LOC), `Backup/DatabaseBackupService.cs` (149 LOC), `Integrations/IntegrationReadinessService.cs` (195 LOC) |
| Entities | `SystemSetting.cs`, `Permission.cs`, `RolePermission.cs`, `ActivityLog.cs`, `BackupJob.cs`, `Complaint.cs`, `SupportTicket.cs` in `AdminEntities.cs` |
| DTOs | `DTOs/Admin/AdminDtos.cs`, `AdminRbacDtos.cs`, `AdminSettingsDtos.cs`, `DTOs/Support/SupportDtos.cs` (admin complaint/ticket), `DTOs/Integrations/IntegrationReadinessDtos.cs` |

### Messaging (cross-cutting)
| Layer | Paths |
|-------|-------|
| Controllers | `ChatController` in `MessagingControllers.cs` |
| Commands/Queries | `Features/Messaging/MessagingCommands.cs` |
| Services | `ChatService.cs` (236 LOC) |
| Entities | `ChatConversation.cs`, `ChatMessage.cs` |
| DTOs | `DTOs/Messaging/ChatDtos.cs` |

---

## 4. Cross-Cutting Issues

### Duplicate / Overlapping Endpoints
| Overlap | Endpoints | Notes |
|---------|-----------|-------|
| User profile (3 surfaces) | `GET/PUT /users/me`, `GET/PUT /profile`, `GET /auth/me` | Different DTOs (`UserProfileDto` vs `ProfileDto` vs auth context object) |
| Admin email verify (2 paths) | `POST /auth/admin/verify-email`, `POST /admin/users/{id}/verify-email` | Different commands; auth path lacks permission attribute |
| Subscription plan by ID | `GET /subscription-plans/{id}` (public), `GET /admin/subscription-plans/{id}` | Same `GetSubscriptionPlanByIdQuery`; admin adds permission gate |
| Regions/Cities read | `GET /locations/*` (active-only, anonymous), `GET /admin/regions`, `GET /admin/cities` | Public uses admin queries with hardcoded filters |
| Service catalog read | `GET /services/*` (IUnitOfWork), `GET /admin/categories`, `GET /admin/services` | Different data access layers |
| Permissions | `GET /auth/permissions` vs permissions embedded in `GET /auth/me` | Same query, redundant |

### Missing FluentValidation (no `AbstractValidator` found)
- **Bookings:** `ConfirmBookingDto`
- **Subscriptions:** `UpdateSubscriptionPlanDto`, `SubscribeRequestDto`, `CancelSubscriptionDto`, `UpdateAutoRenewDto`, `AdminGrantSubscriptionDto`, `UserSubscriptionListQueryDto`
- **Craftsman/Store:** all portal DTOs
- **Verification:** `SubmitVerificationDocumentDto`, `RejectVerificationDocumentDto`, list query DTOs
- **Support:** complaint/ticket/review DTOs
- **Messaging:** push token + chat message DTOs
- **Admin catalog/settings/marketing:** category/service/region/city/coupon/ad/setting DTOs
- **Payments:** `MoyasarWebhookDto`
- **Admin:** `AdminGenerateReportRequestDto`, `AdminRestoreRequestDto`, `ResolveComplaintDto`, `UpdateUserPermissionsDto`, RBAC DTOs

### Missing / Weak Authorization
| Endpoint group | Issue |
|----------------|-------|
| `POST /auth/admin/verify-email` | `[Authorize]` only — no `HasPermission` or admin role check |
| `SessionsController` revoke endpoints | No `HasPermission`; relies on inline permission check in handler for cross-user revoke |
| `UsersController`, `ProfileController` | Authenticated only; no resource-level permission |
| `NotificationsController`, `DevicesController`, `ChatController` | Authenticated only |
| `SupportController` user endpoints | Authenticated only |
| `MeSubscriptionController` | Authenticated only (except subscribe role gate) |
| `HealthController` | Unauthenticated (expected for probes) |
| `MoyasarWebhookController` | AllowAnonymous; signature validated in service |

---

## 5. Architecture Observations

1. **AdminService god-object:** 1455 LOC covering dashboard, generic module listing, catalog, locations, marketing, complaints, tickets, payments, RBAC, settings, backups.
2. **Dual profile models:** Identity `ProfileDto` vs Users `UserProfileDto` create parallel update paths.
3. **MediatR coverage:** ~98% of endpoints; Health is the exception.
4. **Authorization styles:** Three patterns coexist — `[HasPermission]`, `[Authorize(Roles=...)]`, and runtime `AdminAuthorization.EnsurePermissionAsync`.
5. **Validation coverage:** Strong for Auth, Admin users, Bookings (partial), Subscription plan create; weak elsewhere despite global `ValidationBehavior`.
