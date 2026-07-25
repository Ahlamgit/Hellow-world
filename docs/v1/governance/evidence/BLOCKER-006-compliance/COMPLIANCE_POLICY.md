# KHADAMATI — Compliance Policy (Governance Draft)

| Field | Value |
|-------|-------|
| **Document ID** | EVD-006-POLICY-001 |
| **Blocker** | BLOCKER-006 — Compliance |
| **Version** | 1.0 |
| **Status** | **DRAFT — Pending Legal / Compliance Approval** |
| **Gate** | B — NOT READY — CODING BLOCKED |
| **Prepared by** | Compliance Governance Manager |
| **Date** | 2026-07-25 |
| **Approval package** | `COMPLIANCE_APPROVAL_PACKAGE.md` (EVD-006-PKG-001) |

```text
GOVERNANCE DRAFT — Not effective until BLOCKER-006 Closed with signed approvals.
Engineering must NOT assume policy values until Legal/Compliance signs §10.
```

---

## 1. Purpose

Define the compliance governance framework for KHADAMATI V1 data handling, access control, financial record protection, and regulatory readiness — **without implementing** technical controls until Gate A and feature authorization.

**Subordinate to:** ADR-022 · `RETENTION_AND_DATA_GOVERNANCE_FRAMEWORK.md` (when restored) · `COMPLIANCE_APPROVAL_PACKAGE.md`

---

## 2. Data categories

| Category | Description | Examples | Sensitivity | Owner |
|----------|-------------|----------|-------------|-------|
| **Account data** | User identity and profile | Customer/provider profiles, contact details, addresses | Confidential — Personal | Legal / Compliance |
| **Booking data** | Service lifecycle records | Bookings, status history, schedules, reviews | Internal — Operational | Legal / Compliance |
| **Payment references** | Payment metadata (no card data) | Payment IDs, amounts, status, gateway references | Highly Confidential — Financial | Finance / Legal |
| **Identity verification data** | KYC and verification | Documents, verification results, cases | Highly Confidential — Identity | Compliance |
| **Audit records** | Immutable action logs | Admin actions, policy changes, security events | Highly Confidential — Audit | Compliance / Security |
| **Security logs** | Access and security events | Login attempts, MFA events, sensitive access | Highly Confidential — Security | Security |

---

## 3. Compliance principles

| Principle | Requirement |
|-----------|-------------|
| Least privilege | RBAC; role-based access to sensitive data |
| Financial integrity | Ledger and payment history are protected records — no silent modification |
| Auditability | Admin and finance actions logged; retention per approved policy |
| Data minimization | Collect only data required for defined purposes |
| Booking-scoped chat | No open marketplace messaging (ADR-020) |
| Multi-market readiness | Policies configurable per market — no hardcoded legal assumptions |
| Engineering boundary | Engineering does not define retention durations or legal interpretations |

---

## 4. Access control governance

| Role category | Access scope | Governance note |
|---------------|--------------|-----------------|
| Customer / Provider | Own data; booking-scoped chat | Self-service where applicable |
| Store operator | Store and booking data within scope | Least privilege |
| Finance Admin | Financial data, policies, settlements | MFA; full audit |
| Super Admin | Platform configuration | Web only; MFA (ADR-006) |
| Compliance / Legal | Audit and policy oversight | Read/approve per policy |

---

## 5. Account deletion governance

Approved process (implementation gated post Gate A):

```text
Deletion request → Identity verification → Account status review
    → Financial/legal obligation check → Deletion or anonymisation decision
    → Execution record → Audit preservation
```

| Rule | Detail |
|------|--------|
| Financial records | May require preservation despite deletion request |
| Audit records | Cannot be removed without approved policy |
| Traceability | Deletion must not break financial traceability (ADR-022) |

---

## 6. Payment and financial compliance

| Principle | Status |
|-----------|--------|
| Ledger is financial source of truth | Confirmed (ADR-004) |
| Payment events auditable | Required |
| No silent modification of financial history | Required |
| Payment.js / IXOPAY — no card storage | Required (ADR-029) |
| Finance policies admin-configurable | Required (ADR-013; BLOCKER-005) |

---

## 7. Chat compliance

| Rule | V1 |
|------|-----|
| Booking-scoped chat only | Required |
| Public / marketplace messaging | Excluded |
| Retention duration | **Pending Legal / Compliance** — see `DATA_RETENTION_POLICY.md` |

---

## 8. Multi-country readiness

| Setting | Initial default | Note |
|---------|-----------------|------|
| Country | Lebanon | Launch market |
| Currency | USD | Per finance policy |
| Phone | +961 | E.164 format |

Architecture supports additional countries, currencies, and regulatory requirements via policy configuration and legal review per market.

---

## 9. Open items (require Legal / Compliance resolution)

| Item | Status | Owner |
|------|--------|-------|
| Retention durations | **Pending** | Legal / Compliance |
| KYC detailed requirements | **Pending** | Compliance |
| Financial records retention rules | **Pending** | Finance / Legal |
| Chat retention rules | **Pending** | Compliance |
| Lebanon regulatory review | **Pending** | Legal |
| Account deletion exception catalogue | **Pending** | Legal / Compliance |
| Cross-border data transfer (if applicable) | **Pending** | Legal |

---

## 10. Approval

**Effective only after BLOCKER-006 Closed.**

| Role | Name | Decision | Date | Signature |
|------|------|----------|------|-----------|
| Legal / Compliance Owner | | Pending | | |
| Business Owner | | Pending | | |
| Technical Architect | | Pending | | |

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | Initial compliance policy draft for BLOCKER-006 review |
