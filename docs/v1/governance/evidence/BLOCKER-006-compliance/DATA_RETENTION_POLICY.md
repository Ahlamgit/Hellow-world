# KHADAMATI — Data Retention Policy (Governance Draft)

| Field | Value |
|-------|-------|
| **Document ID** | EVD-006-RETENTION-001 |
| **Blocker** | BLOCKER-006 — Compliance |
| **Version** | 1.1 |
| **Status** | **APPROVED** — testing retention defined; production durations pending pre-launch legal review |
| **Gate** | B — NOT READY — CODING BLOCKED |
| **Prepared by** | Compliance Governance Manager |
| **Date** | 2026-07-25 |
| **Companion** | `COMPLIANCE_POLICY.md` · `COMPLIANCE_APPROVAL_PACKAGE.md` §2 |

```text
PLACEHOLDER POLICY FRAMEWORK — Durations MUST be completed by Legal / Compliance.
Engineering MUST NOT implement retention jobs with assumed values.
```

---

## 1. Purpose

Define the **data retention governance framework** for KHADAMATI V1. This document establishes data domains, approval authority, and placeholder fields for retention durations pending Legal / Compliance sign-off.

---

## 2. Data domains and retention framework

| Data domain | Data included | Retention duration | Deletion rules | Legal exceptions | Audit retention | Approval authority |
|-------------|---------------|-------------------|----------------|------------------|-----------------|-------------------|
| **Account data** | Customer and provider profiles, contact details | **Pending Legal / Compliance Approval** | Per approved deletion policy | **Pending** | Per security policy | Legal / Compliance |
| **Booking data** | Bookings, status history, schedules, reviews | **Pending Legal / Compliance Approval** | Anonymisation where permitted | **Pending** | Per audit policy | Legal / Compliance |
| **Payment references** | Payment records, gateway references, status | **Pending Legal / Compliance Approval** | Protected — no casual deletion | **Pending** | Financial audit required | Finance + Legal |
| **Ledger / financial records** | Ledger entries, settlements, commission lines | **Pending Legal / Compliance Approval** | Immutable; reversal only | **Pending** | Mandatory financial audit | Finance + Legal |
| **KYC / identity verification** | Documents, verification cases, results | **Pending Legal / Compliance Approval** | Restricted deletion | **Pending** | Compliance audit | Legal / Compliance |
| **Chat (booking-scoped)** | Messages, read status for active bookings | **Pending Legal / Compliance Approval** | Per chat policy | **Pending** | Access logged | Legal / Compliance |
| **Audit records** | Admin actions, policy changes, security events | **Pending Legal / Compliance Approval** | Cannot remove without approved policy | **Pending** | Self-referential minimum | Legal / Compliance |
| **Security logs** | Authentication, access, security events | **Pending Legal / Compliance Approval** | Per security baseline | **Pending** | Security operations | Security + Legal |

---

## 3. Governance rules

| Rule | Confirmation |
|------|--------------|
| Engineering **must not** assume retention durations | Required |
| Durations published only after Legal / Compliance written approval | Required |
| Policy changes require version history and effective date | Required |
| No retroactive reduction without legal review | Required |
| Financial records are **protected records** | Required |

---

## 3A. Testing / localhost environment retention (APPROVED)

**Authority:** Ahlam — Legal / Compliance Officer · **2026-07-25**

| Data type | Environment | Retention | Notes |
|-----------|-------------|-----------|-------|
| All test/sandbox data | **localhost / dev only** | **1 month** | Synthetic or test accounts only — purge after 30 days |
| Sandbox payment logs | IXOPAY sandbox | Per sandbox + **1 month** local copies max | No production PII |

```text
This 1-month rule applies ONLY to non-production test data.
Production retention values in §6 remain pending pre-launch legal review.
```

---

## 4. Deletion rules (framework — values pending)

| Scenario | Governance approach | Duration / rule |
|----------|---------------------|-----------------|
| User account deletion request | Verify → check obligations → delete or anonymise | Per `COMPLIANCE_POLICY.md` §5 |
| Booking data after retention period | **Pending Legal / Compliance** | — |
| Payment / ledger data | Preserve for financial and legal obligations | **Pending Legal / Compliance** |
| KYC documents after verification | **Pending Legal / Compliance** | — |
| Chat messages post-booking | **Pending Legal / Compliance** | — |
| Audit logs | Minimum retention per security/compliance policy | **Pending Legal / Compliance** |

---

## 5. Legal exceptions (placeholder)

| Exception type | Description | Approved by | Status |
|----------------|-------------|-------------|--------|
| Legal hold | Litigation or regulatory hold suspends deletion | Legal | **Pending** |
| Financial obligation | Outstanding ledger/tax obligations | Finance / Legal | **Pending** |
| Active dispute | Booking or payment dispute in progress | Operations / Legal | **Pending** |
| Regulatory requirement | Market-specific mandatory retention | Legal | **Pending** |

---

## 6. Value entry (Legal / Compliance completes on approval)

| Domain | Approved duration | Effective from | Approved by | Date |
|--------|-------------------|----------------|-------------|------|
| Account data | *Pending* | | | |
| Booking data | *Pending* | | | |
| Payment references | *Pending* | | | |
| Ledger / financial records | *Pending* | | | |
| KYC / identity verification | *Pending* | | | |
| Chat (booking-scoped) | *Pending* | | | |
| Audit records | *Pending* | | | |
| Security logs | *Pending* | | | |

---

## 7. Approval

**Effective only after BLOCKER-006 Closed and §6 completed.**

| Role | Name | Decision | Date | Signature |
|------|------|----------|------|-----------|
| Legal / Compliance Officer | **Ahlam** | **Approved** (testing §3A) | **2026-07-25** | Recorded |
| Business Owner | **Ahlam** | **Acknowledged** | **2026-07-25** | Recorded |
| Technical Architect | **Ahlam** | **Approved** implementability | **2026-07-25** | Recorded |

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | Initial retention policy framework — durations pending Legal approval |
| 1.1 | 2026-07-25 | Legal approves **1 month** retention for **non-production test data only** (§3A) |
