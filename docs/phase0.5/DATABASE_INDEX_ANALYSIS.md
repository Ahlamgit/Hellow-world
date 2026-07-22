# KHADAMATI — Database Index Analysis (Phase 0.5)

**Document version:** 1.0  
**Date:** 2026-07-22  
**Authority:** EF `ApplicationDbContextModelSnapshot` (runtime) vs `003_CreateIndexes.sql` (legacy)

---

## 1. Executive Summary

| Finding | Severity | Recommendation |
|---------|----------|----------------|
| EF missing soft-delete filtered unique indexes on Email/Phone | High | Add filtered unique `WHERE IsDeleted=0` |
| SQL geo index on Addresses not in EF | High | Add `(Latitude, Longitude)` index for nearby search |
| EF has rich booking/chat indexes; SQL uses different column names | Medium | Ignore SQL indexes; trust EF |
| No covering indexes for admin dashboard aggregates | Medium | Add composite indexes in Phase 2 |
| No `rowversion` for concurrency | Critical | Add on ServiceRequests, BookingSlotReservations, Coupons |

---

## 2. Existing EF Indexes (by Table)

### Identity & Users

| Table | Index | Type | Columns |
|-------|-------|------|---------|
| Users | IX_Users_Email | Unique | Email |
| Users | IX_Users_Phone | Unique | Phone |
| Users | IX_Users_PrimaryRoleId | | PrimaryRoleId |
| UserProfiles | IX_UserProfiles_UserId | Unique | UserId |
| RefreshTokens | IX_RefreshTokens_Token | Unique | Token |
| RefreshTokens | IX_RefreshTokens_UserId | | UserId |
| RefreshTokens | IX_RefreshTokens_DeviceId | | DeviceId |
| EmailVerificationTokens | IX_EmailVerificationTokens_UserId | | UserId |
| EmailVerificationTokens | IX_EmailVerificationTokens_ExpiresAt | | ExpiresAt |
| PasswordResetTokens | IX_PasswordResetTokens_UserId | | UserId |
| PasswordResetTokens | IX_PasswordResetTokens_ExpiresAt | | ExpiresAt |
| PhoneOtpTokens | IX_PhoneOtpTokens_UserId_Phone | | UserId, Phone |
| PhoneOtpTokens | IX_PhoneOtpTokens_ExpiresAt | | ExpiresAt |
| PasswordHistory | IX_PasswordHistory_UserId_ChangedAt | | UserId, ChangedAt |
| LoginHistory | IX_LoginHistory_Email | | Email |
| LoginHistory | IX_LoginHistory_UserId_LoginAt | | UserId, LoginAt |
| SecurityLogs | IX_SecurityLogs_CreatedAt | | CreatedAt |
| SecurityLogs | IX_SecurityLogs_EventType | | EventType |
| SecurityLogs | IX_SecurityLogs_UserId | | UserId |

### RBAC

| Table | Index | Type |
|-------|-------|------|
| Roles | IX_Roles_Name | Unique |
| Permissions | IX_Permissions_Code | Unique |
| RolePermissions | IX_RolePermissions_RoleId_PermissionId | Unique |
| UserRoles | IX_UserRoles_UserId_RoleId | Unique |
| UserPermissions | IX_UserPermissions_UserId_PermissionId | Unique |

### Catalog & Providers

| Table | Index | Type |
|-------|-------|------|
| ServiceCategories | IX_ServiceCategories_ParentCategoryId | |
| Services | IX_Services_CategoryId | |
| CraftsmanProfiles | IX_CraftsmanProfiles_UserId | Unique |
| CraftsmanServices | IX_CraftsmanServices_CraftsmanProfileId_ServiceId | Unique |
| CraftsmanServices | IX_CraftsmanServices_ServiceId | |
| CraftsmanWorkingHours | IX_CraftsmanWorkingHours_CraftsmanId_DayOfWeek | |
| StoreProfiles | IX_StoreProfiles_UserId | Unique |
| StoreProducts | IX_StoreProducts_StoreProfileId | |

### Bookings & Payments

| Table | Index | Type | Notes |
|-------|-------|------|-------|
| ServiceRequests | IX_ServiceRequests_BookingReference | Unique | |
| ServiceRequests | IX_ServiceRequests_CustomerId | | List by customer |
| ServiceRequests | IX_ServiceRequests_CraftsmanId | | List by craftsman |
| ServiceRequests | IX_ServiceRequests_ServiceId | | Filter by service |
| ServiceRequests | IX_ServiceRequests_Status | | Status filters |
| ServiceRequests | IX_ServiceRequests_ScheduledAt | | Date range queries |
| ServiceRequests | IX_ServiceRequests_AddressId | | |
| ServiceRequests | IX_ServiceRequests_RescheduledFromId | | |
| BookingPayments | IX_BookingPayments_ServiceRequestId | Unique | 1:1 payment |
| BookingPayments | IX_BookingPayments_PayerUserId | | |
| BookingPayments | IX_BookingPayments_PayeeUserId | | |
| BookingSlotReservations | IX_BookingSlotReservations_ServiceRequestId | Unique | |
| BookingSlotReservations | IX_BookingSlotReservations_CraftsmanId_SlotStart | **Filtered Unique** | `IsActive=1 AND IsDeleted=0` |
| ServiceRequestStatusHistories | IX_ServiceRequestStatusHistories_ServiceRequestId | | |
| ServiceRequestStatusHistories | IX_ServiceRequestStatusHistories_ChangedByUserId | | |

### Subscriptions

| Table | Index | Type |
|-------|-------|------|
| SubscriptionPlans | IX_SubscriptionPlans_PlanCode | Unique |
| SubscriptionPlans | IX_SubscriptionPlans_Status | |
| SubscriptionPlans | IX_SubscriptionPlans_TargetRole | |
| SubscriptionPlans | IX_SubscriptionPlans_DisplayPriority | |
| PlanBillingOptions | IX_PlanBillingOptions_PlanId_Cycle | Unique |
| UserSubscriptions | IX_UserSubscriptions_UserId | |
| UserSubscriptions | IX_UserSubscriptions_PlanId | |
| UserSubscriptions | IX_UserSubscriptions_BillingOptionId | |

### Messaging & Support

| Table | Index | Type |
|-------|-------|------|
| Notifications | IX_Notifications_UserId_IsRead | |
| DevicePushTokens | IX_DevicePushTokens_UserId_Token | Unique |
| ChatConversations | IX_ChatConversations_BookingId | Unique |
| ChatMessages | IX_ChatMessages_ConversationId_SentAt | |
| ChatMessages | IX_ChatMessages_SenderId | |
| Complaints | IX_Complaints_ComplainantUserId | |
| SupportTickets | IX_SupportTickets_TicketNumber | Unique |
| SupportTickets | IX_SupportTickets_UserId | |

### Locations & Admin

| Table | Index | Type |
|-------|-------|------|
| Regions | IX_Regions_Code | Unique |
| Cities | IX_Cities_RegionId | |
| Coupons | IX_Coupons_Code | Unique |
| SystemSettings | IX_SystemSettings_SettingKey | Unique |
| VerificationDocuments | IX_VerificationDocuments_UserId_Status | |
| ActivityLogs | IX_ActivityLogs_CreatedAt | |
| AuditLogs | IX_AuditLogs_CreatedAt | |
| AuditLogs | IX_AuditLogs_TableName | |

---

## 3. SQL Script Indexes Not in EF

| SQL Index | Purpose | EF Gap |
|-----------|---------|--------|
| IX_Users_Email (filtered Deleted=0) | Soft-delete safe unique | **Missing filter** |
| IX_Users_Phone (filtered Deleted=0) | Soft-delete safe unique | **Missing filter** |
| IX_Addresses_Geo | Lat/long nearby search | **Missing** |
| IX_Addresses_UserId_IsDefault | Default address lookup | **Missing** |
| IX_CraftsmanProfiles_Available_Rating | Provider search sort | **Missing** |
| IX_ServiceRequests_CustomerId (CreatedDate DESC) | Recent bookings | Partial — no DESC INCLUDE |
| IX_Notifications_UserId_IsRead (CreatedDate DESC) | Inbox sort | Partial — no CreatedAt in index |
| IX_UserSubscriptions_UserId (EndDate DESC) | Subscription history | Partial |

---

## 4. Missing Indexes (Recommended)

| Priority | Table | Proposed Index | Hot Path |
|----------|-------|----------------|----------|
| **P0** | Addresses | `(Latitude, Longitude)` WHERE lat/long NOT NULL | Nearby craftsman search |
| **P0** | ServiceRequests | `(CraftsmanId, Status, ScheduledAt)` | Craftsman booking queue |
| **P1** | Users | Unique filtered Email/Phone WHERE IsDeleted=0 | Registration, login |
| **P1** | BookingPayments | `(Status, CreatedAt)` | Admin payments list |
| **P1** | UserSubscriptions | `(UserId, Status)` INCLUDE EndDate | Subscription validation |
| **P2** | Coupons | `(Code, IsActive)` WHERE IsDeleted=0 | Coupon validation |
| **P2** | Notifications | `(UserId, IsRead, CreatedAt DESC)` | Notification inbox |
| **P2** | CraftsmanProfiles | `(IsAvailable, Rating DESC)` | Provider search ranking |

---

## 5. Duplicate / Redundant Indexes

| Issue | Detail | Action |
|-------|--------|--------|
| SQL vs EF duplicate names | Same logical indexes, different filter predicates | Use EF only; archive SQL 003 |
| ServiceRequests single-column indexes | CustomerId, CraftsmanId, Status separate | Consider composite for admin list queries |
| LoginHistory Email + UserId indexes | Both used for different queries | Keep both |

---

## 6. Filtered Indexes

| Index | Filter | Status |
|-------|--------|--------|
| BookingSlotReservations (CraftsmanId, SlotStart) | `IsActive=1 AND IsDeleted=0` | ✅ EF only |
| SQL script indexes | `Deleted=0` on most tables | ❌ Not in EF |

**Gap:** EF unfiltered unique on Email/Phone allows duplicate emails across soft-deleted rows — may be intentional for re-registration.

---

## 7. Composite Indexes

| Existing Composite | Covers |
|--------------------|--------|
| (CraftsmanId, DayOfWeek) | Working hours lookup |
| (UserId, IsRead) | Unread notifications |
| (ConversationId, SentAt) | Chat message pagination |
| (UserId, PermissionId) | Permission checks |
| (PlanId, Cycle) | Billing option lookup |
| (UserId, Status) on VerificationDocuments | Admin verification queue |

**Missing composites:** `(ServiceId, CraftsmanId)` on CraftsmanServices for booking wizard; `(Status, ScheduledAt)` on ServiceRequests for admin dashboard.

---

## 8. Hot Path Query Analysis

### 8.1 Booking Search

| Query | Service | Tables | Index Use | Risk |
|-------|---------|--------|-----------|------|
| List my bookings | BookingService | ServiceRequests | CustomerId/CraftsmanId | Low |
| Admin list + filter | AdminService | ServiceRequests | Status, ScheduledAt | Medium — full scan at scale |
| Nearby craftsmen | BookingService | Addresses, CraftsmanProfiles, CraftsmanServices | **No geo index** | **High** — table scan on addresses |

### 8.2 Provider Search

| Query | Index | Risk |
|-------|-------|------|
| By serviceId | CraftsmanServices.ServiceId | Low |
| By geo + radius | Addresses lat/long | **High** without geo index |
| Availability slots | BookingSlotReservations filtered unique | Low — protected |
| Rating sort | None in EF | Medium |

### 8.3 Authentication

| Query | Index | Risk |
|-------|-------|------|
| Login by email | Users.Email unique | Low |
| Refresh token | RefreshTokens.Token unique | Low |
| Session list | RefreshTokens.UserId | Low |
| OTP lookup | PhoneOtpTokens (UserId, Phone) | Low |

### 8.4 Subscription Validation

| Query | Index | Risk |
|-------|-------|------|
| Current subscription | UserSubscriptions.UserId | Medium — no Status filter in index |
| Plan by code | SubscriptionPlans.PlanCode | Low |
| Public plans by role | SubscriptionPlans.TargetRole, Status | Low |

### 8.5 Payment Lookup

| Query | Index | Risk |
|-------|-------|------|
| Payment by booking | BookingPayments.ServiceRequestId unique | Low |
| Admin payment list | No Status index | Medium |
| Webhook idempotency | Transaction ref — **no unique index** | **High** — duplicate payment risk |

### 8.6 Admin Dashboards

| Query | Tables | Risk |
|-------|--------|------|
| KPI aggregates | Users, ServiceRequests, UserSubscriptions, Complaints | **High** — multiple COUNT(*) without covering indexes |
| Analytics by month | ServiceRequests, BookingPayments | Medium |
| Generic module list | Per-module | Medium — pagination helps |

---

## 9. High Traffic Query Risks

| Risk ID | Path | Issue | Phase |
|---------|------|-------|-------|
| IDX-01 | Nearby craftsmen | No geo index on Addresses | Phase 1 |
| IDX-02 | Slot reservation | No rowversion — race on concurrent bookings | Phase 1 |
| IDX-03 | Payment webhook | No unique on transaction reference | Phase 1 |
| IDX-04 | Admin dashboard | Heavy aggregates without read replicas | Phase 2 |
| IDX-05 | Soft-delete uniques | Email reuse edge cases | Phase 2 |
| IDX-06 | Notification fan-out | Insert-heavy on Notifications | Phase 3 |

---

## 10. Related Documents

- [DATABASE_ER_MODEL.md](./DATABASE_ER_MODEL.md)
- [DATABASE_OBJECT_INVENTORY.md](./DATABASE_OBJECT_INVENTORY.md)
- [SERVICE_RESPONSIBILITY_REPORT.md](./SERVICE_RESPONSIBILITY_REPORT.md)
