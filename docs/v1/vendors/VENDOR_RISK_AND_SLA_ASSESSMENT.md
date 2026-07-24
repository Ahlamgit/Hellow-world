# KHADAMATI V1 — Vendor Risk & SLA Assessment

**Document ID:** KHAD-V1-VENDOR-RISK-SLA  
**Version:** 1.0  
**Date:** 2026-07-24  
**Role:** Vendor Governance Architect  
**BLOCKER-003 status:** **IN PREPARATION**  

**Sources:**  
[`VENDOR_INTEGRATION_READINESS_MATRIX.md`](./VENDOR_INTEGRATION_READINESS_MATRIX.md) · [`INTEGRATION_CONTRACT_SPECIFICATION.md`](./INTEGRATION_CONTRACT_SPECIFICATION.md) · [`VENDOR_EVALUATION_MATRIX.md`](./VENDOR_EVALUATION_MATRIX.md) · ADR-025 · Master Prompt v1.0

```text
DO NOT write code.
DO NOT integrate vendors.
DO NOT select vendors.
DO NOT create vendor accounts.

This document defines the risk & SLA evaluation framework only.
Numeric SLA targets and vendor scores remain Pending Vendor Selection.
```

---

## Status Snapshot

| Item | State |
|------|-------|
| Vendor evaluation | **IN PREPARATION** |
| Integration contracts | **DEFINED** ([`INTEGRATION_CONTRACT_SPECIFICATION.md`](./INTEGRATION_CONTRACT_SPECIFICATION.md)) |
| Vendors | **NOT SELECTED** |
| Architecture | **APPROVED** (ports/adapters — ADR-025) |
| BLOCKER-003 | **IN PREPARATION** |

---

## Purpose

Define the **risk evaluation framework** required before approving external KHADAMATI vendors.

Use this document to:

- Score candidates consistently across categories  
- Record residual risk before Business/Security approval  
- Confirm exit/replacement readiness (ports + data portability)  
- Avoid approving vendors without SLA and continuity evidence  

---

# 1. Vendor Risk Categories

Every candidate vendor must be assessed against the categories below.  
Scores and residual ratings: **Pending Vendor Selection**.

## 1.1 Availability Risk

| Consider | Questions |
|----------|-----------|
| Service downtime | Historical / contractual uptime? Regional impact on Lebanon users? |
| Outage handling | Status page, failover, degraded modes? |
| Business impact | Which KHADAMATI flows stop if this vendor is down? |

| Severity guide | When |
|----------------|------|
| Critical | Payment confirmations, OTP delivery, KYC blocking onboarding |
| High | Maps search degradation, email delays |
| Medium | Non-critical notifications / reporting |

## 1.2 Security Risk

| Consider | Questions |
|----------|-----------|
| Data protection | What customer/provider data leaves KHADAMATI? |
| Encryption | TLS; at-rest encryption for stored objects/secrets |
| Authentication | API keys, OAuth, mTLS; rotation support |
| Access control | Least privilege; audit of vendor console access |

Align with security requirements in [`INTEGRATION_CONTRACT_SPECIFICATION.md`](./INTEGRATION_CONTRACT_SPECIFICATION.md) §8.

## 1.3 Compliance Risk

| Consider | Questions |
|----------|-----------|
| User data | Privacy notices, subprocessors, residency options |
| KYC documents | Handling of identity images/docs; retention conflict check (BLOCKER-006) |
| Financial information | Payment metadata only — no PAN in KHADAMATI; vendor PCI posture |
| Retention requirements | Can KHADAMATI enforce Admin/Compliance retention vs vendor defaults? |

## 1.4 Business Continuity Risk

| Consider | Questions |
|----------|-----------|
| Vendor dependency | Single point of failure for a critical flow? |
| Replacement difficulty | Port already defined? Adapter surface small? |
| Migration capability | Export data/configs; dual-run period possible? |

**Mitigation baseline (architecture):** ADR-025 ports/adapters — domain must remain replaceable even if a vendor fails commercial or operational review later.

---

# 2. Service Level Requirements

Evaluation criteria for commercial/technical SLA review.  
**All values:** **Pending Vendor Selection** (do not invent percentages or minutes).

| Criterion | What to capture from vendor | Value |
|-----------|----------------------------|-------|
| Availability target | Contractual uptime / credits | **Pending Vendor Selection** |
| Support response time | Sev-1 / Sev-2 / Sev-3 response & restore | **Pending Vendor Selection** |
| Incident notification | How/when KHADAMATI is notified of outages | **Pending Vendor Selection** |
| Maintenance communication | Advance notice window; maintenance windows | **Pending Vendor Selection** |
| Escalation process | Named contacts; escalation ladder; after-hours | **Pending Vendor Selection** |

### Recording template (per vendor — fill after selection)

| Field | Entry |
|-------|-------|
| Vendor name | |
| Integration type | Payment / SMS / Email / Maps / OCR-Face / Storage |
| Availability target | **Pending Vendor Selection** |
| Support response (Sev-1) | **Pending Vendor Selection** |
| Incident notification | **Pending Vendor Selection** |
| Maintenance communication | **Pending Vendor Selection** |
| Escalation process | **Pending Vendor Selection** |
| Residual risk | Low / Medium / High / Critical |
| Accepted by | Tech · Security · Business |

---

# 3. Integration-Specific Risks

Assess each row before approval. Status remains **Pending** until a named vendor is under evaluation.

## 3.1 Payment Provider

| Risk | Evaluate | Status |
|------|----------|--------|
| Transaction failures | Decline rates, clear failure codes, user messaging path | **Pending** |
| Webhook delays | Ordering, retries, signature verify, reconcile jobs | **Pending** |
| Settlement delays | Impact on provider balances / Admin settlement | **Pending** |
| Refund capability | Full/partial; timing; mismatch with Admin policy | **Pending** |
| Reconciliation support | Lookup by merchant id / gateway ref | **Pending** |

Domain money flow after confirmation stays in KHADAMATI (ledger → commission → settlement). Vendor risk ends at payment rail reliability.

## 3.2 SMS Provider

| Risk | Evaluate | Status |
|------|----------|--------|
| Delivery failure | Failure callbacks; dead-letter; user retry UX | **Pending** |
| Regional coverage | Lebanon (+ future Markets) | **Pending** |
| OTP reliability | Latency, sender ID, spoofing/abuse controls | **Pending** |

## 3.3 Email Provider

| Risk | Evaluate | Status |
|------|----------|--------|
| Deliverability | Inbox placement for transactional mail | **Pending** |
| Reputation | Domain/IP reputation management | **Pending** |
| Bounce handling | Hard/soft bounce → suppress lists; Admin visibility | **Pending** |

## 3.4 Maps Provider

| Risk | Evaluate | Status |
|------|----------|--------|
| Coverage | Lebanon map quality; regional expansion | **Pending** |
| Accuracy | Geocode/distance for availability & service areas | **Pending** |
| Availability | Quota limits; degraded search behaviour | **Pending** |

## 3.5 Identity Verification Provider

| Risk | Evaluate | Status |
|------|----------|--------|
| Verification accuracy | Document/face quality for Lebanon docs | **Pending** |
| False positives / negatives | Manual review path; Admin override audit | **Pending** |
| Data handling | Retention, residency, subprocessors, deletion | **Pending** |

## 3.6 Storage Provider

| Risk | Evaluate | Status |
|------|----------|--------|
| Data availability | Durability/availability claims | **Pending** |
| Backup | Versioning / backup options | **Pending** |
| Recovery | Restore path aligned with BLOCKER-004 RPO/RTO (when set) | **Pending** |

---

# 4. Vendor Replacement Strategy

Required capability **before** long-term dependency is accepted:

| Capability | Requirement |
|------------|-------------|
| Port-based replacement | Domain calls ports only (ADR-025); new adapter can replace old without rewriting booking/ledger/commission |
| Data export capability | Ability to export or migrate objects, templates, verification artifacts, or payment references as applicable |
| Migration planning | Dual-run / cutover plan documented for Critical integrations (especially Payment, Storage, KYC) |
| Avoid proprietary dependency | No vendor-specific types in domain; no irreversible proprietary formats for core business records |

```text
Replacement path:

Domain (unchanged)
  → Port (stable contract)
  → New Adapter
  → New Vendor
```

Silent vendor switches for Payment are **not** allowed (see Integration Contract §7).

---

# 5. Approval Matrix

| Vendor Type | Technical Approval | Security Approval | Business Approval |
|-------------|--------------------|-------------------|-------------------|
| Payment | **Pending** | **Pending** | **Pending** |
| SMS | **Pending** | **Pending** | **Pending** |
| Email | **Pending** | **Pending** | **Pending** |
| Maps | **Pending** | **Pending** | **Pending** |
| OCR/Face | **Pending** | **Pending** | **Pending** |
| Storage | **Pending** | **Pending** | **Pending** |

### Approval meaning

| Approval | Owner focus |
|----------|-------------|
| Technical | Fit to port contract, sandbox validation, ops/SLA feasibility |
| Security | Data protection, auth, encryption, audit, residual risk accept |
| Business | Cost, market fit, commercial terms, continuity risk accept |

Workflow remains:

```text
Technical evaluation
  → Security review
  → Business approval
  → Contract approval
  → Integration authorization
```

Compliance co-approval required for OCR/Face (and KYC storage paths) before Business finalizes.

---

# 6. Relationship to BLOCKER-003

This document advances **risk & SLA readiness** only.

BLOCKER-003 remains **IN PREPARATION** until:

- [ ] Vendors selected (or deferred with Product risk ack)  
- [ ] Risk/SLA rows filled with real vendor evidence  
- [ ] Technical + Security + Business approvals recorded  
- [ ] Commercial contracts approved  
- [ ] Sandbox credentials available  
- [ ] Technical validation completed  

---

# 7. Explicit Non-Goals

- No code  
- No vendor integrations  
- No vendor selection  
- No vendor account creation  
- No invented SLA percentages or commercial terms  

---

**End of Vendor Risk & SLA Assessment v1.0**
