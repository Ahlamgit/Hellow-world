# ADR-031: Payment Failure, Retry, and Recovery Strategy

## Status

**Accepted Architecture Direction** — 2026-07-25  
**Implementation:** **Not authorized**

## Related ADRs

- [ADR-004](./ADR-004-financial-ledger-mandatory.md) — Ledger as financial source of truth
- [ADR-013](./ADR-013-admin-configurable-financial-business-rules.md) — Admin-configurable financial rules
- [ADR-025](./ADR-025-external-integration-strategy.md) — Integration ports and adapters
- [ADR-029](./ADR-029-simplified-payment-experience-ledger-control.md) — Simplified customer payment experience
- [ADR-030](./ADR-030-booking-payment-financial-state-separation.md) — Booking / payment / ledger / settlement state separation

## Sources

- `PAYMENT_SECURITY_AND_RECONCILIATION_FRAMEWORK.md`
- `MASTER_IMPLEMENTATION_PROMPT_v1.0.md`
- `FEATURE_TRACEABILITY_MATRIX.md`

---

## 1. Context

KHADAMATI payment processing operates across gateway I/O (Payment.js / IXOPAY via `PaymentGatewayPort`), domain payment state, booking state, and ledger posting (ADR-030). Real-world payment flows encounter failures and timing issues that must be handled without compromising:

| Objective | Requirement |
|-----------|-------------|
| **Customer trust** | Clear, simple status — no exposure to internal failures |
| **Financial correctness** | Money facts match gateway reality |
| **Ledger integrity** | No duplicate or missing accounting entries |
| **Auditability** | Every transition and recovery action traceable |
| **Simple UX** | Customer sees Processing / Success / Failed / Action required — not webhooks or ledger errors (ADR-029) |

### Risks to address

| Risk | Description |
|------|-------------|
| Payment gateway failures | Decline, timeout, processor unavailable |
| Network interruptions | Client or server loses connectivity mid-flow |
| Duplicate webhook events | Gateway retries same notification |
| Delayed webhook delivery | Success at gateway before KHADAMATI confirms |
| Partial processing failures | Payment confirmed but ledger post fails |
| User closes payment screen | Unknown outcome until reconciled |
| Provider/service cancellation after payment | Refund/cancellation per admin policies (ADR-013) |

This ADR defines **architecture-level** failure, retry, and recovery behaviour. It does not authorize code, schema, APIs, or vendor configuration.

---

## 2. Decision

Payment processing must be:

| Property | Meaning |
|----------|---------|
| **Event-driven** | Gateway outcomes arrive as verified events (webhook and/or status query); domain transitions react to trusted events |
| **Idempotent** | Retries and duplicate events do not create duplicate payments or ledger entries |
| **Auditable** | All payment attempts, finalizations, blocks, and recovery actions logged |
| **Recoverable** | Reconciliation and recovery workflows restore consistency when async steps fail |

**Critical rule:** Payment events must **not** directly overwrite booking state without validation. Booking advances to a paid gate only when payment domain reaches a **trusted Paid** state per ADR-030 — never from raw webhook payload alone without verification and domain rules.

All gateway interaction remains behind `PaymentGatewayPort` (ADR-025). Ledger remains source of truth for money (ADR-004).

---

## 3. Failure scenarios

### Scenario A — Customer starts payment but payment fails

| Aspect | Expected behaviour |
|--------|-------------------|
| **Trigger** | Gateway decline, validation error, user abort, or hard failure |
| **Payment domain** | Attempt → **Failed** (or Cancelled if user-aborted before submit) |
| **Booking domain** | Remains **unpaid** (e.g. Confirmed / Awaiting Payment) |
| **Ledger** | **No** credit entry |
| **Customer UX** | **Payment failed** — option to retry per product rules |
| **Retry** | New attempt may reuse idempotency semantics; no double charge for same logical attempt |

**Expected:** **Booking remains unpaid.**

---

### Scenario B — Payment succeeds but confirmation is delayed

| Aspect | Expected behaviour |
|--------|-------------------|
| **Trigger** | Gateway captured funds; webhook or client callback delayed or lost |
| **Payment domain** | **Processing** or **Pending** until trusted confirmation |
| **Booking domain** | **Not** marked Paid until payment domain confirms |
| **Recovery** | Reconciliation job / status query via adapter recovers gateway truth |
| **Customer UX** | **Processing** — not Failed, not Success until confirmed |

**Expected:** **System reconciles payment event before confirming status.** Unknown outcomes never auto-mark booking Paid.

---

### Scenario C — Duplicate webhook received

| Aspect | Expected behaviour |
|--------|-------------------|
| **Trigger** | Gateway resends same event reference |
| **Idempotency** | Event processed once; duplicate acknowledged without side effects |
| **Ledger** | **No** duplicate entry |
| **Audit** | Duplicate blocked event recorded for Finance visibility |

**Expected:** **No duplicate ledger entry.**

---

### Scenario D — Payment successful but ledger processing fails

| Aspect | Expected behaviour |
|--------|-------------------|
| **Trigger** | Payment domain reaches Paid at gateway; ledger post fails (DB, worker, transient error) |
| **Payment domain** | Remains **Paid** (gateway truth) |
| **Booking domain** | May remain Awaiting Payment until ledger + payment coupling complete — **or** Paid with compensating recovery queue (implementation detail deferred) |
| **Recovery** | **Recovery process** retries ledger post idempotently until financial consistency restored |
| **Admin** | Pending reconciliation / failed posting visible to Finance Admin |
| **Customer UX** | **Processing** or **Action required** if stuck beyond product threshold — not ledger error details |

**Expected:** **Recovery process restores financial consistency.** Payment history is not rewritten; ledger catch-up uses idempotent post keyed to payment identity.

---

### Scenario E — Customer paid but provider cannot complete service

| Aspect | Expected behaviour |
|--------|-------------------|
| **Trigger** | Booking Paid; service cancelled or cannot be delivered |
| **Payment / booking** | Cancellation workflow per booking domain |
| **Financial** | **Cancellation and refund rules** from active Admin policies (ADR-013) — not hardcoded |
| **Ledger** | Refund/chargeback posts as new immutable entries |
| **Customer UX** | Clear cancellation/refund status — not commission or settlement detail |

**Expected:** **Cancellation/refund rules apply through Admin-configured policies.**

---

### Additional scenarios (framework alignment)

| Scenario | Expected behaviour |
|----------|-------------------|
| Gateway timeout | Payment **Processing**; reconcile; do not invent success |
| Network interruption (client) | Retry with same idempotency key; server returns prior outcome |
| Incorrect amount at gateway | Reject confirmation; alert Finance; booking unpaid |
| User closes payment screen | Treat as unknown until poll/reconcile; show **Processing** then resolve |
| Missing webhook | Status query + reconciliation job recovers |

See `PAYMENT_SECURITY_AND_RECONCILIATION_FRAMEWORK.md` §§2–3 for detailed integrity tables.

---

## 4. Idempotency principles

| Layer | Principle |
|-------|-----------|
| **Payment request** | Must **not** create duplicate payments for the same logical attempt — client/API idempotency key + server payment identity |
| **Webhook event** | Must **not** create duplicate financial records — gateway event/transaction reference processed once |
| **Ledger** | Must **not** contain duplicate accounting entries — ledger post keyed by payment/event identity; second apply is no-op |
| **Adapter** | Vendor retries must correlate to original payment id — no orphan duplicate domain payments |

Duplicate event protection: domain events (`PaymentCaptured`, `PaymentFailed`, etc.) emit once per successful state transition.

---

## 5. Customer experience rules

Customers see **simple, trustworthy states** only (ADR-029):

| Customer-visible state | When shown |
|------------------------|------------|
| **Processing** | Payment in flight or awaiting reconciliation |
| **Payment successful** | Trusted Paid confirmation |
| **Payment failed** | Definitive failure; retry offered if allowed |
| **Action required** | Stuck/prolonged Processing or product-specific follow-up |

Customers must **not** see:

- Technical failure codes or stack traces
- Webhook delivery status or gateway internals
- Ledger posting failures or reconciliation queue details

Internal errors map to customer-safe labels via payment/booking projection APIs.

---

## 6. Admin / Finance visibility

Finance Admin can see:

| Capability | Purpose |
|------------|---------|
| Failed payments | Declines, errors, abandoned attempts |
| Pending reconciliation | Processing/unknown stuck beyond threshold |
| Duplicate events blocked | Idempotency audit — webhooks deduplicated |
| Refund cases | Policy-driven refunds linked to bookings |
| Audit history | Full payment → ledger → settlement trail |

Admin visibility does not change customer-facing simplicity (ADR-029).

---

## 7. Security rules

| Rule | Confirmation |
|------|--------------|
| **No card data storage** | PAN/CVV never in KHADAMATI — Payment.js hosted fields only |
| **Gateway verification required** | Webhooks/signatures verified in adapter before domain effects |
| **Events authenticated** | Untrusted events cannot finalize payment or ledger |
| **Audit trail preserved** | Failures, retries, duplicates blocked, and recovery actions logged; sensitive fields redacted |

Aligns with ADR-025 ports/adapters and `PAYMENT_SECURITY_AND_RECONCILIATION_FRAMEWORK.md` §6.

---

## 8. Implementation benefits

When implementation is authorized (post Gate A), this strategy is expected to yield:

| Benefit | Description |
|---------|-------------|
| Customer trust | Consistent messaging during delays and failures |
| Financial correctness | Gateway truth reconciled before booking/ledger advance |
| Ledger integrity | Idempotent posts and recovery without duplicate money facts |
| Auditability | Finance can investigate failures and blocked duplicates |
| Simpler mobile UX | Few technical states exposed to users |
| Operational resilience | Workers/reconciliation recover from partial failures |

*Benefits are architectural intent only — no coding authorized by this ADR.*

---

## 9. Traceability

**Principle (FTM):** `BR-PAY-18` — *Payment failure, retry, and recovery are event-driven, idempotent, auditable, and recoverable; booking state advances only on validated payment outcomes.*

Maps to:

- `BR-PAY-03` — Webhook verification + idempotency
- `BR-PAY-04` — Payment state machine
- `BR-PAY-05` — Failure handling + reconcile job
- `BR-PAY-12` / `BR-PAY-13` — Cancellation/refund policies (Scenario E)
- `BR-PAY-16` / `BR-PAY-17` — UX simplicity and domain separation

**Documents updated by this ADR:**

- `docs/v1/adr/README.md` — ADR-031 indexed
- `MASTER_IMPLEMENTATION_PROMPT_v1.0.md` — failure/retry/recovery reference
- `FEATURE_TRACEABILITY_MATRIX.md` — BR-PAY-18 added
- `PAYMENT_SECURITY_AND_RECONCILIATION_FRAMEWORK.md` — ADR-031 cross-reference

---

## 10. Governance rule

| Statement | Confirmation |
|-----------|--------------|
| This ADR authorizes coding | **No** |
| This ADR authorizes schema or APIs | **No** |
| This ADR authorizes payment integration | **No** |
| This ADR selects vendors | **No** |
| This ADR modifies frozen scope | **No** |
| This ADR changes implementation gate | **No** |

Implementation remains **BLOCKED** until Gate A and applicable blockers (including BLOCKER-007) are closed.

---

## Consequences

- Payment and booking modules must implement separate state machines with validated coupling (ADR-030).
- Reconciliation workers are architecturally required (ADR-012) for delayed and missing confirmations.
- Customer apps poll or subscribe to **projected** payment status — not raw gateway events.
- Finance Admin tooling must expose failure and reconciliation queues without exposing PCI data.
- Scenario E refunds remain policy-driven — values from Admin configuration (ADR-013), not code constants.
