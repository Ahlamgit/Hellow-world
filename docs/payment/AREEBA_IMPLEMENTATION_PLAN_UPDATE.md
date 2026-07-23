# Areeba Implementation Plan — Update (Payment Attempt Model)

**Status:** Approved to implement — architecture locked  
**Date:** 2026-07-23  
**Supersedes conflicting attempt guidance in prior plan revisions**

## Final architecture decision

| Concept | Role |
|---------|------|
| `BookingPayment` | Business payment **obligation** for a booking (1:0..1) |
| `BookingPaymentAttempt` | Every gateway interaction (1:N under a payment) |

```
Booking (ServiceRequest)
  └── BookingPayment          ← obligation (amount, currency, current status)
        └── BookingPaymentAttempts[]  ← gateway tries (Areeba / Moyasar / Development)
```

**Do not** store multiple gateway attempts only inside `BookingPayment`.

### Attempt rules

1. One `BookingPayment` may have many attempts.  
2. Only **one** attempt may be `Completed`.  
3. A completed payment **cannot** be overwritten.  
4. Webhook processing must be **idempotent**.  
5. Duplicate webhook events must be **ignored**.  

### Minimum `BookingPaymentAttempts` fields

| Field | Required |
|-------|----------|
| `Id` | Yes |
| `BookingPaymentId` | Yes |
| `PaymentProvider` | Yes |
| `AttemptNumber` | Yes |
| `GatewaySessionId` | Yes (nullable until session created) |
| `GatewayTransactionId` | Yes (nullable until assigned) |
| `Status` | Yes |
| `Amount` | Yes |
| `Currency` | Yes |
| `RequestDate` | Yes |
| `CompletedDate` | Nullable |
| `FailedDate` | Nullable |
| `FailureReason` | Nullable |
| `WebhookEventId` | Nullable |
| `CreatedAt` | Yes |
| `UpdatedAt` | Nullable |

### Header additions on `BookingPayment`

| Field | Purpose |
|-------|---------|
| `PaymentProvider` | Active/last provider |
| `CurrentAttemptId` | FK to active attempt |
| `GatewaySessionId` / `GatewayTransactionId` | Denormalized from current attempt for API/webhook speed |
| `FailedAt` | Header-level failure timestamp |
| Keep `PaidAt` / `FailureReason` / `CreatedAt` | Compatibility |

### Payment status (append-only ints)

| Value | Name |
|-------|------|
| 1–6 | Existing (`Pending`…`Cancelled`) — **do not renumber** |
| 7 | `AwaitingGatewayConfirmation` |
| 8 | `Expired` |

### Gateway independence

- Orchestration in `BookingService` / payment services only uses `IPaymentGateway`.  
- Adapters: Areeba (prod target), Moyasar (temporary), Development (dev/CI).  
- No Areeba types in booking business transitions.

---

## Implementation process (enterprise)

| Step | Deliverable | Status |
|------|-------------|--------|
| 1 | Implementation plan update (this doc) | Done |
| 2 | Database migration plan | `AREEBA_DATABASE_MIGRATION_PLAN.md` |
| 3 | Risk assessment | `AREEBA_RISK_ASSESSMENT.md` |
| 4 | Code implementation | Phases A–E |
| 5 | Unit tests | Required |
| 6 | Integration tests | Required (sandbox-gated where needed) |
| 7 | Verification report | `AREEBA_VERIFICATION_REPORT.md` |
| 8 | **Stop for approval** | After verification |

## Implementation phases

| Phase | Scope |
|-------|-------|
| **A** | Database payment model (`BookingPaymentAttempts` + header columns + enum) |
| **B** | Areeba gateway adapter (`AreebaPaymentGateway` + DI + config + readiness) |
| **C** | Webhook processing (`/webhooks/areeba`, signature, idempotency) |
| **D** | Booking/payment state integration (initiate/finalize via attempts) |
| **E** | Web/mobile checkout correction (no client completion authority) |

**Out of scope until payment migration stable:** Phase 1C, React Native, Moyasar removal, refunds/escrow.

## Completion authority

Only **webhook + server-side `VerifyAsync`** finalizes payment for production gateways.  
Clients open checkout and refresh status; they never mark payment completed.
