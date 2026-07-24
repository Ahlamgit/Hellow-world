# KHADAMATI V1 — Vendor Integration Readiness Matrix

**Document ID:** KHAD-V1-VENDOR-INTEGRATION-READINESS  
**Version:** 1.0  
**Date:** 2026-07-24  
**Role:** Integration Architect  
**BLOCKER-003 status:** **IN PREPARATION**  

**Sources:**  
Master Prompt v1.0 · Decisions Complete · Scope Baseline · [`../infra/CLOUD_INFRASTRUCTURE_DECISION.md`](../infra/CLOUD_INFRASTRUCTURE_DECISION.md) · ADR-025 · [`../payment/PAYMENT_JS_MOBILE_VALIDATION_REPORT.md`](../payment/PAYMENT_JS_MOBILE_VALIDATION_REPORT.md) · [`../payment/AREEBA_IXOPAY_VENDOR_VALIDATION_CHECKLIST.md`](../payment/AREEBA_IXOPAY_VENDOR_VALIDATION_CHECKLIST.md) · [`VENDOR_EVALUATION_MATRIX.md`](./VENDOR_EVALUATION_MATRIX.md)

```text
DO NOT write production code.
DO NOT implement integrations.
DO NOT create API clients.
DO NOT select vendors without approval.

Architecture remains vendor-neutral (ports/adapters).
No vendor is approved until business + technical validation completes.
```

---

## Status Snapshot

| Item | State |
|------|-------|
| Architecture | **APPROVED** |
| Implementation | **B) NOT READY — CODING BLOCKED** |
| Vendor selection | **PENDING** |
| BLOCKER-003 | **IN PREPARATION** |
| BLOCKER-003 COMPLETED | ☐ No — until vendors selected, contracts approved, sandbox available, technical validation done |

---

## Purpose

Define the **vendor evaluation framework** and **integration readiness requirements** for all external services used by KHADAMATI.

Companion scoring sheet: [`VENDOR_EVALUATION_MATRIX.md`](./VENDOR_EVALUATION_MATRIX.md)

---

# 1. Integration Architecture Rules

Confirm (ADR-025):

```text
Application Domain
  → Integration Port
  → Adapter Layer
  → External Vendor
```

| Rule | Confirm |
|------|---------|
| No business logic depends directly on vendor APIs/SDKs/DTOs | ☑ Required |
| Domain services call ports only | ☑ Required |
| Vendor SDKs confined to adapter modules | ☑ Required |
| Config selects active adapter (incl. per Market when needed) | ☑ Required |
| Vendor swap must not rewrite booking/ledger/commission logic | ☑ Required |

### Examples

| Port | Adapter (illustrative) |
|------|------------------------|
| `PaymentPort` / `PaymentGatewayPort` | Areeba IXOPAY Adapter |
| `NotificationPort` / `SmsPort` | SMS Provider Adapter |
| `EmailPort` | Email Adapter |
| `StoragePort` / `ObjectStoragePort` | Object Storage Adapter |
| `MapsPort` | Maps / Geocoding Adapter |
| `IdentityVerificationPort` (OCR/Face) | Verification Adapter |

---

# 2. Payment Gateway Evaluation

## 2.1 Current candidate

| Field | Value |
|-------|-------|
| Candidate | **Areeba IXOPAY Payment.js** |
| V1 architecture lock | Payment.js only (behind port) — architecture approved |
| Business/contract approval | **Not complete** until validation + approvals |
| Status | **Pending Vendor Confirmation** (sandbox/runtime) |
| Evidence | [`../payment/AREEBA_IXOPAY_VENDOR_VALIDATION_CHECKLIST.md`](../payment/AREEBA_IXOPAY_VENDOR_VALIDATION_CHECKLIST.md) · BLOCKER-007 **IN VALIDATION** |

## 2.2 Technical validation required

| Item | State |
|------|-------|
| Sandbox access | **Pending Vendor Confirmation** |
| Payment.js hosted fields validation | **Pending Vendor Confirmation** |
| Android WebView validation | Pending (BLOCKER-007 live) |
| iOS WebView validation | Pending (BLOCKER-007 live) |
| JavaScript bridge validation | Pending (BLOCKER-007 live) |
| 3DS flow validation | **Pending Vendor Confirmation** |
| Redirect handling | **Pending Vendor Confirmation** |
| Webhook validation | **Pending Vendor Confirmation** |
| Signature verification | **Pending Vendor Confirmation** |
| Retry handling | **Pending Vendor Confirmation** |
| Idempotency handling | Architecture required; vendor event ids PVC |

## 2.3 KHADAMATI money flow

```text
Customer Payment
  → Payment Gateway (Payment.js → IXOPAY)
  → Payment Confirmation (webhook + verified debit)
  → Ledger Entry
  → Commission Calculation (Admin policy)
  → Provider Balance
  → Settlement (Admin policy)
```

Domain owns ledger/commission/settlement; adapter owns gateway I/O only.

## 2.4 Evaluation criteria

| Criterion | Notes | Status |
|-----------|-------|--------|
| Lebanon support | Merchant/onboarding | **Pending Vendor Confirmation** |
| USD support | ADR-001 default | **Pending Vendor Confirmation** |
| Multi-country readiness | Future Markets | **Pending Vendor Confirmation** |
| Security compliance | Docs / attestations | **Pending Vendor Confirmation** |
| PCI minimization | Hosted fields / no PAN in KHADAMATI | Architecture confirmed; runtime PVC |
| Webhook reliability | Retries, ordering | **Pending Vendor Confirmation** |
| Refund support | Full/partial API | **Pending Vendor Confirmation** |
| Transaction reporting | Lookup/status | **Pending Vendor Confirmation** |

**Approval owners:** Business + Technical (+ Finance for money rails)

---

# 3. SMS Provider Evaluation

**Required for:** OTP · Booking notifications · Provider alerts · Security notifications  

| Evaluate | Status |
|----------|--------|
| Lebanon coverage | **Pending Vendor Selection** |
| International coverage | **Pending Vendor Selection** |
| Delivery reliability | **Pending Vendor Selection** |
| Sender ID support | **Pending Vendor Selection** |
| Cost model | **Pending Vendor Selection** |
| API availability | **Pending Vendor Selection** |
| Retry support | **Pending Vendor Selection** |

```text
NotificationPort / SmsPort
  → SMS Adapter
  → Vendor
```

**Status:** **Pending Vendor Selection**  
**Approval owner:** Technical (+ Product for UX/SLA)

---

# 4. Email Provider Evaluation

**Required for:** Account communication · Notifications · Admin alerts · Reports  

| Evaluate | Status |
|----------|--------|
| SMTP/API support | **Pending Vendor Selection** |
| Deliverability | **Pending Vendor Selection** |
| Templates | **Pending Vendor Selection** |
| Tracking | **Pending Vendor Selection** |
| Bounce handling | **Pending Vendor Selection** |
| International support | **Pending Vendor Selection** |

```text
EmailPort
  → Email Adapter
  → Vendor
```

**Status:** **Pending Vendor Selection**  
**Approval owner:** Technical

---

# 5. Maps / GPS Provider Evaluation

**Required for:** Provider location · Service areas · Distance · Availability search · Arrival verification  

| Evaluate | Status |
|----------|--------|
| Lebanon map quality | **Pending Vendor Selection** |
| Regional coverage | **Pending Vendor Selection** |
| Geocoding | **Pending Vendor Selection** |
| Routing / distance | **Pending Vendor Selection** |
| Mobile SDK / client support | **Pending Vendor Selection** |
| Cost scalability | **Pending Vendor Selection** |

```text
MapsPort
  → Maps Adapter
  → Vendor
```

**Status:** **Pending Vendor Selection**  
**Approval owners:** Product + Technical

---

# 6. OCR / Face Verification Provider Evaluation

**Required for:** Provider onboarding · Identity documents · Verification workflow · Fraud prevention  

| Evaluate | Status |
|----------|--------|
| Lebanon document support | **Pending Vendor Selection** |
| Accuracy | **Pending Vendor Selection** |
| API security | **Pending Vendor Selection** |
| Data retention | **Pending Vendor Selection** |
| Compliance | **Pending Vendor Selection** |
| Cost | **Pending Vendor Selection** |

```text
IdentityVerificationPort
  → Verification Adapter (OCR / Face)
  → Vendor(s)
```

Phased deferral only with Product risk acknowledgement (see Evaluation Matrix).

**Status:** **Pending Vendor Selection**  
**Approval owner:** Compliance (+ Technical / Product)

---

# 7. Storage Provider Evaluation

**Required for:** Provider documents · Identity files · Service images · Attachments  

| Evaluate | Status |
|----------|--------|
| Encryption | **Pending Vendor Selection** |
| Access control | **Pending Vendor Selection** |
| Lifecycle policies | **Pending Vendor Selection** |
| Retention support | Align BLOCKER-006 |
| Backup options | Align BLOCKER-004 |
| Regional availability | Lebanon + expansion |

```text
StoragePort
  → Storage Adapter
  → Vendor
```

Align with cloud object storage decision (BLOCKER-004) — may be same provider as cloud or separate; still behind port.

**Status:** **Pending Vendor Selection**  
**Approval owner:** Technical (+ Security / Compliance for KYC)

---

# 8. Vendor Evaluation Matrix (summary)

| Integration | Purpose | Candidate | Status | Approval Owner |
|-------------|---------|-----------|--------|----------------|
| Payment | Customer payments | Areeba IXOPAY Payment.js (architecture candidate) | **Pending Vendor Confirmation** | Business + Technical |
| SMS | OTP / alerts | Pending | **Pending Vendor Selection** | Technical |
| Email | Notifications | Pending | **Pending Vendor Selection** | Technical |
| Maps | Location services | Pending | **Pending Vendor Selection** | Product + Technical |
| OCR/Face | Verification | Pending | **Pending Vendor Selection** | Compliance |
| Storage | Files / media | Pending | **Pending Vendor Selection** | Technical |

Detail scoring: [`VENDOR_EVALUATION_MATRIX.md`](./VENDOR_EVALUATION_MATRIX.md)

---

# 9. Security Requirements

Every vendor **must** provide (or document equivalent):

| Requirement |
|-------------|
| Data protection information |
| Encryption support |
| Authentication method (API keys, mTLS, OAuth, etc.) |
| Access controls |
| Logging capability |
| Compliance documentation |

KHADAMATI: secrets in secrets manager; no vendor credentials in git; adapters verify webhooks where applicable.

---

# 10. Data Residency & Compliance

Vendor evaluation **must** consider:

| Topic |
|-------|
| Lebanon requirements |
| Regional expansion (Market model) |
| User data protection |
| Provider identity documents |
| Financial records (payment metadata; ledger stays in KHADAMATI DB) |

Retention of KYC/media: Admin/Compliance policy (ADR-022 / BLOCKER-006) — vendor retention must not conflict.

---

# 11. Vendor Approval Workflow

```text
Technical evaluation
  → Security review
  → Business approval
  → Contract approval
  → Integration authorization
```

| Step | Owner | Output |
|------|-------|--------|
| Technical evaluation | Eng / Integration Architect | Fit vs ports, sandbox proof |
| Security review | Security | Risk accept / reject |
| Business approval | Product / Business | Cost, SLA, market fit |
| Contract approval | Business / Legal | Signed terms |
| Integration authorization | Architect + Eng Lead | Adapter implementation allowed **after** Implementation Gate → A (or explicit phased waiver) |

**No silent vendor lock-in** in domain code at any step.

---

# 12. Criteria to mark BLOCKER-003 COMPLETED

- [ ] Vendors selected (or deferred with Product risk ack) for required categories  
- [ ] Contracts approved (as applicable)  
- [ ] Sandbox credentials available (Payment minimum; others as needed)  
- [ ] Technical validation completed (Payment: checklist + BLOCKER-007 conditions)  
- [ ] Ports/adapters ownership confirmed  

Until then: keep **IN PREPARATION**.

---

# 13. Explicit Non-Goals

- No production integration code  
- No API client generation as product code  
- No unauthorized vendor selection  
- No business logic coupled to vendor SDKs  

---

**End of Vendor Integration Readiness Matrix v1.0**
