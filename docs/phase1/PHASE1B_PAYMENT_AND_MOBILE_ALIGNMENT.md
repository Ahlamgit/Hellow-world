# Phase 1B — Payment & Mobile Architecture Alignment Checkpoint

**Status:** Architecture review — **no code changes**  
**Date:** 2026-07-23  
**Purpose:** Align payment gateway direction and mobile strategy before Phase 1C (API consolidation)  
**Prerequisite:** Phase 1B verified and accepted

---

## Executive summary

| Area | Finding | Recommendation |
|------|---------|----------------|
| **Production payment gateway** | Codebase implements **Moyasar** + **Development** fallback. **Areeba is not present** anywhere in the repository. | Treat Moyasar as interim; plan **Areeba** as target production gateway behind existing `IPaymentGateway` abstraction. Do **not** remove Moyasar code yet. |
| **Mobile payments** | Android/iOS skip hosted checkout and auto-confirm — works only with `Development` gateway. | Fix is **business-critical** for any real gateway; defer to payment alignment wave, not Phase 1C. |
| **Phase 1C** | Profile API consolidation (`/profile` vs `/users/me` vs `/auth/me`) — affects all clients. | **Complete payment architecture alignment first**, then proceed with Phase 1C backend tasks; coordinate client migrations. |

**Checkpoint decision:** **STOP — do not start Phase 1C implementation until payment architecture alignment is approved.**

---

# 1. Payment gateway architecture review

## 1.1 Business target vs current implementation

| | Business target | Current codebase |
|--|-----------------|------------------|
| **Production gateway** | **Areeba** (official) | **Moyasar** (`Payment:Provider = Moyasar` in `appsettings.Production.json`) |
| **Areeba references** | Required | **None** — no classes, config keys, docs, or tests |

The existing `IPaymentGateway` abstraction is the correct integration point for Areeba. Moyasar was implemented as the first production PSP and should be migrated, not extended indefinitely.

---

## 1.2 Existing payment gateway integrations

### Gateway inventory

| Gateway | Class | `ProviderName` | Config switch | Classification |
|---------|-------|----------------|---------------|----------------|
| **Development** | `DevelopmentPaymentGateway` | `Development` | `Payment:Provider` default / Staging | **Keep** — local dev and CI |
| **Moyasar** | `MoyasarPaymentGateway` | `Moyasar` | `Payment:Provider = Moyasar` | **Replace** → Areeba in production |
| **Moyasar (fallback)** | Embedded in `MoyasarPaymentGateway` | — | When `SecretKey` empty | **Disable** in production via config enforcement |
| **Areeba** | — | — | — | **Not implemented** — target gateway |

**Not found:** Stripe, PayPal, Areeba, escrow providers, or subscription billing gateways.

### Provider registration

```csharp
// Khadamati.Infrastructure/DependencyInjection.cs — RegisterPaymentProvider()
Payment:Provider → "moyasar" | default → DevelopmentPaymentGateway
```

`IntegrationReadinessService` only recognizes `Development` and `Moyasar` as valid providers.

---

## 1.3 Controllers, services, and classes

### Abstraction layer

| Component | Path | Role |
|-----------|------|------|
| `IPaymentGateway` | `Khadamati.Application/Interfaces/IPaymentGateway.cs` | Create session + verify payment |
| `IPaymentWebhookService` | `Khadamati.Application/Interfaces/IPaymentWebhookService.cs` | Process provider webhooks |
| `IBookingService` | `Khadamati.Application/Interfaces/IBookingService.cs` | `InitiatePaymentAsync`, `ConfirmPaymentAsync`, `ConfirmPaymentFromWebhookAsync` |

### API controllers

| Controller | Route | Auth | Purpose |
|------------|-------|------|---------|
| `BookingsController` | `POST /api/v1/bookings/{id}/payment` | Bearer (customer) | Initiate payment session |
| `BookingsController` | `POST /api/v1/bookings/{id}/payment/confirm` | Bearer (customer) | Client-side payment confirmation |
| `MoyasarWebhookController` | `POST /api/v1/webhooks/moyasar` | Anonymous | Moyasar webhook callback |
| `AdminPaymentsController` | `GET /api/v1/admin/payments/{id}` | Admin | Payment detail (read-only) |

**Note:** `MoyasarWebhookController` lives in `AdminLocationsController.cs` (file naming mismatch).

### Services

| Service | Path | Role |
|---------|------|------|
| `BookingService` | `Infrastructure/Services/BookingService.cs` | Payment lifecycle, slot reservation on completion |
| `MoyasarPaymentGateway` | `Infrastructure/Services/Payments/MoyasarPaymentGateway.cs` | Moyasar invoice API |
| `DevelopmentPaymentGateway` | `Infrastructure/Services/Payments/DevelopmentPaymentGateway.cs` | Fake `KHD-*` sessions |
| `PaymentWebhookService` | `Infrastructure/Services/Payments/PaymentWebhookService.cs` | HMAC verification + booking confirmation |
| `AdminService` | `Infrastructure/Services/AdminService.cs` | Admin payment list/export |
| `IntegrationReadinessService` | `Infrastructure/Services/Integrations/IntegrationReadinessService.cs` | Moyasar config readiness checks |

### MediatR commands

| Command | Path |
|---------|------|
| `InitiatePaymentCommand` | `Features/Bookings/Commands/BookingCommands.cs` |
| `ConfirmPaymentCommand` | `Features/Bookings/Commands/BookingCommands.cs` |
| `ProcessMoyasarWebhookCommand` | `Features/Payments/Commands/PaymentWebhookCommands.cs` |

### DTOs and validators

| File | Contents |
|------|----------|
| `DTOs/Payments/PaymentDtos.cs` | `PaymentSessionDto`, `PaymentVerificationResult`, coupon DTOs |
| `DTOs/Payments/PaymentWebhookDtos.cs` | `MoyasarWebhookDto`, `PaymentWebhookResultDto` |
| `DTOs/Bookings/BookingDtos.cs` | `InitiatePaymentDto`, `ConfirmPaymentDto`, `BookingPaymentDto` |
| `Validators/PaymentValidators.cs` | `MoyasarWebhookValidator`, `ProcessMoyasarWebhookCommandValidator` |
| `Validators/BookingValidators.cs` | `InitiatePaymentValidator`, `ConfirmPaymentValidator` |

---

## 1.4 Database tables related to payments

### Active schema (EF Core)

| Table | Entity | Migration | Purpose |
|-------|--------|-----------|---------|
| `BookingPayments` | `BookingPayment` | `20260708065052_BookingModule` | 1:1 payment record per booking |

**`BookingPayment` columns:**

| Column | Notes |
|--------|-------|
| `ServiceRequestId` | FK to booking (unique) |
| `PayerUserId` / `PayeeUserId` | Customer pays craftsman directly |
| `Amount`, `Currency` | USD default via `PlatformDefaults` |
| `Status` | `PaymentStatus` enum |
| `PaymentMethod` | Card, Mada, ApplePay, Cash (validator) |
| `TransactionReference` | Moyasar invoice ID or `KHD-*` dev session |
| `PaidAt`, `FailureReason` | Completion metadata |

**`ServiceRequests` payment fields:** `PaymentDueAt` (30-minute payment window after confirm).

### Legacy schema (not used by EF app)

| Artifact | Path | Status |
|----------|------|--------|
| `dbo.Payments` | `src/database/002_CreateTables.sql` | **Legacy** — superseded by `BookingPayments` |
| `ref.PaymentStatuses` | `src/database/` | **Legacy** reference data |

### Subscription metadata (not payment processing)

`SubscriptionPlan.PaymentRequired` / `PaymentMethods` — metadata only; no gateway integration for subscriptions.

---

## 1.5 Payment status flows

### Booking state machine

```
Pending → AwaitingPayment → PaymentConfirmed → PendingCraftsmanConfirmation → Confirmed → ...
Rescheduled → AwaitingPayment (re-payment required)
```

**Source:** `Khadamati.Domain/Common/BookingStateMachine.cs`

### Payment record lifecycle

```
Pending → Processing (session created) → Completed (verified)
         ↘ Failed / Cancelled / Refunded (enum exists; refund logic not implemented)
```

### End-to-end flow (current)

```
Customer confirms booking
    → POST /bookings/{id}/confirm
    → Status: AwaitingPayment (PaymentDueAt set)

Customer initiates payment
    → POST /bookings/{id}/payment
    → IPaymentGateway.CreateSessionAsync
    → BookingPayment: Processing
    → Returns checkoutUrl + sessionId

Payment completion (two paths):
    A) Client: POST /bookings/{id}/payment/confirm { sessionId }
    B) Webhook: POST /webhooks/moyasar (HMAC-verified, Phase 1B)

Finalize (BookingService.FinalizePaymentAsync):
    → IPaymentGateway.VerifyAsync
    → BookingPayment: Completed
    → Slot reserved (unique index)
    → Booking: PendingCraftsmanConfirmation
    → Notify craftsman
```

### Escrow

**Not implemented.** `PayeeUserId` is set to the craftsman at payment creation. There is no hold, release, or payout workflow.

### Refunds

`PaymentStatus.Refunded` exists in the enum. No refund service, admin action, or gateway API call is implemented.

---

## 1.6 Webhook handling

| Item | Current implementation |
|------|------------------------|
| **Endpoint** | `POST /api/v1/webhooks/moyasar` |
| **Auth** | `[AllowAnonymous]` |
| **Body** | Raw JSON read before deserialize (required for HMAC) |
| **Signature headers** | `X-Moyasar-Signature` or `X-Webhook-Secret` |
| **Verification** | HMAC-SHA256 over raw body (Phase 1B) |
| **Fail-closed** | Production/Staging require `Payment:Moyasar:WebhookSecret` |
| **Accepted statuses** | `paid`, `captured`, `success` |
| **Reference resolution** | `InvoiceId` → metadata `session_id` / `payment_id` → `Id` |

**Areeba implication:** New webhook endpoint (e.g. `/webhooks/areeba`) or provider-agnostic `/webhooks/payments/{provider}` with separate verification logic. Moyasar endpoint must remain until migration completes.

---

## 1.7 Configuration keys

| Key | Dev default | Production (`appsettings.Production.json`) |
|-----|-------------|---------------------------------------------|
| `Payment:Provider` | `Development` | `Moyasar` |
| `Payment:CheckoutBaseUrl` | `http://localhost:5173/pay` | `https://khadamati.com/pay` |
| `Payment:Moyasar:SecretKey` | `""` | `${MOYASAR_SECRET_KEY}` |
| `Payment:Moyasar:PublishableKey` | `""` | `${MOYASAR_PUBLISHABLE_KEY}` |
| `Payment:Moyasar:WebhookSecret` | `""` | `${MOYASAR_WEBHOOK_SECRET}` |
| `Payment:Moyasar:ApiBaseUrl` | `https://api.moyasar.com/v1` | same |
| `Payment:Moyasar:CallbackUrl` | `""` | `https://api.khadamati.com/api/v1/webhooks/moyasar` |
| `Payment:Moyasar:SuccessUrl` | `""` | `https://khadamati.com/pay/success` |

**Environment variables:** `Payment__Provider`, `Payment__Moyasar__*` (see `.env.production.example`, `docker-compose.prod.yml`).

**Areeba keys (proposed, not in codebase):**

| Proposed key | Purpose |
|--------------|---------|
| `Payment:Areeba:MerchantId` | Merchant identifier |
| `Payment:Areeba:ApiKey` / `SecretKey` | API authentication |
| `Payment:Areeba:WebhookSecret` | Webhook HMAC/signature |
| `Payment:Areeba:ApiBaseUrl` | Areeba API base URL |
| `Payment:Areeba:SuccessUrl` / `CancelUrl` | Redirect URLs |
| `Payment:Areeba:CallbackUrl` | Server webhook URL |

---

## 1.8 Tests

| Test file | Coverage |
|-----------|----------|
| `PaymentWebhookServiceTests.cs` | HMAC valid/invalid, dev no-secret, pending ignored |
| `BookingServiceTests.cs` | Payment confirm, slot reservation, conflict |
| `IntegrationReadinessServiceTests.cs` | Moyasar configured/misconfigured |
| `BookingValidatorTests.cs` | Payment method validation |

**Gaps:** No `MoyasarPaymentGateway` unit tests; no Areeba tests; no E2E payment tests; no mobile payment integration tests.

---

## 1.9 Client implementations

### Web (`src/web`)

| File | Role |
|------|------|
| `pages/BookingPages.tsx` | `BookingPaymentPage`, `PaymentCheckoutPage` (`/pay`) |
| `services/api.ts` | `initiatePayment`, `confirmPayment` |
| `admin/AdminPaymentsPage.tsx` | Admin payment list |

**Status:** Partially production-ready — opens hosted `checkoutUrl`, manual confirm step; webhook can complete asynchronously.

### Android (`src/android`)

| File | Role |
|------|------|
| `ApiService.kt` | `initiatePayment`, `confirmPayment` |
| `BookingRepository.kt` | **Initiate then immediately confirm** (no checkout UI) |
| `BookingScreens.kt` | "Pay Now" button |

**Status:** **Not production-ready** for Moyasar/Areeba — skips hosted checkout.

### iOS (`src/ios`)

| File | Role |
|------|------|
| `APIEndpoints.swift` | Payment URLs |
| `BookingViewModel.swift` | Same initiate→confirm pattern as Android |
| `BookingDetailView.swift` | "Pay Now" button |

**Status:** **Not production-ready** — same gap as Android.

---

## 1.10 Gateway classification summary

| Gateway / component | Classification | Rationale |
|---------------------|----------------|-----------|
| `DevelopmentPaymentGateway` | **Keep** | Required for local dev, integration tests, staging without PSP credentials |
| `MoyasarPaymentGateway` | **Replace** | Interim production PSP; target is Areeba |
| `MoyasarWebhookController` | **Replace** | Moyasar-specific; add Areeba webhook alongside |
| `PaymentWebhookService` (Moyasar logic) | **Replace** | Generalize to provider-specific handlers behind `IPaymentWebhookService` |
| `IPaymentGateway` | **Keep** | Correct abstraction — add `AreebaPaymentGateway` |
| `BookingService` payment methods | **Keep** | Gateway-agnostic orchestration |
| `BookingPayments` table | **Keep** | Provider-agnostic; `TransactionReference` stores external ID |
| `POST /bookings/{id}/payment*` | **Keep** | Stable API contract for all clients |
| Legacy `dbo.Payments` SQL | **Remove later** | Not used by EF; cleanup in future DB hygiene wave |
| `PaymentStatus.Refunded` enum | **Keep** | Reserve for future refund implementation |

**Do not remove Moyasar code until Areeba is validated in staging and booking flows are regression-tested.**

---

## 1.11 Recommended final payment architecture

```
Customer (Web / Mobile)
        ↓
POST /api/v1/bookings/{id}/payment
        ↓
BookingService (orchestration — unchanged)
        ↓
IPaymentGateway  ←── provider selected by Payment:Provider
        ↓
Areeba Gateway (production)
        ↓
Hosted checkout / redirect (customer completes payment)
        ↓
Webhook: POST /api/v1/webhooks/areeba
        ↓
Webhook verification (HMAC / Areeba signature scheme)
        ↓
BookingService.ConfirmPaymentFromWebhookAsync
        ↓
BookingPayment.Status = Completed
Booking.Status = PendingCraftsmanConfirmation
Slot reserved (filtered unique index)
        ↓
(Optional future) Escrow / payout release — not in Phase 1
```

### Design principles

1. **Keep `IPaymentGateway`** — add `AreebaPaymentGateway`; Moyasar remains registered until cutover.
2. **Keep `BookingPayments` schema** — store Areeba transaction/reference IDs in `TransactionReference`.
3. **Provider-agnostic booking API** — clients never call Areeba directly; they use existing initiate/confirm endpoints.
4. **Webhook per provider** — separate controllers or a dispatcher with provider-specific signature validation.
5. **Development gateway unchanged** — `Payment:Provider=Development` for local/CI.
6. **Mobile must open hosted checkout** — WebView/Safari/deep link before calling confirm (or rely on webhook-only completion).

### Escrow (future, out of Phase 1)

Current model pays craftsman directly (`PayeeUserId`). If Areeba supports hold/capture or marketplace payouts, escrow can be added as a **separate phase** without changing the booking payment API surface.

---

## 1.12 Migration steps (Moyasar → Areeba)

**No code removal in this checkpoint.** Proposed migration sequence for a future implementation wave:

| Step | Action | Risk |
|------|--------|------|
| 1 | Obtain Areeba API docs, sandbox credentials, webhook signature spec | — |
| 2 | Implement `AreebaPaymentGateway : IPaymentGateway` | Low |
| 3 | Add `AreebaWebhookController` + `AreebaWebhookService` (or generalize existing) | Medium |
| 4 | Extend `RegisterPaymentProvider()` — `case "areeba"` | Low |
| 5 | Update `IntegrationReadinessService` for Areeba keys | Low |
| 6 | Add `appsettings.Production.json` Areeba section; set `Payment:Provider=Areeba` in staging first | Medium |
| 7 | Fix mobile checkout UX (WebView + return URL / webhook-only) | **High** — blocks real payments |
| 8 | Parallel run: Moyasar + Areeba in staging; compare webhook → booking completion | Medium |
| 9 | Production cutover: `Payment:Provider=Areeba`; keep Moyasar webhook active for in-flight transactions | Medium |
| 10 | Deprecation window (30 days): Moyasar webhook still processes old invoices | Low |
| 11 | **Remove later:** Moyasar gateway, webhook controller, config keys, docs references | Low (after validation) |

### Breaking-change avoidance

- Keep `POST /bookings/{id}/payment` and `/payment/confirm` response shape.
- Keep `BookingPaymentDto` fields (`sessionId`, `checkoutUrl`, `transactionReference`).
- Run both webhooks during overlap for in-progress Moyasar invoices.
- Do not change `BookingPayments` table structure.

---

# 2. Mobile technology alignment

## 2.1 Current Phase 1 (confirmed)

| Platform | Status | Allowed changes |
|----------|--------|-----------------|
| **Android (Kotlin + Compose)** | Active production candidate | Critical fixes only (e.g. Phase 1B change-password) |
| **iOS (Swift + SwiftUI)** | Active production candidate | Critical fixes only |
| **React Native** | **Not started** | Documentation in Phase 2; implementation in Phase 3 |

Per `docs/technology/KHADAMATI_FINAL_TECHNOLOGY_ARCHITECTURE.md`:

- Native apps remain until React Native reaches feature parity.
- No React Native project bootstrap during Phase 1.
- Admin portal stays web-only permanently.

## 2.2 Future target: React Native + TypeScript

| Milestone | When | Deliverable |
|-----------|------|-------------|
| Phase 2 | After Phase 1 complete | `REACT_NATIVE_MIGRATION_ARCHITECTURE.md` |
| Phase 3 | After Phase 2 approval | Build RN app, parity checklist, beta, decommission native |

**Decommission criteria:** RN at 100% feature parity → internal dogfood → store beta → production release → 90-day native freeze → remove Kotlin/Swift apps.

## 2.3 Alignment rules

1. **Existing native applications remain production candidates** until RN migration is validated.
2. **No parallel feature development** in Kotlin/Swift unless business-critical (security, payment blocking, data integrity).
3. **React Native migration starts only after:**
   - Backend stabilization (Phase 1 complete)
   - API contract freeze (especially profile + payment endpoints)
   - Feature matrix approval (`docs/technology/MOBILE_MIGRATION_PLAN.md` parity checklist)
4. **Payment mobile fix** (hosted checkout) is **business-critical** and should be addressed in the payment alignment wave — either in native apps (minimal WebView) or deferred to RN if timeline allows (not recommended for production launch).

## 2.4 Client API usage today

| Concern | Android / iOS | Web |
|---------|---------------|-----|
| Profile | `GET/PUT /users/me` | `GET/PUT /profile` **and** `/users/me` (duplicate) |
| Auth summary | Not used | `/auth/me` |
| Payments | `POST .../payment` + immediate confirm | Initiate → open checkout → confirm |
| Change password | Phase 1B wired | `/auth/change-password` |

Phase 1C will consolidate profile endpoints — native apps already use `/users/me` (canonical target).

---

# 3. Phase 1C impact review

## 3.1 Planned Phase 1C scope (from `IMPLEMENTATION_PLAN_PHASE1.md`)

| Task | Scope |
|------|-------|
| **1C-01** | Unified `UserProfileDto` (merge `ProfileDto` + `UserProfileDto`) |
| **1C-02** | Single `IUserProfileService` |
| **1C-03** | Canonical `GET/PUT /users/me`; deprecate `/profile`; slim `/auth/me` |
| **1C-04** | Client migration: web → Android → iOS |

## 3.2 Impact assessment

| Dimension | Impact | Details |
|-----------|--------|---------|
| **Android/iOS native apps** | **Medium** | Already use `/users/me` — low change for 1C-04. Must regression-test profile edit, addresses, auth flows. |
| **Web app** | **High** | Uses `/profile` as primary — requires migration to `/users/me`. |
| **React Native migration** | **Positive** | Single canonical profile API reduces future RN client complexity. RN should target `/users/me` only. |
| **Payment abstraction** | **None** | Phase 1C is profile-only; does not touch `IPaymentGateway` or booking payment endpoints. |
| **Authentication contracts** | **Low–Medium** | `/auth/me` may be slimmed to permissions/session summary. Login, refresh, revoke, change-password **unchanged**. JWT claims unaffected. |
| **Payment gateway migration** | **Indirect** | Both Phase 1C-04 and payment mobile fix require coordinated client releases. Doing both simultaneously increases release risk. |

## 3.3 Conflicts and dependencies

| Conflict | Severity | Notes |
|----------|----------|-------|
| Moyasar in prod config vs Areeba business target | **High** | Business misalignment; not a Phase 1C code conflict but blocks real production payments |
| Mobile payment UX broken for real PSP | **High** | Must fix before production regardless of Phase 1C |
| Triple client migration (1C-04) + payment mobile fix | **Medium** | Two client-touching waves — sequence carefully |
| Profile duplication (`/profile` vs `/users/me`) | **Low** | Technical debt; 1C addresses this |

## 3.4 Recommendation

### Do **not** start Phase 1C implementation yet.

### Complete **payment architecture alignment** first:

1. **Approve** Areeba as production gateway and confirm Areeba sandbox/API access.
2. **Document** Areeba integration spec (session creation, verify, webhook signature) — extend this checkpoint into `PHASE1_PAYMENT_AREEBA_PLAN.md` when ready.
3. **Implement** (future wave, post-approval):
   - `AreebaPaymentGateway` behind `IPaymentGateway`
   - Areeba webhook endpoint
   - Mobile hosted checkout (WebView/deep link) — **business-critical**
4. **Keep Moyasar** running in parallel during staging validation; remove only after Areeba cutover.

### Then proceed with Phase 1C in this order:

| Order | Work | Rationale |
|-------|------|-----------|
| 1 | **1C-01 + 1C-02** (backend DTO + service) | No client impact; reduces duplication |
| 2 | **1C-03** (deprecation headers, dual endpoints) | Backward compatible |
| 3 | **1C-04 web migration** | Highest divergence today |
| 4 | **1C-04 mobile** | Already on `/users/me` — lighter lift |

**Alternative (not recommended):** Run Phase 1C backend (1C-01–03) in parallel with payment design **only if** Areeba credentials are delayed — but still **do not** start 1C-04 client migration until payment mobile path is designed.

---

## Checkpoint sign-off

| Item | Status |
|------|--------|
| Payment gateway review | ✅ Documented |
| Mobile technology alignment | ✅ Confirmed |
| Phase 1C impact review | ✅ Documented |
| Code changes | ❌ None (by design) |
| Phase 1C implementation | **BLOCKED** pending payment alignment approval |

**Next step:** Review and approve this document, then authorize either (a) payment alignment implementation wave or (b) explicit decision to proceed with Phase 1C backend-only tasks while payment design continues.
