# KHADAMATI — Areeba Implementation Decision

**Status:** Documentation only — **awaiting approval**  
**Date:** 2026-07-23  
**Parent plan:** [PAYMENT_GATEWAY_MIGRATION_PLAN.md](./PAYMENT_GATEWAY_MIGRATION_PLAN.md)  
**Architecture (already approved):**

- Keep `IPaymentGateway`
- Keep `DevelopmentPaymentGateway` for development/testing
- Implement Areeba as production gateway
- Keep Moyasar temporarily until Areeba staging validation completes
- Do **not** start Phase 1C yet

**This document:** Product choice, payment flow, database impact, Moyasar coexistence strategy, and test plan.  
**Code / migrations / mobile changes:** None until this decision is approved.

---

## Executive recommendation

**Adopt Option A — Areeba MPGS Hosted Checkout (`epayment.areeba.com`) as the production integration for KHADAMATI.**

Rationale in one line: MPGS maps cleanly to the existing Moyasar “create session → hosted checkout → verify/webhook” model, keeps card data off KHADAMATI clients/servers (PCI SAQ-A friendly), and works for Web + native mobile via redirect/WebView without collecting PANs.

**Prerequisite before coding:** Confirm with Areeba merchant boarding that the KHADAMATI merchant account is (or will be) enabled for **MPGS / ePayment**. If boarding is IXOPAY-only, fall back to Option B with the same abstraction and flow rules below.

---

# 1. AREEBA PRODUCT DECISION

## 1.1 Options

| | Option A — MPGS Hosted Checkout | Option B — IXOPAY |
|--|----------------------------------|-------------------|
| **Product** | Areeba ePayment / MasterCard Payment Gateway Services | Areeba Payment Gateway (IXOPAY-based) |
| **Base URL** | `https://epayment.areeba.com` | `https://gateway.areebapayment.com` |
| **Primary API** | `POST .../merchant/{id}/session` (`INITIATE_CHECKOUT`) | `POST .../transaction/{apiKey}/debit` |
| **Customer UX** | Hosted page via `checkout.js` (`showPaymentPage` / embedded) or redirect | Redirect to `redirectUrl` from debit response |
| **Status check** | `GET .../order/{orderId}` | `GET .../status/{apiKey}/getByMerchantTransactionId/{id}` |
| **Auth** | Basic Auth (`merchant.{MerchantId}` + API password) | Basic Auth (username/password) + API key in path |

## 1.2 Evaluation matrix

| Criterion | Option A — MPGS | Option B — IXOPAY | Prefer |
|-----------|-----------------|-------------------|--------|
| **Lebanon availability** | Offered by Areeba for Lebanon merchants; USD common on sandbox/docs; merchant must be boarded on ePayment portal (`epayment.areeba.com/ma`) | Also offered for Lebanon via `gateway.areebapayment.com`; widely used in regional integrations | **Tie** — both available; **boarding decides** |
| **Supported payment methods** | Cards via hosted checkout; 3-D Secure in hosted flow; exact brand list is merchant-configured (Visa/Mastercard typical) | Cards via hosted redirect; payment method returned on status (`Creditcard`, etc.); adapter-dependent extras | **A** for KHADAMATI MVP (card booking payments only) |
| **Card handling responsibility** | Cards entered on Areeba/MPGS hosted UI; KHADAMATI never touches PAN/CVV | Cards entered on gateway hosted payment page after redirect; KHADAMATI never touches PAN/CVV if redirect-only | **Tie** (both OK if hosted) |
| **PCI compliance impact** | Strong SAQ-A path when using full hosted / `checkout.js` hosted page (no card fields on our domain) | SAQ-A path if redirect-only; higher scope if any embedded card fields are ever added | **A** slightly clearer hosted-checkout story |
| **Webhook support** | Notifications/webhook recommended on successful payment; order includes `notificationUrl`; must still **Retrieve Order** on return | Asynchronous status notification to callback URL from debit request; plus status API | **Tie** — both support async notify; both require server verify |
| **Mobile compatibility** | Open hosted checkout in Custom Tabs / SFSafariViewController / WebView; return via `returnUrl` / deep link; optional `checkout.js` on a thin web bridge page | Open `redirectUrl` the same way; return via success/error/cancel URLs with `transactionId` | **Tie** — both mobile-friendly via browser chrome |
| **Refund support** | Available via MPGS order refund/void APIs / merchant admin (confirm exact API with boarding) | Explicit refund API (`referenceUuid`) documented on IXOPAY lineage | **B** slightly clearer documented refund API today; **defer refunds** to later phase either way |
| **Transaction reconciliation** | Reconcile by merchant `order.id` + Retrieve Order; session id is ephemeral | Reconcile by `merchantTransactionId` / `uuid` / status API | **Tie** — both reconcilable if we persist gateway ids |
| **Integration complexity** | Closest to current Moyasar invoice model (`CreateSession` + hosted URL + verify) | Different debit/redirect/status shape; still fits `IPaymentGateway` but more mapping work vs current code | **A** |

## 1.3 Fit with current KHADAMATI code

| Existing concept | MPGS mapping | IXOPAY mapping |
|------------------|--------------|----------------|
| `IPaymentGateway.CreateSessionAsync` | Initiate checkout session + order id | Debit → store `merchantTransactionId` + `redirectUrl` |
| `PaymentSessionDto.SessionId` | Prefer **order id** (stable for Retrieve Order); also persist session id | `merchantTransactionId` (and/or uuid) |
| `PaymentSessionDto.CheckoutUrl` | Hosted checkout URL or KHADAMATI bridge page that runs `Checkout.showPaymentPage()` | `redirectUrl` from debit |
| `IPaymentGateway.VerifyAsync` | Retrieve Order; require paid/captured equivalent | Status by merchant transaction id; require `SUCCESS` |
| Webhook → `ConfirmPaymentFromWebhookAsync` | Map notification → order/session reference | Map callback → merchantTransactionId / uuid |

## 1.4 Final recommendation

| Decision | Value |
|----------|-------|
| **Selected product** | **Option A — Areeba MPGS Hosted Checkout** |
| **Fallback** | Option B — IXOPAY, only if merchant boarding cannot enable MPGS |
| **Config key** | `Payment:Areeba:Product = Mpgs` (or `Ixopay` if fallback) |
| **Implementation class** | Single `AreebaPaymentGateway : IPaymentGateway` with product-specific HTTP mapping inside Infrastructure |
| **Business logic** | Never references MPGS/IXOPAY types — only `IPaymentGateway` |

### Approval note

Ops must confirm boarding product in writing before PG-1 coding. If confirmation is IXOPAY, update this document’s “Selected product” line and proceed with Option B under the same flow/DB/test rules.

---

# 2. PAYMENT FLOW DESIGN

## 2.1 Authoritative end-to-end flow

```
Customer
   |
Booking created
   |
Customer confirms booking → status AwaitingPayment
   |
Payment initiated (POST /bookings/{id}/payment)
   |
Backend creates BookingPayment (Pending → Processing)
   |
Backend calls IPaymentGateway.CreateSessionAsync
   |
AreebaPaymentGateway creates Areeba checkout session (MPGS)
   |
API returns sessionId + checkoutUrl (+ provider)
   |
Client opens Areeba checkout (Web redirect / mobile browser chrome)
   |
Customer completes payment on Areeba
   |
Areeba webhook → POST /api/v1/webhooks/areeba
   |
Backend verifies signature (fail-closed in Staging/Production)
   |
Backend maps event → payment reference
   |
Backend calls ConfirmPaymentFromWebhookAsync
   |
IPaymentGateway.VerifyAsync (Retrieve Order / status) — amount + currency check
   |
BookingPayment → Completed (PaidAt set)
   |
Booking → PaymentConfirmed → PendingCraftsmanConfirmation
   |
Slot reserved + craftsman notified
```

## 2.2 Secondary (non-authoritative) paths

| Path | Allowed? | Rules |
|------|----------|-------|
| Client `POST /payment/confirm` after return URL | Yes | Must call `VerifyAsync`; must be **idempotent** if webhook already completed |
| Client polls `GET /bookings/{id}` | Yes | Preferred UX signal for “paid” |
| Client locally sets “paid” | **No** | Forbidden |
| Development provider fake `/pay` page | Yes (dev/CI only) | Never enabled when `Payment:Provider=Areeba` |

## 2.3 Mobile rule (mandatory)

**The mobile application must never mark payment as completed.**

| Client may do | Client must not do |
|---------------|--------------------|
| Call initiate and open `checkoutUrl` | Call confirm immediately after initiate |
| Show **Pending / Processing** from API status | Optimistically flip UI to Paid without API |
| On return deep link, refresh booking or call confirm | Trust return URL query params alone as proof of payment |
| Rely on webhook + server verify as source of truth | Store a local “paid” flag that overrides server status |

Same rule applies to Web: UI “Completed” only from API booking/payment status after server verification.

## 2.4 Failure / cancel paths

| Event | Backend | Client UX |
|-------|---------|-----------|
| Customer cancels checkout | Payment stays `Processing`/`Pending` until expiry/fail job | Show cancelled/pending; allow retry initiate if business rules allow |
| Payment declined | Webhook ignored or verify fails; optional `Failed` update | Show failure; keep booking `AwaitingPayment` until timeout |
| Invalid webhook signature | `401` / reject; no booking mutation | N/A |
| Duplicate success webhook | Idempotent: already `Completed` → return success/no-op | Show completed |
| Payment window expiry | Existing booking expiry behavior | Show expired |

## 2.5 Provider selection at runtime

```
Payment:Provider
  ├── Development → DevelopmentPaymentGateway
  ├── Moyasar     → MoyasarPaymentGateway   (temporary coexistence)
  └── Areeba      → AreebaPaymentGateway    (production target)
```

Only **one** provider creates **new** sessions. Webhooks for Moyasar and Areeba may both remain active during migration for in-flight payments.

---

# 3. DATABASE IMPACT REVIEW

## 3.1 Current `BookingPayment` (EF)

| Column | Present | Notes |
|--------|---------|-------|
| `Id`, audit soft-delete fields | Yes | `BaseEntity` |
| `ServiceRequestId` | Yes | Unique 1:0..1 |
| `PayerUserId` / `PayeeUserId` | Yes | No escrow |
| `Amount` / `Currency` | Yes | |
| `Status` | Yes | `PaymentStatus` enum |
| `PaymentMethod` | Yes | |
| `TransactionReference` | Yes | Overloaded today as session/invoice id |
| `PaidAt` / `FailureReason` | Yes | |

**Missing for robust multi-gateway operation:** provider, distinct session vs transaction ids, webhook idempotency key, attempt tracking.

## 3.2 Candidate fields — decision

| Proposed field | Need? | Decision | Rationale |
|----------------|-------|----------|-----------|
| **`PaymentProvider`** | Yes | **Add** | Distinguish Development / Moyasar / Areeba during coexistence and support |
| **`GatewaySessionId`** | Yes | **Add** | MPGS session id (ephemeral); needed for checkout.js / debugging |
| **`GatewayTransactionId`** | Yes | **Add** | Stable paid reference (MPGS order id or IXOPAY uuid); prefer this for verify/webhook correlation |
| **`GatewayStatus`** | Optional | **Defer** | Can derive from `PaymentStatus` + logs for MVP; add later if ops needs raw PSP status strings |
| **`WebhookEventId`** | Recommended | **Add** | Idempotent webhook processing (unique filtered index) |
| **`PaymentAttemptNumber`** | Optional | **Defer** | Only one active payment row per booking today; retries can overwrite session fields or use status transitions. Revisit if product requires payment history rows |

### Also recommended (not in the ask list, but additive)

| Field / index | Decision | Rationale |
|---------------|----------|-----------|
| Keep `TransactionReference` | **Keep** | Continue to store the **authoritative paid reference** (usually same as `GatewayTransactionId` on completion) for existing API/admin compatibility |
| Filtered unique index on `TransactionReference` WHERE NOT NULL | **Add** | Webhook lookup + concurrency |
| Filtered unique index on `WebhookEventId` WHERE NOT NULL | **Add** | Duplicate webhook protection |
| Index on `GatewayTransactionId` | **Add** | Verify/webhook correlation |

## 3.3 Proposed additive schema (PG migration — not yet implemented)

```text
BookingPayments
  + PaymentProvider       nvarchar(50)  NOT NULL  DEFAULT 'Development'
  + GatewaySessionId      nvarchar(200) NULL
  + GatewayTransactionId  nvarchar(200) NULL
  + WebhookEventId        nvarchar(200) NULL
  -- existing TransactionReference, Status, etc. unchanged

Indexes (filtered where applicable):
  UX_BookingPayments_TransactionReference
  UX_BookingPayments_WebhookEventId
  IX_BookingPayments_GatewayTransactionId
```

**Rules:**

1. Additive migration only — no drops, no renames of existing columns.
2. EF migration must match physical SQL Server schema.
3. Backfill `PaymentProvider` for existing rows (`Development` if `TransactionReference` like `KHD-%`, else `Moyasar` when historically Moyasar).
4. Do **not** add escrow/refund tables in this wave.

## 3.4 Explicitly not required for Areeba launch

| Field | Why defer |
|-------|-----------|
| `GatewayStatus` | Ops can use Serilog + admin detail; avoid dual status sources |
| `PaymentAttemptNumber` | Single payment row per booking; retry policy TBD |
| Separate `PaymentAttempts` table | Overkill until multi-attempt product requirement |

---

# 4. MOYASAR MIGRATION STRATEGY

Moyasar code and webhook remain until a successful production period on Areeba. **Do not remove old code early.**

## Stage 1 — Moyasar + Areeba coexist

| Item | Action |
|------|--------|
| DI | Register both `Moyasar` and `Areeba` cases; default still Moyasar or Development per environment |
| New sessions | Controlled by `Payment:Provider` (only one active creator) |
| Webhooks | Both `/webhooks/moyasar` and `/webhooks/areeba` live |
| DB | `PaymentProvider` records which gateway created the row |
| Goal | Implement Areeba behind `IPaymentGateway`; no production cutover yet |

## Stage 2 — Areeba production validation (staging / limited)

| Item | Action |
|------|--------|
| Staging | `Payment:Provider=Areeba` |
| Sandbox | End-to-end booking pay + webhook + mobile checkout |
| Regression | Phase 1A booking integrity; Phase 1B webhook fail-closed |
| Moyasar | Keep webhook + code path for rollback drills |
| Goal | Signed-off staging evidence pack |

## Stage 3 — Switch default provider

| Item | Action |
|------|--------|
| Production | Set `Payment:Provider=Areeba` (config/secret only) |
| Monitoring | Success rate, webhook failures, `AwaitingPayment` stuck count |
| Moyasar webhook | Remains enabled for in-flight Moyasar `Processing` payments |
| Rollback | Revert `Payment:Provider` to `Moyasar` if Areeba outage (code still present) |
| Goal | All **new** payments created on Areeba |

## Stage 4 — Moyasar deprecation

| Gate | Requirement |
|------|-------------|
| Drain | No remaining Moyasar `Processing` / unpaid in-flight invoices past TTL |
| Soak | Agreed successful production period (recommend **≥ 30 days** stable Areeba) |
| Then | Remove Moyasar gateway, webhook controller, Moyasar config keys, readiness branches, docs |
| Secrets | Rotate/remove Moyasar vault secrets per security runbook |

**Hard rule:** No Moyasar deletion in Stages 1–3.

---

# 5. TEST PLAN

## 5.1 Backend unit / service tests

| Case | Expectation |
|------|-------------|
| **Create payment session** | `AreebaPaymentGateway.CreateSessionAsync` posts expected payload; returns `SessionId` + `CheckoutUrl`; persists `PaymentProvider=Areeba`, `GatewaySessionId`, `GatewayTransactionId` |
| **Verify webhook (valid)** | Valid signature + paid status → `ConfirmPaymentFromWebhookAsync` → payment `Completed`, booking advances |
| **Invalid webhook rejection** | Bad/missing signature → unauthorized; **no** payment/booking mutation |
| **Duplicate webhook handling** | Second identical event (`WebhookEventId` or already `Completed`) → idempotent success/no-op; no double slot reservation |
| **Payment failure handling** | Declined/failed verify → payment not completed; booking remains `AwaitingPayment` (or marked `Failed` per agreed rule); craftsman not notified |

Additional backend cases:

| Case | Expectation |
|------|-------------|
| Amount/currency mismatch on verify | Reject confirmation |
| Moyasar webhook still works during coexistence | Unaffected by Areeba code |
| Development provider | Still creates `KHD-*` sessions for CI |

## 5.2 Integration tests

| Case | Expectation |
|------|-------------|
| **Areeba sandbox transaction** | Staging/sandbox credentials: create booking → initiate → complete sandbox card → webhook (or retrieve-order verify) → booking `PendingCraftsmanConfirmation` |
| Integration readiness | `/health/integrations` Ready only when Areeba required secrets present |
| Config switch | `Payment:Provider=Areeba` resolves `AreebaPaymentGateway` |

## 5.3 Mobile tests (manual + automated where feasible)

| Case | Expectation |
|------|-------------|
| **Open checkout** | After initiate, app opens `checkoutUrl` (Custom Tabs / Safari VC); does **not** call confirm immediately |
| **Return handling** | Success/cancel deep link returns to app; app refreshes booking from API |
| **Pending payment state** | UI shows pending/processing while API status is `AwaitingPayment` / payment not `Completed` |
| **Completed payment state** | UI shows paid/next booking state **only** after API reflects server-side completion (webhook/verify) |

Web parity checks: redirect/open checkout, success page does not claim paid until API confirms.

## 5.4 Exit criteria before production Stage 3

- [ ] All §5.1 tests green in CI  
- [ ] Sandbox E2E (§5.2) recorded  
- [ ] Android + iOS + Web mobile/web checklist (§5.3) signed  
- [ ] Duplicate webhook and invalid signature cases proven  
- [ ] Rollback to Moyasar provider switch rehearsed on staging  

---

## Approval checklist

- [ ] Approve **Option A (MPGS Hosted Checkout)** as default (or document IXOPAY fallback if boarding requires it)  
- [ ] Approve payment flow with **webhook + VerifyAsync** as source of truth  
- [ ] Approve rule: **mobile never marks payment completed**  
- [ ] Approve additive DB fields: `PaymentProvider`, `GatewaySessionId`, `GatewayTransactionId`, `WebhookEventId`  
- [ ] Approve deferral of `GatewayStatus` and `PaymentAttemptNumber`  
- [ ] Approve Moyasar Stages 1–4 (no early removal)  
- [ ] Approve test plan exit criteria  

**Still blocked until approval:**

- Areeba implementation  
- Database migration  
- Phase 1C  
- Mobile payment changes  

**Sign-off:**

| Role | Name | Date | Decision |
|------|------|------|----------|
| Product | | | Approve / Changes requested |
| Engineering | | | Approve / Changes requested |
| Ops / Merchant boarding | | | Confirm MPGS vs IXOPAY |

---

*End of document — documentation only; STOP and wait for approval.*
