# Test Coverage Analysis — Phase 0.5 Discovery

**Generated:** 2026-07-22  
**Projects:** `Khadamati.Tests` (unit), `Khadamati.IntegrationTests` (API)

## Executive Summary

| Metric | Value |
|--------|-------|
| Unit test classes | 17 |
| Unit test methods (`[Fact]`) | 77 |
| Integration test classes | 1 |
| Integration test methods | 7 |
| **Total automated tests** | **84** |
| Client tests (web/android/ios) | **0** |
| Estimated BookingService coverage | Validator only — **no service tests** |
| Controller coverage | **~0%** direct — 7 integration smoke tests |

---

## 1. Khadamati.Tests — Full Inventory

### 1.1 Services

#### AdminOperationsServiceTests
| Method | Type |
|--------|------|
| `ResolveComplaintAsync_SetsResolvedStatus` | Fact |

#### AdminServiceTests (9 tests)
| Method | Type |
|--------|------|
| `GetDashboardAsync_ReturnsCounts` | Fact |
| `ListModuleAsync_Users_ReturnsPaginatedResults` | Fact |
| `ListModuleAsync_Customers_FiltersByRole` | Fact |
| `ListModuleAsync_Regions_ReturnsSeededData` | Fact |
| `BulkActionAsync_ActivateUser_UpdatesStatus` | Fact |
| `ExportAsync_Users_ReturnsExcelBytes` | Fact |
| `ExportAsync_Users_ReturnsPdfBytes` | Fact |
| `GetSystemHealthAsync_ReturnsHealthyStatus` | Fact |
| `CreateBackupAsync_CreatesBackupJob` | Fact |

#### AuthServiceTests (4 tests)
| Method | Type |
|--------|------|
| `Register_ShouldCreateUserAndReturnTokens` | Fact |
| `Login_WithValidCredentials_ShouldReturnTokens` | Fact |
| `Login_WithInvalidPassword_ShouldThrow` | Fact |
| `ChangePassword_ShouldUpdateHash` | Fact |

#### DatabaseBackupServiceTests (1 test)
| Method | Type |
|--------|------|
| `CreateAndRestoreBackup_RoundTripsCatalogData` | Fact |

#### IdentityServiceTests.cs — contains multiple classes

**PermissionServiceTests (3 tests)**
| Method | Type |
|--------|------|
| `Admin_ShouldHaveUsersCreatePermission` | Fact |
| `Customer_ShouldNotHaveUsersCreatePermission` | Fact |
| `Customer_ShouldHaveUsersViewPermission` | Fact |

**PasswordPolicyServiceTests (2 tests)**
| Method | Type |
|--------|------|
| `WeakPassword_ShouldThrow` | Fact |
| `Customer_ShouldHaveUsersViewPermission` | Fact *(class name mismatch in file)* |
| `StrongPassword_ShouldPass` | Fact |

#### IntegrationReadinessServiceTests (3 tests)
| Method | Type |
|--------|------|
| `GetReport_AllDevelopmentProviders_IsNotProductionReady` | Fact |
| `GetReport_MoyasarFullyConfigured_IsProductionReadyForPayment` | Fact |
| `GetReport_MoyasarMissingSecret_IsMisconfigured` | Fact |

#### PaymentWebhookServiceTests (3 tests)
| Method | Type |
|--------|------|
| `ProcessMoyasarWebhookAsync_InvalidSignature_ThrowsUnauthorized` | Fact |
| `ProcessMoyasarWebhookAsync_PaidStatus_ConfirmsBooking` | Fact |
| `ProcessMoyasarWebhookAsync_PendingStatus_IsIgnored` | Fact |

#### SecurityServiceTests.cs — multiple classes

**PasswordHasherTests (3 tests)**
| Method | Type |
|--------|------|
| `Hash_ShouldProduceDifferentHashEachTime` | Fact |
| `Verify_WithCorrectPassword_ShouldReturnTrue` | Fact |
| `Verify_WithWrongPassword_ShouldReturnFalse` | Fact |

**OtpServiceTests (2 tests)**
| Method | Type |
|--------|------|
| `GenerateOtp_ShouldReturnSixDigits` | Fact |
| `VerifyOtp_WithCorrectCode_ShouldReturnTrue` | Fact |

**TokenServiceTests (2 tests)**
| Method | Type |
|--------|------|
| `HashAndVerifyToken_ShouldWork` | Fact |
| `GenerateAccessToken_ShouldIncludeClaims` | Fact |

#### SubscriptionPlanServiceTests (4 tests)
| Method | Type |
|--------|------|
| `Create_ShouldPersistPlanWithBillingOptions` | Fact |
| `Activate_ShouldSetStatusToActive` | Fact |
| `Clone_ShouldCreateCopyWithInactiveStatus` | Fact |
| `GetPublicPlans_ShouldReturnOnlyActivePlans` | Fact |

#### UserManagementServiceTests (4 tests)
| Method | Type |
|--------|------|
| `Create_ShouldCreateCustomerUser` | Fact |
| `Search_ShouldReturnPagedUsers` | Fact |
| `Suspend_ShouldUpdateStatus` | Fact |
| `AssignRoles_ShouldUpdatePrimaryRole` | Fact |

#### UserSubscriptionServiceTests (3 tests)
| Method | Type |
|--------|------|
| `Subscribe_ShouldCreateActiveSubscriptionAndSyncUser` | Fact |
| `Subscribe_ShouldRejectWhenActiveSubscriptionExists` | Fact |
| `Cancel_ShouldMarkSubscriptionCancelled` | Fact |

#### VerificationDocumentServiceTests (3 tests)
| Method | Type |
|--------|------|
| `SubmitAsync_SetsPendingReview` | Fact |
| `ApproveAsync_UpdatesUserVerificationStatus` | Fact |
| `RejectAsync_RequiresReason` | Fact |

### 1.2 Validators

#### AdminUserValidatorTests (2 tests)
- `CreateAdminUserValidator_ShouldRejectInvalidRole`
- `AssignUserRolesValidator_ShouldRequirePrimaryInRoles`

#### AdminValidatorTests (6 tests)
- `ListQuery_InvalidPage_Fails`
- `ListQuery_InvalidPageSize_Fails`
- `ListQuery_ValidInput_Passes`
- `BulkAction_InvalidAction_Fails`
- `BulkAction_EmptyIds_Fails`
- `BulkAction_ValidInput_Passes`

#### AuthValidatorTests (8 tests)
- `Register_WithValidData_ShouldPass`
- `Register_WithWeakPassword_ShouldFail`
- `Register_WithAdminRole_ShouldFail`
- `Login_WithValidData_ShouldPass`
- `Register_WithMismatchedConfirmPassword_ShouldFail`
- `ChangePassword_WithMismatchedConfirm_ShouldFail`
- `VerifyOtp_WithInvalidFormat_ShouldFail`
- `VerifyOtp_WithValidFormat_ShouldPass`

#### BookingValidatorTests (5 tests)
- `Create_WithValidData_ShouldPass`
- `Create_WithPastDate_ShouldFail`
- `Pending_To_AwaitingPayment_ShouldBeAllowed`
- `Pending_To_Confirmed_ShouldBeDenied`
- `PaymentConfirmed_To_PendingCraftsmanConfirmation_ShouldBeAllowed`

#### SubscriptionPlanValidatorTests (5 tests)
- `Create_WithValidData_ShouldPass`
- `Create_WithInvalidPlanCode_ShouldFail`
- `Create_WithoutBillingOptions_ShouldFail`
- `Create_WithDuplicateBillingCycles_ShouldFail`
- `Clone_WithValidCode_ShouldPass`

---

## 2. Khadamati.IntegrationTests — Full Inventory

### ApiIntegrationTests (7 tests)

| Method | Endpoint / behavior tested |
|--------|---------------------------|
| `Health_ReturnsHealthy` | `GET /api/v1/health` |
| `HealthReady_ReturnsReadyWithDatabase` | `GET /api/v1/health/ready` |
| `AdminDashboard_WithoutAuth_ReturnsUnauthorized` | `GET /api/v1/admin/dashboard` → 401 |
| `Login_WithSeededAdmin_ReturnsTokenWithPermissions` | `POST /api/v1/auth/login` |
| `AdminDashboard_WithAdminToken_ReturnsOk` | `GET /api/v1/admin/dashboard` → 200 |
| `AdminAnalytics_WithoutReportsPermission_ReturnsForbidden` | `GET /api/v1/admin/analytics/data` → 401 |
| `NearbyCraftsmen_WithCoordinates_ReturnsOrderedResults` | `GET /bookings/craftsmen/nearby` |

**Factory:** `KhadamatiWebApplicationFactory` (in-memory / test host setup)

---

## 3. Untested Critical Paths

### 3.1 Services — no dedicated unit tests

| Service | Risk | Why critical |
|---------|------|--------------|
| **BookingService** | **Critical** | Core revenue path: create, confirm, pay, accept, cancel, reschedule, state machine |
| ChatService | High | Real-time customer–craftsman communication |
| CouponService | High | Discount validation affects billing |
| CraftsmanPortalService | Medium | Provider availability and pricing |
| StoreService | Medium | Store product management |
| SupportService | Medium | Complaints and tickets |
| LocationCatalogService | Medium | Address/region data integrity |
| SessionService | Medium | Multi-device session security |
| ProfileService | Medium | PII updates |
| DeviceTokenService | Medium | Push delivery |
| EmailService / SmsService | High | Auth and notification delivery |
| MoyasarPaymentGateway | **Critical** | Live payment initiation |
| AdminExportService | Low | Partially exercised via AdminServiceTests |
| AuditService | Medium | Compliance trail |
| CurrentUserService | Low | Request context |
| FirebasePushNotificationService | Medium | Production notifications |

### 3.2 Controllers — no direct tests (24 controllers)

| Controller | Integration coverage |
|------------|---------------------|
| AuthController | Login only (integration) |
| BookingsController | Nearby craftsmen query only |
| AdminController | Dashboard only |
| HealthController | Health + ready |
| **All others** | **None** |

**Untested controller groups:**
- `BookingsController` — create, confirm, payment, lifecycle transitions
- `MeSubscriptionController` — subscribe, cancel, auto-renew
- `MeCraftsmanController` / `MeStoreController`
- `MessagingControllers` (chat, notifications)
- `SupportController`
- `UsersController` / `IdentityControllers`
- All `Admin*` controllers except dashboard/analytics auth check

### 3.3 BookingService — specific gaps

Validator tests cover:
- Create DTO validation (valid data, past date)
- 3 status transition rules

**Not tested at service layer:**
- Craftsman matching / nearby search logic
- Availability slot calculation
- Payment initiation and confirmation orchestration
- Expiry / no-show scheduling
- Review submission side effects
- Notification side effects on status change
- Concurrent booking conflicts
- Role-based authorization inside service

### 3.4 Payment path gaps

| Layer | Coverage |
|-------|----------|
| PaymentWebhookService | 3 unit tests (signature, paid, pending) |
| MoyasarPaymentGateway | **0 tests** |
| BookingsController payment endpoints | **0 integration tests** |
| End-to-end booking → pay → confirm | **0 tests** |

### 3.5 Client / E2E gaps

| Area | Status |
|------|--------|
| Web (Vitest/Jest/Playwright) | No test suite found |
| Android (JUnit/Espresso) | No test suite found |
| iOS (XCTest) | No test suite found |
| Contract tests (OpenAPI) | None |
| Load / performance tests | None |

---

## 4. Coverage by Domain

| Domain | Unit | Integration | Gap severity |
|--------|:----:|:-----------:|:------------:|
| Auth / identity | Good | Minimal | Medium |
| Admin ops | Good | Minimal | Medium |
| Subscriptions | Good | None | Medium |
| **Bookings** | **Validator only** | **1 read endpoint** | **Critical** |
| Payments | Webhook only | None | Critical |
| Chat / notifications | None | None | High |
| Support / verification | Partial | None | Medium |
| Locations / catalog | None | None | Medium |
| Push / devices | None | None | Medium |

---

## 5. Recommended Test Priorities (Phase 1+)

1. **BookingService** unit tests — state machine, payment flow, craftsman assignment
2. **BookingsController** integration tests — full happy path + auth matrix
3. **Payment** integration — initiate → webhook → confirmed booking
4. **MeSubscriptionController** — subscribe with/without coupon
5. **ChatService** + **SupportService** — basic CRUD and permissions
6. **Staging smoke extension** — booking create smoke, subscription list
7. **Client smoke** — web build + critical path Playwright (login → book)
