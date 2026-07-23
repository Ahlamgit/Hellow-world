# KHADAMATI — Payment Gateway Migration Plan (Moyasar → Areeba)

**Status:** Documentation only — **awaiting approval**  
**Date:** 2026-07-23  
**Scope:** Architecture analysis and phased implementation plan  
**Code changes:** None in this deliverable  
**Prerequisite:** Phase 1A (data integrity) and Phase 1B (security hardening) completed and verified  
**Blocks:** Phase 1C (API consolidation) until payment architecture is approved  

**Continues from:** Phase 1B payment & mobile architecture alignment checkpoint  
(`docs/phase1/PHASE1B_PAYMENT_AND_MOBILE_ALIGNMENT.md` on the Phase 1B branch)

---

## Executive summary

| Finding | Decision |
|---------|----------|
| Production PSP must be **Areeba** | Official business requirement |
| Codebase today uses **Moyasar** + **Development** | Moyasar is interim; keep until Areeba cutover is validated |
| `IPaymentGateway` abstraction already exists | **Reuse** — add `AreebaPaymentGateway`; never call Areeba from domain/application business logic |
| Mobile Android/iOS skip hosted checkout | **Must fix** before any real PSP goes live |
| Refunds / escrow | Enum stubs only — schedule as **future** phases after capture flow is stable |

**Architecture target (approved direction):**

```
IPaymentGateway
        |
        +---- AreebaPaymentGateway      (production)
        |
        +---- DevelopmentPaymentGateway (local / CI / staging without PSP)
```

Moyasar remains registered and deployable during migration overlap, then is removed only after deprecation.

**Stop condition:** This document is for review. Do **not** implement Areeba, change DI, or alter mobile payment UX until this plan is explicitly approved.

---

## 1. Existing Moyasar implementation

### 1.1 What Moyasar does today

| Concern | Implementation |
|---------|----------------|
| Class | `MoyasarPaymentGateway` (`Infrastructure/Services/Payments/`) |
| Interface | `IPaymentGateway` |
| Session creation | `POST https://api.moyasar.com/v1/invoices` (hosted invoice) |
| Auth | HTTP Basic with `Payment:Moyasar:SecretKey` |
| Amount encoding | Minor units (“halalas”) — `amount * 100` |
| Returns | `PaymentSessionDto` with Moyasar invoice `Id` as `SessionId` and invoice `Url` as `CheckoutUrl` |
| Verification | `GET /invoices/{id}` — accepts status `paid` / `captured`; checks amount + currency |
| Unconfigured fallback | If `SecretKey` empty → delegates to `DevelopmentPaymentGateway` |
| Provider switch | `Payment:Provider = Moyasar` in `RegisterPaymentProvider()` |

### 1.2 Moyasar webhook path

| Item | Detail |
|------|--------|
| Endpoint | `POST /api/v1/webhooks/moyasar` |
| Controller | `MoyasarWebhookController` (currently co-located in `AdminLocationsController.cs`) |
| Command | `ProcessMoyasarWebhookCommand` |
| Service | `PaymentWebhookService.ProcessMoyasarWebhookAsync` |
| Signature (Phase 1B) | HMAC-SHA256 over raw body; headers `X-Moyasar-Signature` / `X-Webhook-Secret`; fail-closed in Production/Staging |
| Success statuses | `paid`, `captured`, `success` |
| Side effect | `BookingService.ConfirmPaymentFromWebhookAsync(transactionReference)` |

### 1.3 Configuration surface

| Key | Role |
|-----|------|
| `Payment:Provider` | `Development` \| `Moyasar` (production currently Moyasar) |
| `Payment:CheckoutBaseUrl` | Dev fake checkout page (`/pay`) |
| `Payment:Moyasar:SecretKey` | Invoice API |
| `Payment:Moyasar:PublishableKey` | Readiness / future client use |
| `Payment:Moyasar:WebhookSecret` | Webhook HMAC |
| `Payment:Moyasar:ApiBaseUrl` | Default `https://api.moyasar.com/v1` |
| `Payment:Moyasar:CallbackUrl` | Webhook URL |
| `Payment:Moyasar:SuccessUrl` | Post-checkout redirect |

Documented in `docs/PRODUCTION.md` and `.env.production.example`.

### 1.4 Moyasar strengths to preserve as patterns

1. Hosted checkout URL returned to clients (no card data on KHADAMATI servers).
2. Server-side `VerifyAsync` before marking payment completed.
3. Dual completion paths: client confirm **and** webhook.
4. Metadata binding payment id / session id for webhook correlation.
5. Integration readiness checks for missing secrets.

### 1.5 Moyasar limitations / migration risks

1. **Wrong production PSP** relative to business mandate (Areeba).
2. Silent fallback to Development when secret missing — dangerous if Production misconfigured.
3. Moyasar-specific DTOs/commands (`MoyasarWebhookDto`, `ProcessMoyasarWebhookCommand`) leak provider names into Application layer.
4. Currency historically SAR-oriented in Moyasar “halala” helpers; platform direction is USD/Lebanon — Areeba typically uses USD for Lebanon merchants.
5. No refund API integration despite `PaymentStatus.Refunded`.

---

## 2. Existing payment abstraction

### 2.1 Core contract

```csharp
public interface IPaymentGateway
{
    string ProviderName { get; }
    Task<PaymentSessionDto> CreateSessionAsync(PaymentSessionRequest request, CancellationToken ct = default);
    Task<PaymentVerificationResult> VerifyAsync(string sessionId, decimal expectedAmount, string currency, CancellationToken ct = default);
}
```

| Type | Responsibility |
|------|----------------|
| `PaymentSessionRequest` | PaymentId, Amount, Currency, Description, CustomerEmail, CallbackUrl |
| `PaymentSessionDto` | SessionId, CheckoutUrl, Provider |
| `PaymentVerificationResult` | IsSuccessful, TransactionReference, FailureReason |

### 2.2 Orchestration (gateway-agnostic — keep)

`BookingService` owns the business flow:

```
AwaitingPayment
    → InitiatePaymentAsync
        → create BookingPayment (Pending)
        → IPaymentGateway.CreateSessionAsync
        → store SessionId in TransactionReference
        → status Processing
        → return BookingPaymentDto (sessionId, checkoutUrl, provider)

    → ConfirmPaymentAsync (client) OR ConfirmPaymentFromWebhookAsync
        → FinalizePaymentAsync
            → slot concurrency check
            → IPaymentGateway.VerifyAsync
            → PaymentStatus.Completed
            → PaymentConfirmed → PendingCraftsmanConfirmation
            → slot reservation + craftsman notification
```

**Rule:** Application/Domain layers must continue to depend only on `IPaymentGateway` / booking services — never on `AreebaPaymentGateway`, Moyasar types, or HTTP clients.

### 2.3 Registration pattern (keep, extend)

```csharp
// DependencyInjection.RegisterPaymentProvider
switch (Payment:Provider)
{
    case "moyasar": → MoyasarPaymentGateway   // retain during migration
    case "areeba":  → AreebaPaymentGateway    // NEW
    default:        → DevelopmentPaymentGateway
}
```

### 2.4 What the abstraction does **not** cover yet

| Gap | Impact |
|-----|--------|
| No `RefundAsync` / `CaptureAsync` / `CancelSessionAsync` | Future refund/escrow need interface extension |
| No provider-agnostic webhook interface | Webhook service is Moyasar-named today |
| No persisted `Provider` column on `BookingPayments` | Harder to support dual-gateway overlap & reconciliation |
| Session id vs final transaction id conflated in `TransactionReference` | Areeba order id / session id / uuid differ |

These gaps are addressed in sections 6–8 without requiring domain coupling to Areeba.

---

## 3. Areeba migration architecture

### 3.1 Product choice (decision required at approval)

Areeba exposes more than one integration surface. KHADAMATI should pick **one** for Phase PG-1:

| Option | Base | Flow | Fit with current model |
|--------|------|------|------------------------|
| **A — MPGS / ePayment (recommended default)** | `https://epayment.areeba.com` | `INITIATE_CHECKOUT` session → `checkout.js` / hosted page → Retrieve order | Closest to Moyasar invoice + hosted URL |
| **B — IXOPAY gateway** | `https://gateway.areebapayment.com` | `debit` → `redirectUrl` → status by merchantTransactionId | Also works; different payload/status model |

**Recommendation:** Prefer **Option A (MPGS hosted checkout)** unless merchant boarding confirms IXOPAY-only credentials. Document the chosen option in the approval sign-off before coding.

### 3.2 Target component diagram

```
┌──────────── Web / Android / iOS ────────────┐
│  initiate → open hosted checkout → return   │
│  (confirm optional; webhook authoritative)  │
└─────────────────────┬───────────────────────┘
                      │
        POST /api/v1/bookings/{id}/payment
        POST /api/v1/bookings/{id}/payment/confirm
                      │
              BookingService  (unchanged orchestration)
                      │
                 IPaymentGateway
            ┌─────────┴──────────┐
            │                    │
   AreebaPaymentGateway   DevelopmentPaymentGateway
            │
   Areeba REST (session + order/status)
                      │
        POST /api/v1/webhooks/areeba   (new)
        POST /api/v1/webhooks/moyasar  (keep until deprecation)
                      │
           IPaymentWebhookService (generalized)
                      │
        ConfirmPaymentFromWebhookAsync
```

### 3.3 Proposed `AreebaPaymentGateway` behavior (Option A)

| Method | Areeba mapping |
|--------|----------------|
| `CreateSessionAsync` | `POST /api/rest/version/{v}/merchant/{merchantId}/session` with `apiOperation: INITIATE_CHECKOUT`, order amount/currency/id |
| Store | Map Areeba `session.id` (and/or merchant `order.id`) into payment session fields |
| `CheckoutUrl` | Either gateway-hosted redirect URL **or** KHADAMATI return page that loads `checkout.min.js` with session id (`Checkout.showPaymentPage()`) |
| `VerifyAsync` | `GET .../order/{orderId}` — require successful paid/captured equivalent; match amount + currency |
| Auth | Basic Auth (`merchant.{MerchantId}` + API password) |

### 3.4 Webhook strategy

| Phase | Endpoints |
|-------|-----------|
| Migration | Keep `/webhooks/moyasar`; add `/webhooks/areeba` |
| Optional later | `/webhooks/payments/{provider}` facade routing to provider handlers |
| Verification | Provider-specific signature validation (HMAC or Areeba-documented scheme) — mirror Phase 1B fail-closed rules |
| Completion | Always call gateway-agnostic `ConfirmPaymentFromWebhookAsync` after mapping external id → `TransactionReference` |

### 3.5 Configuration (proposed)

```json
"Payment": {
  "Provider": "Areeba",
  "Areeba": {
    "Product": "Mpgs",
    "MerchantId": "",
    "ApiUsername": "",
    "ApiPassword": "",
    "ApiBaseUrl": "https://epayment.areeba.com",
    "ApiVersion": "100",
    "WebhookSecret": "",
    "CallbackUrl": "https://api.khadamati.com/api/v1/webhooks/areeba",
    "SuccessUrl": "https://khadamati.com/pay/success",
    "CancelUrl": "https://khadamati.com/pay/cancel",
    "Currency": "USD"
  }
}
```

`IntegrationReadinessService` must recognize `Areeba` and validate required keys (same pattern as Moyasar today).

### 3.6 Non-negotiable architecture rules

1. **Business logic never depends on Areeba** — only Infrastructure implements the gateway.
2. **Single active create/verify provider** via `Payment:Provider` (no dual-write of new sessions).
3. **Webhook overlap allowed** — Moyasar webhook may still finalize in-flight Moyasar invoices after cutover.
4. **Clients stay provider-agnostic** — they only consume `checkoutUrl` / `sessionId` / `provider` from `BookingPaymentDto`.
5. **No React Native payment work** in this wave — fix native Kotlin/Swift checkout only.

---

## 4. What to keep / reuse

| Component | Action | Why |
|-----------|--------|-----|
| `IPaymentGateway` | **Keep** | Correct seam for Areeba |
| `DevelopmentPaymentGateway` | **Keep** | Local, CI, staging without live PSP |
| `BookingService` payment orchestration | **Keep** | Already gateway-agnostic |
| `BookingPayments` entity/table | **Keep** | Core ledger; extend carefully (see §6) |
| `PaymentStatus` enum | **Keep** | Includes Refunded for later |
| Booking payment API routes | **Keep** | Stable client contract |
| `BookingPaymentDto` fields | **Keep** | `sessionId`, `checkoutUrl`, `provider`, `transactionReference` |
| Web `BookingPaymentPage` pattern | **Keep & harden** | Already opens checkout URL |
| Phase 1B webhook HMAC pattern | **Reuse** for Areeba signature verification |
| Admin payments read/export | **Keep** | Ops visibility |
| Moyasar code during overlap | **Keep** | In-flight payments + rollback |

---

## 5. What to replace

| Component | Action | Timing |
|-----------|--------|--------|
| Production `Payment:Provider=Moyasar` | **Replace** with `Areeba` | Staging first, then production cutover |
| `MoyasarPaymentGateway` as sole production impl | **Supersede** by `AreebaPaymentGateway` | After staging validation |
| Moyasar-only readiness checks | **Extend** then later remove Moyasar branch | With Areeba readiness |
| `IPaymentWebhookService.ProcessMoyasarWebhookAsync` naming | **Generalize** (e.g. provider handlers) | During Areeba webhook work |
| Mobile initiate→immediate confirm | **Replace** with hosted checkout UX | Same wave as Areeba staging |
| `docs/PRODUCTION.md` Moyasar-as-production | **Update** to Areeba | Cutover docs |
| Moyasar gateway/webhook/config | **Remove later** | After deprecation window (§9) |

**Explicitly do not remove Moyasar before migration validation.**

---

## 6. Database impact

### 6.1 Current physical schema (EF)

Table `BookingPayments`:

| Column | Notes |
|--------|-------|
| `Id` | PK |
| `ServiceRequestId` | Unique FK (1:0..1 payment per booking) |
| `PayerUserId` / `PayeeUserId` | Customer → craftsman (no escrow hold) |
| `Amount` / `Currency` | Precision(18,2); currency length 3 |
| `Status` | int enum |
| `PaymentMethod` | nvarchar(50) |
| `TransactionReference` | nvarchar(200) — currently stores gateway session/invoice id |
| `PaidAt` / `FailureReason` | Completion metadata |

Indexes today: `ServiceRequestId` (unique), payer/payee FKs.  
**Gap:** No dedicated index on `TransactionReference` (webhook lookup is equality scan — add filtered unique index in migration wave).

Legacy SQL `dbo.Payments` / `ref.PaymentStatuses` remain unused by EF — out of scope except documentation hygiene.

### 6.2 Required for Areeba cutover (minimal)

Prefer **minimal schema change** for first Areeba release:

| Change | Required? | Rationale |
|--------|-----------|-----------|
| Filtered unique index on `TransactionReference` WHERE NOT NULL | **Yes (recommended)** | Webhook idempotency + lookup performance |
| Persist provider name | **Recommended** | Dual-gateway overlap & support |
| Separate gateway session vs order ids | **Recommended if Areeba needs both** | Avoid overloading one column |

#### Proposed additive columns (EF migration; must match SQL Server)

| Column | Type | Purpose |
|--------|------|---------|
| `Provider` | `nvarchar(50)` NULL → default `'Development'` then backfill | Which gateway created the session |
| `GatewaySessionId` | `nvarchar(200)` NULL | Areeba session id / Moyasar invoice id |
| `GatewayOrderId` | `nvarchar(200)` NULL | Areeba order id used for Retrieve Order |
| `TransactionReference` | keep | Final authoritative paid reference (may equal order id) |

If columns are deferred, store Areeba order id in `TransactionReference` at session creation and ensure Verify/Webhook use the same id — **document the convention** and add the index anyway.

### 6.3 Explicitly out of scope for first cutover

| Change | Phase |
|--------|-------|
| Escrow / payout tables | Future (§8) |
| Refund ledger tables | Future (§8) |
| Dropping Moyasar-related columns (none today) | N/A |
| Migrating legacy `dbo.Payments` | Separate DB hygiene |

### 6.4 Migration rules

1. Additive, expandable migrations only.
2. EF migrations must match physical SQL Server schema (Phase 1A discipline).
3. Backfill `Provider` for existing rows (`Moyasar` / `Development` inferred from `TransactionReference` prefix `KHD-` when possible).
4. No destructive drops until Moyasar deprecation closes.

---

## 7. Mobile payment flow correction

### 7.1 Current broken flow (Android / iOS)

```
Pay Now
  → POST /bookings/{id}/payment
  → immediately POST /bookings/{id}/payment/confirm { sessionId }
  → never opens checkoutUrl
```

This only “works” when `DevelopmentPaymentGateway.VerifyAsync` accepts any `KHD-*` session. Against Moyasar/Areeba, confirm either fails verification or falsely depends on unpaid sessions.

| Client | Files | Status |
|--------|-------|--------|
| Android | `BookingRepository.pay`, `BookingScreens` | Not production-ready |
| iOS | `BookingViewModel.pay`, `BookingDetailView` | Not production-ready |
| Web | `BookingPaymentPage` | Partially ready (opens checkout; still has manual confirm) |

### 7.2 Target production flow (all clients)

```
1. POST /payment → receive sessionId + checkoutUrl + provider
2. Open hosted checkout:
     Web: redirect / new tab / checkout.js page
     Android: Custom Tabs or WebView with return intent
     iOS: SFSafariViewController / ASWebAuthenticationSession / WKWebView
3. Customer completes payment on Areeba-hosted UI
4. Return via SuccessUrl deep link / app link
5. Completion (prefer both):
     a) Webhook → ConfirmPaymentFromWebhookAsync (authoritative)
     b) Client may call /payment/confirm after return (idempotent VerifyAsync)
6. Poll GET booking until status leaves AwaitingPayment (UX)
```

### 7.3 Rules for native fix (Phase PG — business-critical)

1. **Never** call confirm immediately after initiate.
2. Always present / open `checkoutUrl` when provider ≠ Development (Development may keep simulated `/pay` page).
3. Deep links: e.g. `khadamati://pay/success?bookingId=...&session=...` and HTTPS app links mirroring SuccessUrl.
4. Treat webhook as source of truth; client confirm is convenience + offline-webhook resilience.
5. Show clear pending / failed / cancelled UI; do not mark paid locally without API status.
6. **No React Native** implementation in this wave.

### 7.4 Web hardening (same wave)

1. Prefer redirect to hosted checkout over “open in new tab + manual confirm”.
2. Success page should trigger confirm **or** poll booking status after webhook.
3. Hide Development-only `/pay` simulator when provider is Areeba/Moyasar.

---

## 8. Refund and escrow — future requirements

### 8.1 Current state

| Capability | Status |
|------------|--------|
| `PaymentStatus.Refunded` | Enum only |
| Admin refund action | Not implemented |
| Gateway refund API | Not implemented |
| Escrow hold / release | Not implemented |
| Payout to craftsman | `PayeeUserId` recorded only; funds path is PSP merchant account today |

### 8.2 Refund (future phase — after Areeba capture stable)

**Business needs (expected):**

- Full and partial refunds on cancelled / disputed bookings.
- Admin-initiated refund with audit trail.
- Customer-visible refund status.
- Idempotent refund requests.

**Architecture sketch (do not implement now):**

```
IPaymentGateway
  + RefundAsync(RefundRequest) → RefundResult

BookingPayment / PaymentRefunds table
  - Original payment FK, amount, reason, gateway refund id, status, actor

Admin API: POST /admin/payments/{id}/refund
```

**Areeba note:** Confirm refund/void API for the chosen product (MPGS order refund vs IXOPAY credit). Extend interface only when sandbox proves capability.

### 8.3 Escrow / delayed payout (future phase)

**Business needs (expected for marketplace):**

- Hold customer funds until service completion (or milestone).
- Release to craftsman on `Completed`.
- Partial release / clawback on disputes.
- Platform commission deduction.

**Architecture sketch (do not implement now):**

```
PaymentIntent / EscrowHold
  - BookingPaymentId
  - HeldAmount, PlatformFee, CraftsmanAmount
  - Status: Held | Released | Refunded | Disputed

IPaymentGateway or IPayoutGateway
  - Capture / Transfer / Release APIs (provider-specific in Infrastructure)
```

**Constraint:** Escrow may require Areeba merchant account features (split settlements / marketplace). Validate commercially before schema design lock.

### 8.4 Sequencing relative to Areeba

```
PG-0 Docs approval (this document)
 → PG-1 Areeba capture + webhooks + mobile checkout
 → PG-2 Reconciliation & ops tooling
 → PG-3 Refunds
 → PG-4 Escrow / payouts
```

Do **not** block Areeba launch on escrow.

---

## 9. Implementation phases

### Phase PG-0 — Approval gate (this deliverable)

| Deliverable | Owner |
|-------------|-------|
| This migration plan | Engineering |
| Confirm Areeba product (MPGS vs IXOPAY) | Product + merchant ops |
| Obtain sandbox credentials + webhook signature spec | Ops |
| Explicit approval to start PG-1 | Stakeholders |

**Exit:** Written approval of architecture target and product choice.

---

### Phase PG-1 — Areeba integration (backend + config)

| Task | Notes |
|------|-------|
| Implement `AreebaPaymentGateway : IPaymentGateway` | Infrastructure only |
| Register `case "areeba"` in DI | Keep Moyasar case |
| Extend `IntegrationReadinessService` | Required Areeba keys |
| Add Areeba webhook endpoint + HMAC/signature validation | Fail-closed in Staging/Production |
| Generalize webhook service surface without breaking Moyasar | Dual endpoints |
| appsettings + `.env.*.example` + PRODUCTION.md | Document secrets |
| Unit tests for Areeba gateway (mocked HTTP) + webhook tests | Mirror Phase 1B coverage |
| Optional DB: `Provider` + index on `TransactionReference` | Additive migration |

**Exit:** Staging can create Areeba session, complete sandbox payment, webhook finalizes booking.

---

### Phase PG-2 — Mobile & web checkout correction

| Task | Notes |
|------|-------|
| Android: Custom Tabs/WebView + return deep link | Stop auto-confirm |
| iOS: Safari VC / auth session + return URL | Stop auto-confirm |
| Web: redirect-first checkout + status poll | Harden success path |
| Idempotent confirm after return | Safe if webhook already completed |
| Manual QA matrix (Development + Areeba sandbox) | All three clients |

**Exit:** Real hosted checkout works on Web, Android, iOS against Areeba sandbox.

---

### Phase PG-3 — Parallel validation & cutover

| Step | Action |
|------|--------|
| 1 | Staging `Payment:Provider=Areeba` soak |
| 2 | Regression: booking integrity, concurrency, coupons (Phase 1A) |
| 3 | Production cutover: switch provider to Areeba |
| 4 | Keep Moyasar webhook live for in-flight invoices |
| 5 | Monitor: payment success rate, webhook failures, awaiting-payment timeouts |
| 6 | Rollback plan: set `Payment:Provider=Moyasar` if Areeba outage (code still present) |

**Exit:** Production bookings pay via Areeba; no open Moyasar invoices remaining (or aged past TTL).

---

### Phase PG-4 — Moyasar deprecation

| Task | Condition |
|------|-----------|
| Disable Moyasar DI registration | No pending Moyasar `Processing` payments |
| Remove Moyasar gateway, webhook, DTOs, config keys | After agreed window (e.g. 30 days) |
| Update readiness tests & docs | Areeba-only production story |
| Archive Moyasar secrets from vault | Rotation/removal per security runbook |

**Exit:** Codebase matches architecture target (Areeba + Development only).

---

### Phase PG-5 — Refunds (future)

Design + implement refund API, admin UX, Areeba refund calls, ledger rows.  
**Depends on:** PG-3 stable.

### Phase PG-6 — Escrow / payouts (future)

Commercial validation with Areeba + schema + release workflow tied to booking completion.  
**Depends on:** PG-5 or explicit product decision to escrow before refunds.

---

## 10. API contract stability (non-breaking)

Clients must continue to work with:

| Endpoint | Contract |
|----------|----------|
| `POST /bookings/{id}/payment` | Returns `BookingPaymentDto` including `sessionId`, `checkoutUrl`, `provider` |
| `POST /bookings/{id}/payment/confirm` | `{ transactionReference }` — verifies via active gateway |
| Webhooks | Provider-specific URLs; clients do not call these |

Breaking changes require a versioned API bump and coordinated client releases — **avoid during PG-1–PG-3**.

---

## 11. Risks and mitigations

| Risk | Severity | Mitigation |
|------|----------|------------|
| Wrong Areeba product assumed | High | Approval gate chooses MPGS vs IXOPAY with sandbox proof |
| Mobile still auto-confirms | Critical | PG-2 mandatory before production Areeba |
| Silent Development fallback in prod | High | Readiness gate + disallow empty secrets when Provider=Areeba |
| In-flight Moyasar payments after cutover | Medium | Keep Moyasar webhook until drain |
| Currency mismatch (SAR vs USD) | Medium | Align `BookingPayment.Currency` with Areeba merchant currency (platform USD direction) |
| Webhook signature ambiguity | Medium | Confirm Areeba signature algorithm in sandbox before coding |
| Dual client waves (Phase 1C + payments) | Medium | Finish payment mobile path before Phase 1C client migration |
| Escrow assumed available | Medium | Defer; validate commercially |

---

## 12. Test plan outline (for PG-1+)

| Layer | Cases |
|-------|-------|
| Unit | Areeba CreateSession success/failure; Verify paid/unpaid/mismatch; webhook signature valid/invalid; ignored statuses |
| Integration | Initiate → sandbox pay → webhook → booking `PendingCraftsmanConfirmation`; confirm idempotency |
| Client | Android/iOS open checkout; cancel path; success deep link; Development still works in CI |
| Regression | Phase 1A booking/coupon concurrency; Phase 1B auth/webhook fail-closed |
| Readiness | `/health/integrations` reports Areeba Ready only when secrets present |

---

## 13. Approval checklist

Before any implementation:

- [ ] Stakeholders approve **Areeba** as sole production gateway target  
- [ ] Areeba **product** selected: MPGS / ePayment **or** IXOPAY  
- [ ] Sandbox merchant id + API password (+ webhook secret) available  
- [ ] Agree to **keep Moyasar** until PG-4 deprecation  
- [ ] Agree mobile hosted-checkout fix is **in scope for PG-2** (native, not React Native)  
- [ ] Agree refunds/escrow are **out of scope** until PG-5/PG-6  
- [ ] Agree Phase 1C remains **blocked** until this plan is approved (per Phase 1B checkpoint)  

**Sign-off:**

| Role | Name | Date | Decision |
|------|------|------|----------|
| Product | | | Approve / Changes requested |
| Engineering | | | Approve / Changes requested |
| Ops / Merchant | | | Approve / Changes requested |

---

## 14. Next step after approval

1. Open implementation wave **PG-1** on a dedicated branch (`cursor/areeba-payment-gateway-*`).
2. Do not start React Native.
3. Do not start Phase 1C client migrations until PG-2 mobile payment path is designed/scheduled.
4. Update `docs/PRODUCTION.md` and Phase 1 README status when PG-1 begins.

---

*End of document — documentation only; awaiting approval.*
