# KHADAMATI — Compliance Approval Package

| Field | Value |
|-------|-------|
| **Document ID** | EVD-006-PKG-001 |
| **Blocker** | BLOCKER-006 — Compliance Approval |
| **Version** | 1.0 |
| **Status** | **READY FOR APPROVAL** (signatures pending) |
| **Gate** | B — NOT READY — CODING BLOCKED |
| **Prepared by** | Compliance Governance Manager |
| **Date** | 2026-07-25 |
| **Closure artifact (on completion)** | `COMPLIANCE_APPROVAL_PACK_v1.0.md` |

---

## Authority and Purpose

This package is the formal compliance governance approval instrument required before KHADAMATI data lifecycle, KYC, financial protection, chat, deletion, and audit controls may be **implemented**.

**Source of truth:**

- `MASTER_IMPLEMENTATION_PROMPT_v1.0.md`
- ADR-022 — Account Retention & Deletion
- `RETENTION_AND_DATA_GOVERNANCE_FRAMEWORK.md`
- `FINAL_SCOPE_BASELINE.md`
- `PAYMENT_SECURITY_AND_RECONCILIATION_FRAMEWORK.md`
- `BLOCKER_EVIDENCE_MANAGEMENT_FRAMEWORK.md`
- `FINAL_IMPLEMENTATION_GATE_REPORT.md`

| Dimension | Status |
|-----------|--------|
| Architecture | **APPROVED** |
| Product scope | **FROZEN** |
| Compliance policies & retention values | **NOT APPROVED** (this package) |
| Implementation | **NOT AUTHORIZED** |

**Explicit exclusions from this deliverable:**

- No production code, database schema, migrations, or deletion workflow implementation
- No encryption implementation
- No vendor selection (including OCR/Face)
- **No legal retention period definitions** — all durations remain *Pending Legal / Compliance Approval*
- No architecture or scope changes
- No new features

Until §10 signatures exist and evidence is archived, **BLOCKER-006 remains open**.

---

## Instructions to Approvers

1. Review §1–§9 for governance alignment with ADR-022 and `RETENTION_AND_DATA_GOVERNANCE_FRAMEWORK.md`
2. Complete retention duration decisions in §2 and §9 (Legal / Compliance authority only)
3. Sign §10 — verbal approval is insufficient
4. Return to Compliance Governance Manager for `COMPLIANCE_APPROVAL_PACK_v1.0` completion and archival

---

## 1. Data Classification Approval

Stakeholders review and approve KHADAMATI data categories for governance purposes. **This section defines classification — not storage implementation.**

### 1.1 Personal Data

| Attribute | Definition |
|-----------|------------|
| **Description** | Identifiable information relating to customers, providers, and platform users |
| **Examples** | Customer information; provider information; contact details; addresses; profile information |
| **Sensitivity level** | **Confidential — Personal** |
| **Access expectation** | Role-based; least privilege; user self-access where applicable |
| **Governance requirements** | Retention per §2; deletion per §3; access audit per §7 |

| Approval | ☐ Approved ☐ Rejected |
|----------|----------------------|

---

### 1.2 Identity Verification Data

| Attribute | Definition |
|-----------|------------|
| **Description** | Data collected and generated for KYC and provider identity verification |
| **Examples** | KYC documents; verification results; identity evidence; provider verification records |
| **Sensitivity level** | **Highly Confidential — Identity** |
| **Access expectation** | Restricted roles only; no general operational access |
| **Governance requirements** | KYC governance per §4; retention per §2; audit per §7 |

| Approval | ☐ Approved ☐ Rejected |
|----------|----------------------|

---

### 1.3 Financial Data

| Attribute | Definition |
|-----------|------------|
| **Description** | Monetary transactions, balances, and financial audit records |
| **Examples** | Payments; ledger records; settlements; withdrawals; commission calculations |
| **Sensitivity level** | **Highly Confidential — Financial (Protected Records)** |
| **Access expectation** | Finance Admin and authorized roles only; full audit trail |
| **Governance requirements** | **Protected records — cannot be silently modified or removed**; retention per §2; payment compliance per §5 |

| Approval | ☐ Approved ☐ Rejected |
|----------|----------------------|

**Mandatory principle:** Financial records are **protected records**. No silent modification or removal.

---

### 1.4 Operational Data

| Attribute | Definition |
|-----------|------------|
| **Description** | Platform operational records supporting service delivery |
| **Examples** | Bookings; reviews; availability; notifications; service listings |
| **Sensitivity level** | **Internal — Operational** |
| **Access expectation** | Role-based by function (customer, provider, store, admin) |
| **Governance requirements** | Retention per §2; booking-scoped chat per §6 |

| Approval | ☐ Approved ☐ Rejected |
|----------|----------------------|

---

### 1.5 Audit Data

| Attribute | Definition |
|-----------|------------|
| **Description** | Immutable or protected records of system and administrative actions |
| **Examples** | Admin actions; finance policy changes; security events; permission changes |
| **Sensitivity level** | **Highly Confidential — Audit** |
| **Access expectation** | Super Admin / Compliance / Security roles; read-focused for most |
| **Governance requirements** | Retention per §2; **cannot be removed without approved policy**; access per §7 |

| Approval | ☐ Approved ☐ Rejected |
|----------|----------------------|

**Attestation (§1):** ☐ Legal / Compliance ☐ Business Owner ☐ Technical Architect

---

## 2. Retention Policy Governance

### 2.1 Approval framework

This section defines **who approves** retention policies — **not** the retention durations themselves.

| Data domain | Retention duration status | Owner | Approval authority | Change control | Audit requirement |
|-------------|---------------------------|-------|-------------------|----------------|-------------------|
| Customer data | **Pending Legal / Compliance Approval** | Legal / Compliance | Legal / Compliance Officer | New version + effective date; no retroactive reduction without legal review | All policy changes logged |
| Provider data | **Pending Legal / Compliance Approval** | Legal / Compliance | Legal / Compliance Officer | As above | As above |
| KYC data | **Pending Legal / Compliance Approval** | Compliance | Legal / Compliance Officer | As above; cross-ref §4 | As above |
| Financial records | **Pending Legal / Compliance Approval** | Finance / Legal | Finance + Legal / Compliance | As above; protected record rules apply | As above + financial audit |
| Booking history | **Pending Legal / Compliance Approval** | Legal / Compliance | Legal / Compliance Officer | As above | As above |
| Chat (booking-scoped) | **Pending Legal / Compliance Approval** | Compliance | Legal / Compliance Officer | As above; cross-ref §6 | As above |
| Audit logs | **Pending Legal / Compliance Approval** | Compliance / Security | Legal / Compliance Officer | As above; minimum retention per security policy | Immutable audit trail |

### 2.2 Governance rules

| Rule | Confirmation |
|------|--------------|
| Engineering **must not** assume retention durations | ☐ Confirmed |
| Durations published only after Legal / Compliance written approval | ☐ Confirmed |
| Policy changes require version history per `RETENTION_AND_DATA_GOVERNANCE_FRAMEWORK.md` | ☐ Confirmed |
| No retroactive application to completed bookings without approved exception | ☐ Confirmed |

**Value entry (Legal / Compliance completes on approval):**

| Domain | Approved duration | Effective from | Approved by | Date |
|--------|-------------------|----------------|-------------|------|
| | *Pending* | | | |

---

## 3. Account Deletion Governance

### 3.1 Approved governance workflow

Stakeholders approve the **process** (not implementation):

```
User deletion request
        ↓
Identity verification
        ↓
Account status review
        ↓
Financial / legal obligation check
        ↓
Deletion or anonymisation decision
        ↓
Execution record
        ↓
Audit preservation
```

| Stage | Governance requirement | Approval |
|-------|------------------------|----------|
| User deletion request | Authenticated request; logged | ☐ Approved |
| Identity verification | Confirm requester identity | ☐ Approved |
| Account status review | Active bookings, disputes, holds | ☐ Approved |
| Financial / legal obligation check | Outstanding ledger, tax, legal holds | ☐ Approved |
| Deletion or anonymisation decision | Per approved policy — not engineering discretion | ☐ Approved |
| Execution record | Immutable record of action taken | ☐ Approved |
| Audit preservation | Audit trail retained per §2 | ☐ Approved |

### 3.2 Confirmations

| Principle | Confirmation |
|-----------|--------------|
| Financial records may require **preservation** despite account deletion request | ☐ Confirmed |
| Audit records **cannot be removed** without approved policy | ☐ Confirmed |
| Deletion must **not break financial traceability** | ☐ Confirmed |
| Alignment with ADR-022 | ☐ Confirmed |

**Asset path (on approval):** `evidence/BLOCKER-006-compliance/deletion/`

---

## 4. KYC and Identity Verification Governance

### 4.1 Governance requirements — approval requested

| Area | Requirement | Approval |
|------|-------------|----------|
| Document collection | Only for defined verification purposes; consent where required | ☐ Approved |
| Document access | Restricted roles; logged access | ☐ Approved |
| Verification status handling | Clear states; no manual override without audit | ☐ Approved |
| Provider verification lifecycle | Submit → review → approve/reject → expiry/re-verification | ☐ Approved |
| Restricted access | Least privilege; separation from general admin | ☐ Approved |
| Audit visibility | Compliance can audit access and status changes | ☐ Approved |

### 4.2 Security expectations

| Expectation | Confirmation |
|-------------|--------------|
| Least privilege access | ☐ Confirmed |
| Controlled access to identity documents | ☐ Confirmed |
| Audit trail on view and change | ☐ Confirmed |
| Protection against unauthorized usage | ☐ Confirmed |

**Explicit exclusion:** This package does **not** select OCR/Face verification vendors (BLOCKER-003 scope).

**Asset path (on approval):** `evidence/BLOCKER-006-compliance/kyc/`

---

## 5. Payment and Financial Compliance

### 5.1 Approved financial governance model

Stakeholders confirm the compliance-aligned payment flow:

```
Booking confirmation
        ↓
Payment through Payment.js / IXOPAY integration
        ↓
Payment confirmation
        ↓
Ledger entry
        ↓
Settlement process
```

### 5.2 Confirmations

| Principle | Confirmation |
|-----------|--------------|
| **Ledger is the financial source of truth** | ☐ Confirmed |
| **Historical financial records are preserved** | ☐ Confirmed |
| **Payment events are auditable** | ☐ Confirmed |
| **No silent modification of financial history** | ☐ Confirmed |
| Finance policies remain **Admin-configurable** with approval workflow (BLOCKER-005) | ☐ Confirmed |
| Alignment with `PAYMENT_SECURITY_AND_RECONCILIATION_FRAMEWORK.md` | ☐ Confirmed |

**Asset path (on approval):** `evidence/BLOCKER-006-compliance/financial-records/`

---

## 6. Chat Governance

### 6.1 V1 chat decision — confirmation requested

| Decision | Status |
|----------|--------|
| **Booking-scoped chat only** | ☐ Confirmed for V1 |

### 6.2 Governance dimensions

| Dimension | Requirement | Approval |
|-----------|-------------|----------|
| **Chat participants** | Customer and provider (and authorized admin per policy) for active booking only | ☐ Approved |
| **Access restrictions** | No access outside booking context | ☐ Approved |
| **Retention governance** | Duration **Pending Legal / Compliance Approval** (§2) | ☐ Approved |
| **Security requirements** | Encrypted in transit; access logged (implementation gated post Gate A) | ☐ Approved |
| **Audit requirements** | Admin access and export per compliance policy | ☐ Approved |

### 6.3 Explicit V1 exclusions

| Excluded | Acknowledgement |
|----------|-----------------|
| Public messaging | ☐ Excluded from V1 |
| Marketplace-wide chat | ☐ Excluded from V1 |
| Unrelated user communication | ☐ Excluded from V1 |

**Asset path (on approval):** `evidence/BLOCKER-006-compliance/chat/`

---

## 7. Access Control Governance

### 7.1 Sensitive data access — confirmation requested

| Requirement | Confirmation |
|-------------|--------------|
| Role-based access control (RBAC) | ☐ Confirmed |
| Least privilege | ☐ Confirmed |
| Audit logging of sensitive access | ☐ Confirmed |
| Periodic access review (operational process) | ☐ Confirmed |

### 7.2 Admin portal rules

| Rule | Confirmation |
|------|--------------|
| Admin portal is **web-only** | ☐ Confirmed |
| **MFA required** for admin access | ☐ Confirmed |
| Sensitive operations require **authorized roles** | ☐ Confirmed |

### 7.3 Role categories (governance level — not permission matrix)

| Role category | Purpose | Governance note |
|---------------|---------|-----------------|
| **Finance Admin** | Financial data, policies, settlements | Highly restricted; full audit |
| **Super Admin** | Platform configuration, user management | MFA; full audit |
| **Operational roles** | Day-to-day operations within scope | Least privilege |

**Note:** Detailed permission implementation is **out of scope** for this package — governed by approved architecture and post–Gate A engineering standards.

**Attestation (§7):** ☐ Legal / Compliance ☐ Technical Architect

---

## 8. Multi-Country Compliance Readiness

### 8.1 Initial market (documentation only)

| Setting | Initial value | Compliance note |
|---------|---------------|-----------------|
| **Country** | Lebanon | Launch market |
| **Currency** | USD | Display/settlement context per finance policy |
| **Phone** | +961 | Default locale |

### 8.2 Readiness confirmations

| Requirement | Confirmation |
|-------------|--------------|
| Architecture supports **additional countries** without code-level legal assumptions | ☐ Confirmed |
| Architecture supports **additional currencies** | ☐ Confirmed |
| **Different regulatory requirements** addressed via policy configuration and legal review per market | ☐ Confirmed |

**Explicit exclusion:** This package does **not** define country-specific legal requirements.

---

## 9. Compliance Risks and Open Items

| Item | Status | Owner |
|------|--------|-------|
| Retention durations | **Pending** | Legal / Compliance |
| KYC requirements (detailed) | **Pending** | Compliance |
| Financial retention rules | **Pending** | Finance / Legal |
| Chat retention rules | **Pending** | Compliance |
| Regulatory review (Lebanon launch) | **Pending** | Legal |
| Account deletion exception catalogue | **Pending** | Legal / Compliance |
| Cross-border data transfer (if applicable) | **Pending** | Legal |

**Risk note:** Implementation of data lifecycle features remains **blocked** until open items are resolved and BLOCKER-006 is **Closed**.

---

## 10. Compliance Approval Record

**BLOCKER-006 status:** **READY FOR APPROVAL** — not **Closed** until signatures and required policy values approved.

| Role | Name | Decision | Date | Signature |
|------|------|----------|------|-----------|
| Legal / Compliance Owner | | **Pending** | | |
| Business Owner | | **Pending** | | |
| Technical Architect | | **Pending** | | |

### Decision values

- **Approved** — Governance framework accepted; retention values completed in §2 where applicable
- **Approved with conditions** — Document conditions below
- **Rejected** — Package returned; BLOCKER-006 remains open

### Comments / conditions

| Role | Comments |
|------|----------|
| Legal / Compliance Owner | |
| Business Owner | |
| Technical Architect | |

---

## 11. Blocker Status

| Item | Status |
|------|--------|
| **BLOCKER-006** | **READY FOR APPROVAL** |
| **Evidence package** | `COMPLIANCE_APPROVAL_PACKAGE.md` (this document) |
| **Retention durations** | **Pending Legal / Compliance Approval** |
| **BLOCKER-006 Closed** | **No** |
| **Gate** | **B — NOT READY — CODING BLOCKED** |
| **Closed blockers** | **0 / 7** |
| **Implementation authorized** | **No** |

### Closure criteria (all required)

- [ ] Data classification §1 approved
- [ ] Retention durations approved by Legal / Compliance and recorded in §2
- [ ] Deletion, KYC, payment, chat, access governance §3–§7 approved
- [ ] Open items §9 resolved or accepted with documented risk acceptance
- [ ] Legal / Compliance Owner — signed §10
- [ ] Business Owner — signed §10
- [ ] Technical Architect — signed §10
- [ ] `COMPLIANCE_APPROVAL_PACK_v1.0.md` completed and archived
- [ ] `READINESS_BLOCKER_CLOSURE_STATUS.md` updated to **Closed**

---

## Document Control

| Version | Date | Author | Change |
|---------|------|--------|--------|
| 1.0 | 2026-07-25 | Compliance Governance Manager | Initial compliance approval package |

**Distribution:** Legal / Compliance Owner, Business Owner, Technical Architect, Program Governance Manager, Finance Governance Manager (informational)
