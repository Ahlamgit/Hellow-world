# ADR-030: Booking, Payment, Ledger and Settlement State Separation

## Status

**Accepted Architecture Direction** — 2026-07-25  
*(No implementation authorized by this ADR)*

## Related ADRs

- [ADR-004](./ADR-004-financial-ledger-mandatory.md) — Financial ledger mandatory
- [ADR-005](./ADR-005-booking-confirm-then-pay.md) — Booking confirm then pay
- [ADR-013](./ADR-013-admin-configurable-financial-business-rules.md) — Admin-configurable financial rules
- [ADR-029](./ADR-029-simplified-payment-experience-ledger-control.md) — Simplified customer/provider payment UX; internal ledger control

## Sources

- `MASTER_IMPLEMENTATION_PROMPT_v1.0.md`
- `PAYMENT_SECURITY_AND_RECONCILIATION_FRAMEWORK.md`
- `FEATURE_TRACEABILITY_MATRIX.md`
- `FINAL_SCOPE_BASELINE.md`

---

## 1. Context

KHADAMATI contains multiple related but **independent** business processes:

| Process | Concern |
|---------|---------|
| **Customer booking experience** | Service request, provider acceptance, service execution, completion |
| **Payment processing** | Gateway authorization, capture, confirmation, failure, refund |
| **Financial accounting** | Immutable ledger entries, commission evaluation, provider payables |
| **Provider settlement** | Payout eligibility, batching, execution, withdrawal lifecycle |

These processes are **sequenced** in the marketplace workflow but must **not** share a single status field or one overloaded state model. Conflating domains causes incorrect UI, brittle APIs, audit gaps, and bugs when payment succeeds but service is incomplete, or service completes before settlement runs.

ADR-005 defines confirm-then-pay booking order. ADR-029 defines what customers and providers see. This ADR defines **domain state ownership** and **cross-domain rules**.

**Unchanged:** Payment.js / IXOPAY approach, ledger as financial source of truth, admin-configurable finance policies, frozen scope, and compliance requirements.

---

## 2. Decision

Maintain **separate state ownership** — each domain owns its own lifecycle and status vocabulary.

| Domain | Owns | Does not own |
|--------|------|--------------|
| **Booking** | Customer service lifecycle (request → confirm → pay gate → in progress → complete / cancel) | Gateway capture outcome, ledger posting, settlement batch status |
| **Payment** | Gateway payment lifecycle (initiated → processing → paid / failed / cancelled / refunded) | Service completion, commission math, settlement execution |
| **Ledger** | Financial records (entries, balances derived from entries, commission lines) | Booking UX states, gateway I/O, settlement batch UI |
| **Settlement** | Provider payout lifecycle (eligible → batched → processing → paid / failed) | Payment history mutation, booking status, customer-facing labels |

**Anti-pattern (prohibited):** A single `status` column or enum on `bookings` (or any aggregate) that encodes payment, ledger, and settlement phases together — e.g. `PAID_AND_SETTLED` or `PAYMENT_PENDING_SETTLEMENT`.

**Required pattern:** Each bounded context exposes its own state; cross-domain transitions are **events** or orchestrated domain services — not merged status strings.

---

## 3. Relationship

Canonical cross-domain sequence (conceptual — no schema or API authorized here):

```text
Booking Confirmed
  ↓
Payment Requested
  ↓
Payment Successful
  ↓
Ledger Entry Created
  ↓
Service Completed
  ↓
Settlement Eligible
  ↓
Settlement Processed
```

| Step | Booking domain | Payment domain | Ledger domain | Settlement domain |
|------|----------------|----------------|---------------|-------------------|
| Booking Confirmed | Confirmed | — | — | — |
| Payment Requested | Awaiting Payment (or equivalent) | Pending / Processing | — | — |
| Payment Successful | Paid (payment gate satisfied) | Paid | Entry created | — |
| Ledger Entry Created | (unchanged) | (unchanged) | Posted | — |
| Service Completed | Completed | (unchanged) | May trigger commission evaluation | May become eligible |
| Settlement Eligible | (unchanged) | (unchanged) | Payable derived | Eligible |
| Settlement Processed | (unchanged) | (unchanged) | Settlement entries | Processed |

Domains advance independently within these coupling points. A delay in settlement does **not** roll back booking or payment state.

---

## 4. Rules

| Rule | Confirmation |
|------|--------------|
| **Payment success does not mean service completion** | **Confirmed** — booking may be Paid while service is not yet Completed |
| **Service completion does not mean settlement completed** | **Confirmed** — settlement follows admin policy and may be batched/async |
| **Settlement does not modify payment history** | **Confirmed** — settlement posts new ledger facts; does not rewrite payment records |
| **Ledger records are immutable financial history** | **Confirmed** — corrections via reversing entries (ADR-004), not silent edits |
| **Each domain API exposes only its own states** | **Confirmed** — customer booking API ≠ payment API ≠ admin ledger API |
| **Cross-domain updates are idempotent** | **Confirmed** — duplicate payment webhooks must not double-post ledger (see payment framework) |

---

## 5. Customer visibility

**Customer sees:**

- Booking status (service lifecycle)
- Payment confirmation (success / failure / retry as product allows)
- Service progress (in progress, completed)

**Customer does NOT see:**

- Ledger entries or account postings
- Settlement status or batches
- Commission calculations or platform take rate

Aligns with ADR-029 — customer surfaces use **booking + payment status** projections only.

---

## 6. Provider visibility

**Provider sees:**

- Bookings (their pipeline)
- Completed services
- Available earnings (summary derived from ledger — read-only)

**Provider does NOT manage:**

- Ledger entries
- Settlement calculations or batch composition

Provider apps query **projections** (earnings summary, withdrawal availability) — not raw ledger or settlement domain internals.

---

## 7. Finance / Admin visibility

**Finance / Admin sees:**

- Payment transactions (gateway-linked)
- Ledger (full entries, accounts, commission lines)
- Commission policy application and audit
- Settlement batches and payout status
- Audit history across money paths

Admin is the only role with **cross-domain** financial visibility. Operational booking support may see booking + payment status without full ledger unless role permits.

---

## 8. Implementation benefits

When implementation is authorized (post Gate A), separate domain states are expected to yield:

| Benefit | Description |
|---------|-------------|
| **Cleaner APIs** | `GET /bookings/{id}` returns booking state; payment and settlement have distinct resources |
| **Easier testing** | Unit tests per state machine; integration tests at event boundaries |
| **Reduced bugs** | No accidental “set booking to settled” when only payment succeeded |
| **Better auditability** | Ledger and payment histories remain distinct, traceable chains |
| **Easier multi-country expansion** | Payment and settlement adapters vary by market without rewriting booking UX states |

*Benefits are architectural intent only — no coding authorized by this ADR.*

---

## 9. Traceability

**Principle (FTM):** `BR-PAY-17` — *Booking, payment, ledger, and settlement each maintain separate state ownership; no single shared status field across domains.*

Maps to:

- Booking lifecycle: `BR-CUS-*`, `BR-BOOK-*`, booking module entities
- Payment: `BR-PAY-01`–`BR-PAY-05`, `BR-PAY-04` (payment state machine)
- Ledger: `BR-PAY-06`–`BR-PAY-08`, `BR-PAY-13`
- Settlement: `BR-PAY-14`, `BR-PAY-09`
- UX separation: `BR-PAY-16` (ADR-029)

**Documents updated by this ADR:**

- `docs/v1/adr/README.md` — ADR-030 indexed
- `MASTER_IMPLEMENTATION_PROMPT_v1.0.md` — domain state separation reference
- `FEATURE_TRACEABILITY_MATRIX.md` — BR-PAY-17 added
- `PAYMENT_SECURITY_AND_RECONCILIATION_FRAMEWORK.md` — §1B domain state ownership

---

## 10. Governance rule

| Statement | Confirmation |
|-----------|--------------|
| This ADR authorizes coding | **No** |
| This ADR authorizes schema or APIs | **No** |
| This ADR changes implementation gate | **No** |
| This ADR modifies frozen scope | **No** |
| This ADR removes ledger requirements | **No** |
| This ADR replaces Payment.js architecture | **No** |

Implementation remains **BLOCKED** until Gate A and applicable blockers are closed per governance records.

---

## Consequences

- Engineering must model four distinct state machines (booking, payment, ledger effects, settlement) with explicit coupling events.
- UI and public APIs must not expose a unified “mega-status” spanning financial domains.
- Refunds and cancellations must update the **correct** domain states without corrupting unrelated histories.
- Future ADRs or specs that define entity schemas must respect this separation.
