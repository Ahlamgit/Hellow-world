# ADR-029: Simplified Customer Payment Experience & Internal Ledger Control

## Status

**Accepted Architecture Direction** — 2026-07-25  
*(No implementation authorized by this ADR)*

## Related ADRs

- [ADR-004](./ADR-004-financial-ledger-mandatory.md) — Financial ledger mandatory
- [ADR-005](./ADR-005-booking-confirm-then-pay.md) — Booking confirm then pay
- [ADR-013](./ADR-013-admin-configurable-financial-business-rules.md) — Admin-configurable financial rules
- [ADR-025](./ADR-025-external-integration-strategy.md) — Integration ports and adapters
- [ADR-026](./ADR-026-lebanon-finance-policy-configuration.md) — Lebanon finance configuration

## Sources

- `MASTER_IMPLEMENTATION_PROMPT_v1.0.md`
- `FINAL_SCOPE_BASELINE.md`
- `PAYMENT_SECURITY_AND_RECONCILIATION_FRAMEWORK.md`
- `FEATURE_TRACEABILITY_MATRIX.md`

---

## 1. Context

KHADAMATI requires a payment and settlement architecture that is:

- **Secure** — PCI-minimized online payment via Payment.js / IXOPAY
- **Protective** — provider earnings and platform commission handled correctly
- **Auditable** — immutable ledger, reconciliation, and Finance Admin visibility
- **Configurable** — commission, cancellation, refund, withdrawal, and settlement policies are Admin-managed (ADR-013, ADR-026)

At the same time:

- **Customers** must not experience financial complexity. They are booking a service, not operating a ledger.
- **Providers** must not manage accounting concepts, commission math, or settlement batches in V1.
- **V1** must avoid unnecessary product and engineering complexity that increases support burden, security exposure, and time-to-market without adding core marketplace value.

The approved architecture already mandates Payment.js, ports/adapters, ledger-as-truth, and Booking → Payment → Ledger → Settlement. This ADR clarifies **where complexity lives** (internal only) and **what V1 explicitly excludes** from customer and provider surfaces.

**Unchanged by this ADR:** Payment.js / IXOPAY integration approach, port & adapter model (ADR-025), ledger as financial source of truth (ADR-004), admin-configurable financial policies (ADR-013/026), confirm-then-pay booking sequence (ADR-005), audit requirements, and multi-country readiness.

---

## 2. Decision

Adopt a **dual-layer payment model**:

| Layer | Audience | Responsibility |
|-------|----------|----------------|
| **Experience layer** | Customer, Provider | Simple status, amounts, confirmations, earnings summaries |
| **Financial layer** | Platform (backend), Finance Admin | Payment transactions, ledger, commission evaluation, settlement |

### 2.1 Simple payment experience (customer)

Canonical customer flow:

```text
Booking Request
  → Provider Confirmation
  → Customer Pays
  → Payment Confirmed
  → Service Completed
  → Settlement Processing (internal — not shown as a customer step)
```

**Customer sees only:**

- Service
- Provider
- Amount (payable total)
- Payment status
- Confirmation / receipt-style acknowledgment

**Customer does NOT see:**

- Ledger entries
- Commission calculations
- Settlement batches
- Provider balance or payable breakdown
- Internal financial rules or policy engines

Customer APIs and UI expose **payment status** and **booking status** — not ledger line items.

---

## 3. Provider experience

**Provider sees:**

- Booking status
- Completed services
- Earnings summary (derived, read-only)
- Withdrawal availability (when policy allows — not instant-by-default in V1)

**Provider does NOT manage:**

- Ledger entries
- Commission rules
- Settlement calculations
- Manual balance adjustments

Provider financial views are **summaries** sourced from ledger-derived balances — never editable accounting screens.

---

## 4. Internal financial model

Financial complexity remains **entirely internal** to the platform and Finance Admin.

```text
Customer Payment
  → Payment Transaction (gateway via PaymentGatewayPort — ADR-025)
  → Ledger Entry (ADR-004)
  → Commission Rule Evaluation (ADR-013 — active admin rules)
  → Provider Payable Amount
  → Settlement Process (admin-configurable policy)
```

| Principle | Rule |
|-----------|------|
| Ledger | **Financial source of truth** — all money facts post here |
| Provider balance | **Derived** from ledger — not a parallel wallet truth |
| Commission | Evaluated from **active admin rules** at defined lifecycle events |
| Gateway | Payment.js tokenization only; KHADAMATI never stores card data |
| Corrections | Reversing ledger entries — not silent mutation |

This ADR does **not** replace or weaken ADR-004, ADR-013, or `PAYMENT_SECURITY_AND_RECONCILIATION_FRAMEWORK.md`. It constrains **what surfaces** expose financial detail.

---

## 5. V1 complexity control

Explicitly **avoid in V1**:

| Exclusion | Rationale |
|-----------|-----------|
| Customer wallet | Reduces UX confusion and regulatory surface |
| Provider wallet UI | Providers see summaries, not ledger operations |
| Manual financial adjustments (non-admin) | Prevents support abuse and audit gaps |
| Gateway split payments | Defer to post-V1; single capture flow via Payment.js |
| Complex escrow screens | Escrow/holding is internal state — not a customer workflow |
| Instant withdrawals | Withdrawal follows admin policy and approval paths |
| Customer financial dashboards | No ledger/commission/settlement visibility for customers |

**Goals:** reduce development complexity, support complexity, security exposure, and customer confusion — without removing ledger, audit, or settlement capabilities internally.

---

## 6. Payment states (conceptual only)

*Conceptual states for architecture alignment — no schema, API, or implementation authorized here.*

### 6.1 Payment states

| State | Meaning (conceptual) |
|-------|----------------------|
| Pending | Payable exists; payment not yet started or awaiting action |
| Processing | Authorization/capture in flight with gateway |
| Paid | Trusted confirmation received; booking may advance per ADR-005 |
| Failed | Attempt declined or errored; no ledger credit |
| Cancelled | Payment attempt voided before completion |
| Refunded | Refund processed per admin refund rules (ADR-013) |

### 6.2 Booking states (payment-relevant subset)

| State | Meaning (conceptual) |
|-------|----------------------|
| Requested | Customer submitted booking request |
| Confirmed | Provider accepted — payment may proceed (ADR-005) |
| Awaiting Payment | Confirmed; payment required |
| Paid | Payment confirmed; service may proceed |
| In Progress | Service execution underway |
| Completed | Service finished; commission/settlement events may follow |
| Cancelled | Booking cancelled per cancellation policy |

Full booking lifecycle remains defined in workflow docs and ADR-005; this table aligns payment UX with booking status only.

---

## 7. Security principles

Confirm and retain (no relaxation):

| Control | Requirement |
|---------|-------------|
| Card data | Handled by **Payment.js hosted fields** only |
| Storage | KHADAMATI does **not** store card data or PCI-sensitive payloads |
| Events | Payment gateway events **verified** (webhook/signature) before domain finalization |
| Ledger | Updates are **auditable**; financial history cannot be silently changed |
| Amounts | Server-authoritative payable amounts — client cannot override |
| Idempotency | Duplicate events must not double-post ledger entries |

See `PAYMENT_SECURITY_AND_RECONCILIATION_FRAMEWORK.md` for integrity rules and failure handling.

---

## 8. Admin finance experience

**Finance Admin** retains full complexity and responsibility:

- Commission policies (ADR-013)
- Cancellation rules
- Refund rules
- Settlement configuration
- Financial reporting and reconciliation
- Withdrawal approval workflows

**Rule:** Admin sees complexity; customers and providers do not.

---

## 9. Implementation impact (anticipated benefits)

When implementation is authorized (post Gate A), this direction is expected to yield:

| Area | Benefit |
|------|---------|
| Mobile UX | Fewer screens and states exposed to customers |
| Backend | Clear boundary between payment status APIs and ledger domain |
| API design | Customer/provider DTOs exclude ledger internals |
| Testing | Simpler contract tests on public payment/booking surfaces |
| Scaling | Internal financial model can evolve without customer app churn |
| Support | Fewer “where is my commission?” tickets from customers |

*Benefits are architectural intent only — no coding authorized by this ADR.*

---

## 10. Traceability

**Principle (FTM):** `BR-PAY-16` — *Financial complexity is internal; customer payment experience remains simple.*

Maps to:

- Customer pay flow: `BR-CUS-12`, `BR-PAY-01`–`BR-PAY-04`
- Internal ledger: `BR-PAY-06`–`BR-PAY-08`, `BR-PAY-14`
- Admin policy: `BR-PAY-12`–`BR-PAY-15`, `BR-ADM-*` finance modules

**Documents updated by this ADR:**

- `FEATURE_TRACEABILITY_MATRIX.md` — BR-PAY-16 added
- `MASTER_IMPLEMENTATION_PROMPT_v1.0.md` — §9 / ADR companion list
- `PAYMENT_SECURITY_AND_RECONCILIATION_FRAMEWORK.md` — experience vs internal model
- `FINAL_ARCHITECTURE_DECISIONS_COMPLETE.md` — ADR-029 registered

---

## 11. Governance rule

| Statement | Confirmation |
|-----------|--------------|
| This ADR authorizes coding | **No** |
| This ADR changes implementation gate (Gate B) | **No** |
| This ADR changes vendor selections | **No** |
| This ADR changes Payment.js / IXOPAY decision | **No** |
| This ADR removes ledger requirements | **No** |
| This ADR removes compliance requirements | **No** |
| This ADR modifies frozen scope | **No** |
| This ADR replaces Payment.js architecture | **No** |

Implementation remains **BLOCKED** until Gate A and applicable blockers (including BLOCKER-007 for payment integration) are closed per governance records.

---

## Consequences

- Customer and provider UIs/APIs must not expose ledger entries, commission math, or settlement batches in V1.
- Engineering may implement rich ledger and admin finance modules internally without simplifying audit requirements.
- V1 explicitly defers wallets, split payments, instant withdrawal UX, and customer financial dashboards.
- Future V2 features that expose more financial detail require ADR and scope change control.
