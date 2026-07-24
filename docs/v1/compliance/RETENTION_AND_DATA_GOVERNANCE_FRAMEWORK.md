# KHADAMATI V1 — Retention & Data Governance Framework

**Document ID:** KHAD-V1-DATA-GOVERNANCE  
**Version:** 1.0  
**Date:** 2026-07-24  
**Role:** Compliance & Data Governance Architect  
**BLOCKER-006 status:** **IN PREPARATION**  

**Sources:**  
Master Prompt v1.0 · [`../FINAL_ARCHITECTURE_DECISIONS_COMPLETE.md`](../FINAL_ARCHITECTURE_DECISIONS_COMPLETE.md) · [`../FINAL_SCOPE_BASELINE.md`](../FINAL_SCOPE_BASELINE.md) · ADR-022 · [`../FINAL_IMPLEMENTATION_GATE_REPORT.md`](../FINAL_IMPLEMENTATION_GATE_REPORT.md) · [`RETENTION_POLICY_DECISIONS.md`](./RETENTION_POLICY_DECISIONS.md)

```text
DO NOT write code.
DO NOT create database schema.
DO NOT implement deletion jobs.
DO NOT define legal obligations that are not approved.

No final retention durations.
No invented legal bases.
Numeric defaults and legal sign-off remain Pending Business / Legal Approval.
```

---

## Status Snapshot

| Item | State |
|------|-------|
| Architecture | **APPROVED** |
| Implementation | **B) NOT READY — CODING BLOCKED** |
| Compliance | **NOT FINALIZED** |
| BLOCKER-006 | **IN PREPARATION** |
| Numeric retention defaults | **Pending Business / Legal Approval** |
| Decision capture sheet | [`RETENTION_POLICY_DECISIONS.md`](./RETENTION_POLICY_DECISIONS.md) |

---

## Purpose

Define KHADAMATI **data governance requirements** before implementation.

Coverage (conceptual — not schema):

| Domain |
|--------|
| Customer data |
| Provider data |
| Store data |
| Identity verification data |
| Financial records |
| Audit records |
| Chat records |
| Media files |

Companion for filling durations / approvals: [`RETENTION_POLICY_DECISIONS.md`](./RETENTION_POLICY_DECISIONS.md)

---

# 1. Data Classification

## 1.1 Personal Data

| Examples | Notes |
|----------|-------|
| Name | Display and legal name fields |
| Phone | OTP / contact channel |
| Email | Account communication |
| Address | Service / profile location context |

Treat as subject to anonymisation on account deletion (ADR-022), subject to approved retention.

## 1.2 Provider Verification Data

| Examples | Notes |
|----------|-------|
| Identity documents | KYC media via StoragePort |
| Verification results | Status / reason codes from IdentityVerificationPort |
| Approval history | Admin/onboarding decisions |

Higher sensitivity; least-privilege access; retention separate from casual profile PII.

## 1.3 Financial Data

| Examples | Notes |
|----------|-------|
| Payments | Payment intents / captures / refunds |
| Ledger entries | Authoritative money facts |
| Commissions | Policy-driven calculations |
| Settlements | Provider balance / payout records |

**Protected** from casual account deletion (see §4).

## 1.4 Operational Data

| Examples | Notes |
|----------|-------|
| Bookings | Service marketplace lifecycle |
| Reviews | Ratings / moderation |
| Availability | Provider calendars / slots |
| Notifications | Delivery metadata (not OTP secrets in logs) |

## 1.5 Audit Data

| Examples | Notes |
|----------|-------|
| Admin actions | Who/what/when |
| Policy changes | Finance / retention / settings history |
| Security events | Auth failures, privilege use, webhook auth failures |

**Protected** — append-oriented; purge only per approved audit retention (never for convenience).

---

# 2. Data Ownership

Ownership is **business/accountability ownership**, not storage location.

| Data set | Primary owner | Steward |
|----------|---------------|---------|
| Customer data | Product / Customer Ops | Eng (processing) |
| Provider data | Product / Provider Ops | Eng |
| Store data | Product / Store Ops | Eng |
| Financial data | Finance | Eng (ledger integrity) |
| Audit data | Security / Admin | Eng |

### Access by role (framework)

| Data set | Customer | Provider | Store | Admin | Finance Admin | Super Admin |
|----------|----------|----------|-------|-------|---------------|-------------|
| Own customer profile | Own | — | — | Support (audited) | Limited need | Elevated (audited) |
| Own provider profile / listings | — | Own | — | Ops (audited) | Money views only | Elevated |
| Own store profile / catalog | — | — | Own | Ops (audited) | Money views only | Elevated |
| Booking parties’ chat | Own booking | Own booking | Own booking (if party) | Support access (audited) | No content by default | Elevated (audited) |
| KYC / identity docs | — | Own status | Own status | KYC ops (restricted) | — | Elevated (audited) |
| Financial records | Own receipts | Own balances | Own balances | Read per RBAC | Full finance scope | Elevated |
| Audit records | — | — | — | Scoped | Finance-related audits | Full (audited) |

Exact permission matrices remain in security/RBAC specs; this table defines governance expectations only.

---

# 3. Retention Policy Framework

**Do NOT define final durations in this document.**

For each data class, record (in [`RETENTION_POLICY_DECISIONS.md`](./RETENTION_POLICY_DECISIONS.md)):

| Field | Meaning | Value |
|-------|---------|-------|
| Retention owner | Who proposes/maintains the default | **Pending Business / Legal Approval** |
| Approval authority | Who may approve changes | **Pending Business / Legal Approval** |
| Retention reason | Business / operational / compliance rationale (approved wording only) | **Pending Business / Legal Approval** |
| Deletion eligibility | When purge/anonymise is allowed | **Pending Business / Legal Approval** |
| Legal/compliance review | Sign-off recorded | **Pending Business / Legal Approval** |

### Architecture constraints (already approved)

| Constraint | Source |
|------------|--------|
| Retention policies are **Admin/Market-configurable** | ADR-022 |
| Durations are **not** hardcoded as sole production truth | ADR-022 · Master Prompt |
| Financial + audit records protected from casual deletion | ADR-022 |
| Lebanon launch defaults require Compliance/Legal approval before Gate A | BLOCKER-006 |

---

# 4. Account Deletion Model

Aligned with ADR-022:

```text
Account deletion request
  → Verification
  → Restriction period
  → Data review
  → Deletion or anonymisation
  → Audit record
```

| Step | Governance expectation |
|------|------------------------|
| Request | Customer / Provider / Store self-service or Admin-initiated (product rules Pending) |
| Verification | OTP / re-auth before irreversible steps |
| Restriction period | Cooling-off (configurable; duration Pending) |
| Data review | Open bookings, balances, disputes, legal holds |
| Deletion or anonymisation | Remove or anonymise Personal Data; unlink display identity |
| Audit record | Every step emits audit events |

### Confirm (architecture requirement)

| Rule | Status |
|------|--------|
| **Financial records are protected from deletion** with the account | **Confirmed** (ADR-022) — retain money facts; anonymise display PII where feasible |
| Audit records are protected | **Confirmed** (ADR-022) |
| KYC media / chat / optional media purge only after approved retention elapses | Configurable — durations Pending |

---

# 5. Anonymisation Strategy

Apply when deletion eligibility is met and legal hold is absent:

| Goal | Approach |
|------|----------|
| Personal information removal | Clear or replace identifiers (name, phone, email, address display fields) with non-identifying refs |
| Historical reporting preservation | Keep aggregated / anonymised operational history where Product requires reporting |
| Financial record protection | Keep ledger/payment/commission/settlement facts; replace personal labels with anonymised party refs |

Anonymisation **does not** delete protected financial or required audit rows.

---

# 6. Access Control Requirements

| Requirement | Expectation |
|-------------|-------------|
| Least privilege | Default deny; grant only for role duty |
| Role-based access | Customer / Provider / Store / Admin / Finance Admin / Super Admin (and finer RBAC as designed) |
| Admin separation | Admin web-only + MFA (ADR-006); support access to sensitive data is audited |
| Finance permissions | Finance Admin scoped to money ops; cannot casually purge protected finance/audit |
| Audit access | Read of audit logs restricted; export/use logged |

---

# 7. Data Security Requirements

| Requirement | Expectation |
|-------------|-------------|
| Encryption | In transit (TLS); at rest for stores holding sensitive objects/secrets |
| Secure storage | KYC/media via StoragePort; private by default |
| Access logging | Sensitive reads/exports auditable |
| Backup protection | Backups inherit access controls; restore is controlled/ops-audited (align BLOCKER-004) |
| Data transfer protection | Vendor transfers only via approved ports/adapters; no ad-hoc exports |

No schema or job design in this document.

---

# 8. Chat Data Governance

**Scope (frozen):** Booking-scoped chat only (Scope Baseline · ADR-020).  
No open marketplace messaging in V1.

| Topic | Framework rule | Status |
|-------|----------------|--------|
| Access rules | Only booking parties + audited Admin/support | Architecture confirmed |
| Retention decision | Configurable chat retention (ADR-022) | Duration **Pending Business / Legal Approval** |
| Abuse reporting | Lightweight complaint / dispute path may reference chat evidence | Product ops model exists; retention of evidence Pending |
| Audit requirements | Admin access to chat content is audited; deletion/anonymisation steps audited | Required |

On account anonymisation: anonymise sender labels; retain or purge message bodies per approved chat retention config.

---

# 9. Compliance Approval Matrix

| Area | Owner | Status |
|------|-------|--------|
| Customer data | Pending | Approval required |
| Provider KYC | Pending | Approval required |
| Financial records | Finance | Approval required |
| Audit logs | Security/Admin | Approval required |
| Chat retention | Product/Legal | Approval required |

Additional acknowledgements (when defaults are set): Compliance/Legal (primary for BLOCKER-006 closure), Security, Product (UX messaging), Solution Architect (ADR-022 alignment).

Capture signatures and numeric defaults in [`RETENTION_POLICY_DECISIONS.md`](./RETENTION_POLICY_DECISIONS.md).

---

# 10. Relationship to BLOCKER-006 Closure

This document advances **governance preparation** only.

BLOCKER-006 remains **IN PREPARATION** until:

- [ ] Numeric or explicit retention defaults set (not TBD) in decisions sheet  
- [ ] Account deletion UX/policy choices approved  
- [ ] Financial + audit protection acknowledged by Finance / Security  
- [ ] Compliance / Legal approval recorded  
- [ ] Chat / KYC / media retention defaults approved  

Until then: **no** deletion job implementation, **no** schema as production truth for durations.

---

# 11. Explicit Non-Goals

- No code  
- No database schema  
- No deletion/purge job implementation  
- No unapproved legal obligations or jurisdiction claims  
- No invented retention day counts  

---

**End of Retention & Data Governance Framework v1.0**
