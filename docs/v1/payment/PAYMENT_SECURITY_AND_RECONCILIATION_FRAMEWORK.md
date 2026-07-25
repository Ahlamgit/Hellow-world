# KHADAMATI V1 — Payment Security & Reconciliation Framework

**Document ID:** KHAD-V1-PAYMENT-SECURITY-RECON  
**Version:** 1.3  
**Date:** 2026-07-25  
**Role:** Payment Architecture & Financial Integrity Architect  
**Status:** Architecture preparation — **Implementation BLOCKED**  

**Sources:**  
Master Prompt v1.0 · [`PAYMENT_JS_MOBILE_VALIDATION_REPORT.md`](./PAYMENT_JS_MOBILE_VALIDATION_REPORT.md) · [`AREEBA_IXOPAY_VENDOR_VALIDATION_CHECKLIST.md`](./AREEBA_IXOPAY_VENDOR_VALIDATION_CHECKLIST.md) · ADR-004 · ADR-013 · ADR-025 · **ADR-029** · **ADR-030** · **ADR-031** · [`../workflows/23-PAYMENT-FLOW.md`](../workflows/23-PAYMENT-FLOW.md) · [`../vendors/INTEGRATION_CONTRACT_SPECIFICATION.md`](../vendors/INTEGRATION_CONTRACT_SPECIFICATION.md)

```text
DO NOT write production code.
DO NOT integrate Areeba.
DO NOT create payment APIs.
DO NOT define commercial values (fees, commissions, settlement schedules).

Payment architecture APPROVED · Payment.js DESIGNED · Ledger APPROVED.
Implementation remains BLOCKED until Implementation Gate → A.
```

---

## Status Snapshot

| Item | State |
|------|-------|
| Payment architecture | **APPROVED** |
| Payment.js | **DESIGNED** (mobile spike PASS WITH CONDITIONS — BLOCKER-007 **IN VALIDATION**) |
| Ledger | **APPROVED** |
| Implementation | **BLOCKED** |
| Gateway runtime validation | Pending vendor sandbox (BLOCKER-003 / BLOCKER-007) |
| Commercial / finance policy values | Pending Business Decision (BLOCKER-005) — out of scope here |

---

## Purpose

Define **payment integrity requirements** before implementation:

- Correct lifecycle from booking pay → ledger → provider balance → settlement  
- Failure and idempotency behaviour  
- Ledger as financial source of truth  
- Reconciliation and Finance Admin visibility  
- PCI-minimizing security controls  
- Multi-country / multi-provider readiness via ports  

Companion spike/evidence: [`PAYMENT_JS_MOBILE_VALIDATION_REPORT.md`](./PAYMENT_JS_MOBILE_VALIDATION_REPORT.md) · [`AREEBA_IXOPAY_VENDOR_VALIDATION_CHECKLIST.md`](./AREEBA_IXOPAY_VENDOR_VALIDATION_CHECKLIST.md)

---

# 1. Payment Lifecycle

Domain-owned sequence (vendor I/O only via `PaymentGatewayPort`):

```text
Booking Created
  → Payment Initiated
  → Payment Authorization
  → Payment Confirmation
  → Webhook Verification
  → Ledger Entry
  → Booking Paid State
  → Provider Balance Update
  → Settlement
```

| Step | Integrity rule |
|------|----------------|
| Booking Created | Payable exists; amount/currency server-authoritative |
| Payment Initiated | Payment record CREATED/PENDING; idempotency key accepted |
| Payment Authorization | Client uses hosted fields → opaque token only; no PAN in KHADAMATI |
| Payment Confirmation | Gateway success path and/or verified webhook |
| Webhook Verification | Authenticity check in adapter **before** domain finalization |
| Ledger Entry | Immutable money fact; references payment + payable |
| Booking Paid State | Booking transitions only after trusted confirmation |
| Provider Balance Update | Derived from ledger / policy — not from client claims |
| Settlement | Admin-configurable policy (ADR-013); values Pending Business Decision |

Commission calculation uses Admin financial rules (ADR-013) — **not** hardcoded in this framework.

---

# 1A. Experience Layer vs Internal Financial Layer (ADR-029)

| Surface | Exposes | Must NOT expose (V1) |
|---------|---------|----------------------|
| **Customer app** | Service, provider, payable amount, payment status, confirmation | Ledger entries, commission, settlement, provider balance |
| **Provider app** | Booking status, completed services, earnings summary, withdrawal availability | Ledger operations, commission rules, settlement batches |
| **Finance Admin** | Full policies, ledger, reconciliation, settlement, reporting | — (admin owns complexity) |
| **Backend / workers** | Payment transactions, verified webhooks, ledger posts, commission evaluation, settlement | N/A — internal domain |

**Principle:** *Financial complexity is internal; customer payment experience remains simple.* (FTM: `BR-PAY-16`)

This section does not reduce ledger, audit, or reconciliation requirements in §§1–9 below.

---

# 1B. Domain State Ownership (ADR-030)

Booking, payment, ledger, and settlement are **separate domains** — each with its own state machine. They must not share one overloaded status field.

| Domain | Owns | Example states (conceptual) |
|--------|------|----------------------------|
| **Booking** | Service lifecycle | Requested → Confirmed → Awaiting Payment → Paid → In Progress → Completed / Cancelled |
| **Payment** | Gateway lifecycle | Pending → Processing → Paid / Failed / Cancelled / Refunded |
| **Ledger** | Financial facts | Entry posted; balances derived; commission lines; reversals only |
| **Settlement** | Provider payout lifecycle | Eligible → Batched → Processing → Processed / Failed |

**Cross-domain sequence:**

```text
Booking Confirmed → Payment Requested → Payment Successful → Ledger Entry Created
  → Service Completed → Settlement Eligible → Settlement Processed
```

| Rule | Requirement |
|------|-------------|
| Payment success ≠ service completion | Booking may be Paid before Completed |
| Service completion ≠ settlement done | Settlement follows policy; may be async/batched |
| Settlement does not modify payment history | New ledger facts only; no rewrite of payment records |
| Ledger immutability | ADR-004 — corrections via reversing entries |

**Visibility:** Customers see booking + payment status; providers see bookings + earnings summary; Finance Admin sees all domains (see ADR-029, ADR-030). FTM: `BR-PAY-17`.

---

# 2. Payment Failure Scenarios

*Normative architecture: [ADR-031](../adr/ADR-031-payment-failure-retry-and-recovery-strategy.md). FTM: `BR-PAY-18`.*

| Scenario | Required handling |
|----------|-------------------|
| Customer payment failure | Mark attempt FAILED/DECLINED; booking remains unpaid; user may retry under product rules; no ledger credit |
| Gateway timeout | Payment stays PENDING/unknown; **do not** invent success; reconcile job + wait for webhook |
| Duplicate webhook | Idempotent finalize — same outcome, no double ledger post |
| Missing webhook | Reconciliation / status query recovers; alert if stuck beyond ops threshold (threshold Pending Ops) |
| Incorrect payment amount | Server amount wins; reject or fail confirmation if gateway amount ≠ payable; alert Finance |
| Partial payment | Not accepted as full booking pay unless explicit product/policy allows (default: reject / leave unpaid) |
| Network interruption | Client may retry with same idempotency key; server returns prior state; no double debit |

Unknown outcomes never auto-mark booking Paid.

---

# 3. Idempotency Rules

| Layer | Rule |
|-------|------|
| Payment request idempotency | Client/API `Idempotency-Key` + merchant/payment transaction id; replay returns original payment outcome |
| Webhook idempotency | Gateway event / transaction reference processed once; duplicates acknowledged without re-applying side effects |
| Ledger idempotency | Ledger post keyed by payment/event identity; second apply is no-op |
| Duplicate event protection | Outbox / domain events for `PaymentCaptured` / `PaymentFailed` emit once per successful transition |

Adapters must not map vendor retries into new domain payments without correlation to the original payment id.

---

# 4. Ledger Integrity

**Ledger is the financial source of truth.**  
Wallet/balance displays are projections; they must reconcile to ledger.

| Requirement | Expectation |
|-------------|-------------|
| Immutable financial records | No casual hard-delete; corrections via compensating entries / controlled adjustments with audit |
| Audit trail | Who/what/when for payment finalization, refunds, adjustments, Admin overrides |
| Transaction references | KHADAMATI payment id ↔ gateway reference ↔ payable (booking/subscription/etc.) |
| Reconciliation capability | Exportable/queryable enough to match gateway and settlement reports |

Provider balance and settlement consume ledger facts under Admin policies (ADR-013) — commercial rates **not** defined here.

---

# 5. Reconciliation Process

```text
Internal ledger
  → Gateway transactions
  → Settlement reports
```

| Activity | Expectation |
|----------|-------------|
| Daily reconciliation | Compare ledger captures/refunds to gateway transaction set for the period |
| Mismatch detection | Missing webhook, amount mismatch, status mismatch, duplicate gateway refs |
| Investigation workflow | Queue for Finance Admin: open → investigate → resolve (adjust / retry reconcile / escalate vendor) → close with audit |

Workers (architecture): payment reconciliation jobs; no implementation in this document.

Severity: payment/ledger mismatches are **Critical** ops items (align Production Ops readiness).

---

# 6. Security Requirements

| Requirement | Expectation |
|-------------|-------------|
| No raw card storage | PAN/CVV never in KHADAMATI apps, APIs, DB, or logs |
| Hosted payment fields | Payment.js (or equivalent hosted fields) for card entry |
| Token handling | Opaque `transactionToken` (or successor) used server-side once per debit rules; do not log tokens |
| Webhook verification | Signature/auth verified in adapter before domain effects |
| Access control | Payment credentials in secrets manager; Finance/Admin least privilege |
| Audit logging | Payment lifecycle + Admin finance actions audited; redact sensitive fields |

PCI minimization via ports/adapters (ADR-025) and Payment.js model (ADR-004).

---

# 7. Admin Visibility

**Finance Admin** capabilities (governance — not UI build):

| Capability | Purpose |
|------------|---------|
| View transactions | Inspect payment lifecycle and gateway refs |
| View failed payments | Support retries / customer issues |
| View reconciliation status | Daily/period reconcile health |
| Review mismatches | Investigation queue |
| Access audit history | Who finalized, refunded, adjusted, or overrode |

Super Admin elevated access remains audited. Support Admin does not gain unrestricted finance mutation rights by default.

---

# 8. Multi-country Readiness

| Capability | V1 posture |
|------------|------------|
| Lebanon default | Launch market |
| USD default | Architecture default currency (ADR-001) |
| Multiple currencies later | Market/config-driven; amounts server-owned per Market |
| Multiple payment providers later | New adapter behind `PaymentGatewayPort`; domain/ledger unchanged (ADR-025) |

No commercial FX or fee schedules defined here.

---

# 9. Approval Checklist

| Area | Status |
|------|--------|
| Payment flow | **Pending** |
| Security | **Pending** |
| Reconciliation | **Pending** |
| Ledger integrity | **Pending** |
| Gateway validation | **Pending** |
| Finance approval | **Pending** |

**Notes:**

- *Payment flow / Security / Reconciliation / Ledger integrity* = Architect + Eng (+ Finance for money paths) acceptance of this framework.  
- *Gateway validation* = BLOCKER-007 checklist + sandbox evidence.  
- *Finance approval* = acknowledge ledger-as-truth + reconcile/admin visibility; commercial policy values remain BLOCKER-005.

---

# 10. Relationship to Readiness Gates

| Gate / blocker | Relationship |
|----------------|--------------|
| Implementation Gate | Remains **B) NOT READY — CODING BLOCKED** |
| BLOCKER-007 | Mobile/Payment.js validation still **IN VALIDATION** |
| BLOCKER-003 | Payment vendor sandbox / credentials Pending |
| BLOCKER-005 | Commission/settlement **values** Pending — not this doc |
| ADR-013 / ADR-025 / **ADR-029** / **ADR-030** / **ADR-031** | Financial rules Admin-configurable; vendor behind port; simplified UX; separate domain states; failure/retry/recovery |

This document does **not** authorize payment module coding.

---

# 11. Explicit Non-Goals

- No production code  
- No Areeba / IXOPAY integration  
- No payment API implementation  
- No commercial fee/commission/settlement values  
- No schema or job code  

---

**End of Payment Security & Reconciliation Framework v1.3**

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-24 | Initial framework |
| 1.1 | 2026-07-25 | ADR-029 experience vs internal layer; BR-PAY-16 principle |
| 1.2 | 2026-07-25 | ADR-030 domain state ownership; BR-PAY-17 principle |
| 1.3 | 2026-07-25 | ADR-031 failure/retry/recovery; BR-PAY-18 principle |
