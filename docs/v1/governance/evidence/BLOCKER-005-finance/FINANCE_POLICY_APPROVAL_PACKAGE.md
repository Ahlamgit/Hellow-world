# KHADAMATI — Finance Policy Approval Package

| Field | Value |
|-------|-------|
| **Document ID** | EVD-005-PKG-001 |
| **Blocker** | BLOCKER-005 — Finance Configuration |
| **Version** | 1.0 |
| **Status** | **READY FOR APPROVAL** (signatures pending) |
| **Gate** | B — NOT READY — CODING BLOCKED |
| **Prepared by** | Finance Governance Manager |
| **Date** | 2026-07-25 |
| **Closure artifact (on completion)** | `FINANCE_RULE_MATRIX_v1.0.md` |

---

## Authority and Purpose

This package is the formal finance policy approval instrument required before KHADAMATI financial rule implementation. It approves **business values and policies** only — not code, schema, or payment logic.

**Source of truth:**

- `MASTER_IMPLEMENTATION_PROMPT_v1.0.md`
- `FINAL_SCOPE_BASELINE.md`
- ADR-013 Financial Policy Configuration
- ADR-026 Lebanon Finance Values
- `FINANCE_POLICY_APPROVAL_MATRIX.md`
- `FINANCE_POLICY_LIFECYCLE.md`
- `PAYMENT_SECURITY_AND_RECONCILIATION_FRAMEWORK.md`
- `BLOCKER_EVIDENCE_MANAGEMENT_FRAMEWORK.md`

| Dimension | Status |
|-----------|--------|
| Architecture | **APPROVED** |
| Financial architecture | **APPROVED** |
| Finance policy **values** | **NOT APPROVED** (this package) |
| Implementation | **BLOCKED** |

**System constraints (non-negotiable):**

- **Admin configurable** — no hardcoded financial rules in application code
- **Multi-country ready** — no country-specific logic embedded in code
- **Multi-currency ready** — currency configuration via policy, not constants
- **Auditable** — all financial policy changes traceable
- **Version controlled** — effective dates and policy versions enforced

**Explicit exclusions:** No code, database schema, payment implementation, architecture changes, or hardcoded financial values in this deliverable.

---

## Instructions to Approvers

1. Review §1–§9 against ADR-013, ADR-026, and `FINANCE_POLICY_APPROVAL_MATRIX.md`
2. Complete **business value decisions** where marked *Pending Business Decision* or *Pending Finance Decision*
3. Sign §10 — verbal approval is insufficient
4. Return to Finance Governance Manager for archival and `FINANCE_RULE_MATRIX_v1.0` completion

Until §10 signatures exist and values are approved, **BLOCKER-005 remains open**.

---

## 1. Financial Architecture Approval

### 1.1 Approved financial flow — confirmation requested

Stakeholders confirm the KHADAMATI financial model:

```
Customer payment
        ↓
Payment Gateway (Areeba IXOPAY Payment.js)
        ↓
Payment verification
        ↓
Ledger entry
        ↓
Provider balance
        ↓
Settlement / withdrawal
```

| Principle | Status | Confirmation |
|-----------|--------|--------------|
| Payment Gateway | Areeba IXOPAY via Payment.js (per approved architecture) | ☐ Confirmed |
| Ledger is the **financial source of truth** | Approved architecture | ☐ Confirmed |
| Provider balance derived from ledger | Per ADR-013 | ☐ Confirmed |
| Settlement/withdrawal follows approved policy (§7–§8) | Subject to value approval below | ☐ Confirmed |
| No silent financial mutations | Per reconciliation framework | ☐ Confirmed |

**Note:** Architecture approval does **not** approve commission rates, subscription prices, or settlement schedules — only the **model**.

**Attestation (§1):** ☐ Business Owner ☐ Finance Owner ☐ Operations Owner

---

## 2. Currency and Market Configuration

### 2.1 Default market (launch configuration)

Documented defaults for initial market — **Admin configurable**, not code constants:

| Setting | Default value | Configuration method |
|---------|---------------|-------------------|
| **Country** | Lebanon | Admin market configuration |
| **Default currency** | USD | Admin currency configuration |
| **Default phone country** | Lebanon (+961) | Admin locale/phone configuration |

### 2.2 Multi-market readiness — confirmation requested

| Requirement | Confirmation |
|-------------|--------------|
| System supports **additional countries** without code change | ☐ Confirmed |
| System supports **additional currencies** without code change | ☐ Confirmed |
| System supports **regional expansion** via configuration | ☐ Confirmed |
| **No country-specific hardcoding** in application logic | ☐ Confirmed |

**Cross-reference:** ADR-026 defines Lebanon as initial finance values context; future markets require new policy versions per `FINANCE_POLICY_LIFECYCLE.md`.

**Attestation (§2):** ☐ Business Owner ☐ Finance Owner ☐ Operations Owner

---

## 3. Commission Policy Approval

### 3.1 Commission model — structure approval

| Model element | Supported (Admin configurable) | Approval |
|---------------|-------------------------------|----------|
| Percentage based | Yes | ☐ Approved |
| Fixed amount | Yes | ☐ Approved |
| Provider type based (Craftsman vs Store) | Yes | ☐ Approved |
| Service category based | Yes | ☐ Approved |
| Promotional exceptions | Yes | ☐ Approved |

### 3.2 Commission values

| Item | Status |
|------|--------|
| Default commission rates | **Pending Business Decision** |
| Category-specific rates | **Pending Business Decision** |
| Promotional overrides | **Pending Business Decision** |
| Effective date | **Pending Business Decision** |

### 3.3 Approval

| Role | Decision | Date | Signature |
|------|----------|------|-----------|
| Business Owner | **Pending** | | |
| Finance Owner | **Pending** | | |

**Value entry table (complete on approval):**

| Provider type | Category | Model (% / fixed) | Rate / amount | Effective from |
|---------------|----------|-------------------|---------------|----------------|
| | | | | |

---

## 4. Subscription Policy Approval

### 4.1 Craftsman subscription plans — structure approval

| Field | Admin configurable | Approval |
|-------|-------------------|----------|
| Plan name | Yes | ☐ Approved |
| Duration | Yes | ☐ Approved |
| Price | Yes | ☐ Approved |
| Benefits | Yes | ☐ Approved |
| Status (active/inactive) | Yes | ☐ Approved |

### 4.2 Store subscription plans — structure approval

| Plan purpose | Admin configurable | Approval |
|--------------|-------------------|----------|
| Service advertising | Yes | ☐ Approved |
| Product catalogue advertising (display only — no checkout) | Yes | ☐ Approved |

### 4.3 Subscription values

| Item | Status |
|------|--------|
| Craftsman plan catalogue | **Pending Business Decision** |
| Store plan catalogue | **Pending Business Decision** |
| Pricing | **Pending Business Decision** |
| Billing cycle | **Pending Business Decision** |

**Value entry table (complete on approval):**

| Plan ID | Provider type | Name | Duration | Price | Currency | Benefits summary | Effective from |
|---------|---------------|------|----------|-------|----------|------------------|----------------|
| | | | | | | | |

---

## 5. Cancellation Policy Approval

### 5.1 Configurable rule dimensions — approval requested

Rules configurable by Admin based on:

| Dimension | Approval requested |
|-----------|-------------------|
| Booking status | ☐ Approved |
| Cancellation time (relative to service) | ☐ Approved |
| Customer vs provider responsibility | ☐ Approved |
| Penalties (fee, commission retention) | ☐ Approved |
| Manual approval requirement | ☐ Approved |

### 5.2 Cancellation values

| Item | Status |
|------|--------|
| Penalty schedules | **Pending Business Decision** |
| Free cancellation windows | **Pending Business Decision** |
| Provider cancellation rules | **Pending Business Decision** |

**Value entry table (complete on approval):**

| Rule ID | Booking status | Time window | Responsible party | Penalty type | Penalty value | Approval required |
|---------|----------------|-------------|-------------------|--------------|---------------|-------------------|
| | | | | | | |

---

## 6. Refund Policy Approval

### 6.1 Supported refund models — approval requested

| Model | Ledger impact required | Approval |
|-------|------------------------|----------|
| Full refund | Yes — reversing entries | ☐ Approved |
| Partial refund | Yes — partial reversal | ☐ Approved |
| No refund | Yes — documented decision record | ☐ Approved |
| Manual review | Yes — holds until approved | ☐ Approved |

### 6.2 Governance rules

| Rule | Confirmation |
|------|--------------|
| All refunds **must create ledger entries** | ☐ Confirmed |
| **No silent financial changes** | ☐ Confirmed |
| Refund policy versioned per `FINANCE_POLICY_LIFECYCLE.md` | ☐ Confirmed |

### 6.3 Refund values

| Item | Status |
|------|--------|
| Default refund rules by booking state | **Pending Finance Decision** |
| Partial refund calculation method | **Pending Finance Decision** |
| Manual review thresholds | **Pending Finance Decision** |

**Value entry table (complete on approval):**

| Rule ID | Trigger condition | Refund model | % / amount | Ledger treatment | Effective from |
|---------|-------------------|--------------|------------|------------------|----------------|
| | | | | | |

---

## 7. Withdrawal Policy Approval

### 7.1 Provider withdrawal configuration — structure approval

| Setting | Admin configurable | Approval |
|---------|-------------------|----------|
| Available withdrawal methods | Yes | ☐ Approved |
| Minimum withdrawal amount | Yes | ☐ Approved |
| Approval requirement (auto vs manual) | Yes | ☐ Approved |
| Processing time (SLA) | Yes | ☐ Approved |

### 7.2 Lebanon payment rails

| Item | Status |
|------|--------|
| Approved withdrawal rails for Lebanon launch | **Pending Vendor/Finance Decision** |
| Rail-specific fees | **Pending Finance Decision** |
| KYC/verification requirements | Cross-ref BLOCKER-006 |

**Value entry table (complete on approval):**

| Method | Min amount | Currency | Approval required | Processing SLA | Rail (Lebanon) | Effective from |
|--------|------------|----------|-------------------|----------------|----------------|
| | | | | | | |

---

## 8. Settlement Policy Approval

### 8.1 Settlement configuration — structure approval

| Setting | Admin configurable | Approval |
|---------|-------------------|----------|
| Settlement frequency | Yes | ☐ Approved |
| Holding period | Yes | ☐ Approved |
| Reserve rules | Yes | ☐ Approved |
| Reconciliation requirement | Yes | ☐ Approved |

### 8.2 Settlement values

| Item | Status |
|------|--------|
| Default settlement cycle | **Pending Finance Decision** |
| Holding period duration | **Pending Finance Decision** |
| Reserve percentage/amount | **Pending Finance Decision** |
| Reconciliation cadence | **Pending Finance Decision** |

**Cross-reference:** `PAYMENT_SECURITY_AND_RECONCILIATION_FRAMEWORK.md`

**Value entry table (complete on approval):**

| Provider type | Frequency | Holding days | Reserve % | Reconciliation | Effective from |
|---------------|-----------|--------------|-----------|----------------|----------------|
| | | | | | |

---

## 9. Financial Governance Rules

Stakeholders confirm the following are **required** for all finance policies:

| Governance control | Required | Confirmation |
|--------------------|----------|----------------|
| **Effective dates** on every policy version | Yes | ☐ Confirmed |
| **Version history** retained | Yes | ☐ Confirmed |
| **Audit trail** for create/update/activate/deactivate | Yes | ☐ Confirmed |
| **Role permissions** for policy management (Admin) | Yes | ☐ Confirmed |
| **Historical protection** — completed bookings use policy at time of booking | Yes | ☐ Confirmed |
| **No retroactive changes** to completed bookings | Yes | ☐ Confirmed |

**Attestation (§9):** ☐ Business Owner ☐ Finance Owner ☐ Operations Owner

---

## 10. Approval Record

**BLOCKER-005 status:** **READY FOR APPROVAL** — not **Closed** until signatures and policy values are approved.

| Role | Name | Decision | Date | Signature |
|------|------|----------|------|-----------|
| Business Owner | | **Pending** | | |
| Finance Owner | | **Pending** | | |
| Operations Owner | | **Pending** | | |

### Decision values

- **Approved** — Policy structure and values accepted for `FINANCE_RULE_MATRIX_v1.0`
- **Approved with conditions** — Document conditions below
- **Rejected** — Package returned; values remain not approved

### Comments / conditions

| Role | Comments |
|------|----------|
| Business Owner | |
| Finance Owner | |
| Operations Owner | |

---

## 11. Blocker Status

| Item | Status |
|------|--------|
| **BLOCKER-005** | **READY FOR APPROVAL** |
| **Financial architecture** | **APPROVED** (unchanged) |
| **Finance values** | **NOT APPROVED** — pending §3–§8 decisions and §10 signatures |
| **Evidence package** | `FINANCE_POLICY_APPROVAL_PACKAGE.md` (this document) |
| **BLOCKER-005 Closed** | **No** |
| **Gate** | **B — NOT READY — CODING BLOCKED** |
| **Closed blockers** | **0 / 7** |
| **Implementation authorized** | **No** |

### Closure criteria (all required)

- [ ] Policy structures §3–§8 approved
- [ ] All *Pending* business/finance values completed in value tables
- [ ] Business Owner — signed §10
- [ ] Finance Owner — signed §10
- [ ] Operations Owner — signed §10
- [ ] `FINANCE_RULE_MATRIX_v1.0.md` published from approved values
- [ ] Package archived under `evidence/BLOCKER-005-finance/`
- [ ] `READINESS_BLOCKER_CLOSURE_STATUS.md` updated to **Closed**

---

## Document Control

| Version | Date | Author | Change |
|---------|------|--------|--------|
| 1.0 | 2026-07-25 | Finance Governance Manager | Initial finance policy approval package |

**Distribution:** Business Owner, Finance Owner, Operations Owner, Program Governance Manager, Technical Architect (informational)
