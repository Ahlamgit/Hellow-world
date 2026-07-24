# KHADAMATI V1 — Vendor Evaluation Matrix

**Document ID:** KHAD-V1-VENDOR-MATRIX  
**Version:** 1.1  
**Date:** 2026-07-24  
**Status:** **IN PREPARATION** — selections pending (BLOCKER-003)  
**ADR:** ADR-025  

**Related:** [`VENDOR_INTEGRATION_READINESS_MATRIX.md`](./VENDOR_INTEGRATION_READINESS_MATRIX.md) · [`../READINESS_BLOCKER_CLOSURE_STATUS.md`](../READINESS_BLOCKER_CLOSURE_STATUS.md)

```text
All integrations MUST use ports/adapters.
NO vendor-specific business logic in domain services.
No vendor approved until business + technical validation completes.
```

**Integration readiness framework:** [`VENDOR_INTEGRATION_READINESS_MATRIX.md`](./VENDOR_INTEGRATION_READINESS_MATRIX.md) — ports, money flow, approval workflow, security/residency.

---

## Summary Matrix

| Integration | Purpose | Candidate | Status | Approval Owner |
|-------------|---------|-----------|--------|----------------|
| Payment | Customer payments | Areeba IXOPAY Payment.js (architecture candidate) | **Pending Vendor Confirmation** | Business + Technical |
| SMS | OTP / alerts | Pending | **Pending Vendor Selection** | Technical |
| Email | Notifications | Pending | **Pending Vendor Selection** | Technical |
| Maps | Location services | Pending | **Pending Vendor Selection** | Product + Technical |
| OCR/Face | Verification | Pending | **Pending Vendor Selection** | Compliance |
| Storage | Files / media | Pending | **Pending Vendor Selection** | Technical |

BLOCKER-003 remains **IN PREPARATION** until vendors selected, contracts approved, sandbox credentials available, and technical validation completed.

---

## Evaluation Criteria (all categories)

| Criterion | Weight guide |
|-----------|--------------|
| Functional fit (OTP, webhooks, RTL SMS, etc.) | High |
| Lebanon / regional deliverability & compliance | High |
| Security & credential model | High |
| Cost predictability | Medium |
| SLA / support | Medium |
| Adapter complexity | Medium |
| Exit / portability | High |

**Decision outcomes:** Selected · Deferred (with Product risk ack) · Rejected  

---

## 1. Payment — Areeba IXOPAY (Locked)

**Evidence pack:** [`../payment/AREEBA_IXOPAY_VENDOR_VALIDATION_CHECKLIST.md`](../payment/AREEBA_IXOPAY_VENDOR_VALIDATION_CHECKLIST.md)  
**Mobile spike:** [`../payment/PAYMENT_JS_MOBILE_VALIDATION_REPORT.md`](../payment/PAYMENT_JS_MOBILE_VALIDATION_REPORT.md) — PASS WITH CONDITIONS · BLOCKER-007 **IN VALIDATION**

| Item | Status | Notes |
|------|--------|-------|
| Gateway locked for V1 | ✅ | Payment.js only |
| Port | `PaymentGatewayPort` | Adapter: IXOPAY |
| Sandbox availability | ☐ Validate | Checklist §1 — PVC |
| Payment.js requirements | ☐ Validate | Checklist §2 — PVC |
| Mobile flow | ☐ Validate | Checklist §3 + BLOCKER-007 |
| Webhook process | ☐ Validate | Checklist §5 — PVC |
| Credentials process | ☐ Validate | Checklist §1.3 — secrets manager |

| Field | Value |
|-------|-------|
| Sandbox account owner | **Pending Vendor Confirmation** |
| Webhook URL pattern (staging) | **Pending Vendor Confirmation** |
| Credential custody | **Pending Vendor Confirmation** |
| Validation owner | Eng |
| Validated date | |

---

## 2. SMS

**Requirements:** OTP · Booking notifications · Reminders  

| Candidate | OTP | Booking notify | Reminders | Lebanon fit | Cost | Adapter effort | Score | Decision |
|-----------|-----|----------------|-----------|-------------|------|----------------|-------|----------|
| Option A (name) | ☐ | ☐ | ☐ | ☐ | | | /10 | |
| Option B (name) | ☐ | ☐ | ☐ | ☐ | | | /10 | |
| Option C (name) | ☐ | ☐ | ☐ | ☐ | | | /10 | |

| Field | Value |
|-------|-------|
| Selected vendor | TBD |
| Port | `SmsPort` |
| Deferral ack (if any) | |
| Approved by | |
| Date | |

---

## 3. Email

**Requirements:** Verification · Notifications · Reports  

| Candidate | Verify | Notify | Reports/attachments | Deliverability | Cost | Adapter effort | Score | Decision |
|-----------|--------|--------|---------------------|----------------|------|----------------|-------|----------|
| Option A | ☐ | ☐ | ☐ | | | | /10 | |
| Option B | ☐ | ☐ | ☐ | | | | /10 | |
| Option C | ☐ | ☐ | ☐ | | | | /10 | |

| Field | Value |
|-------|-------|
| Selected vendor | TBD |
| Port | `EmailPort` |
| Approved by | |
| Date | |

---

## 4. Storage (Object)

**Requirements:** Images · Documents · Attachments  

| Candidate | Images | Docs | Attachments | Security | Cost | Region options | Score | Decision |
|-----------|--------|------|-------------|----------|------|----------------|-------|----------|
| Option A | ☐ | ☐ | ☐ | | | | /10 | |
| Option B | ☐ | ☐ | ☐ | | | | /10 | |
| Option C | ☐ | ☐ | ☐ | | | | /10 | |

| Field | Value |
|-------|-------|
| Selected vendor | TBD |
| Port | `ObjectStoragePort` |
| Align with BLOCKER-004 storage hosting | Yes |
| Approved by | |
| Date | |

---

## 5. Maps

**Requirements:** Location · Distance · Service areas  

| Candidate | Geocode/places | Distance | Service area | Licensing | Cost | Score | Decision |
|-----------|----------------|----------|--------------|-----------|------|-------|----------|
| Option A | ☐ | ☐ | ☐ | | | /10 | |
| Option B | ☐ | ☐ | ☐ | | | /10 | |
| Option C | ☐ | ☐ | ☐ | | | /10 | |

| Field | Value |
|-------|-------|
| Selected vendor | TBD |
| Port | `MapsPort` |
| Approved by | |
| Date | |

---

## 6. OCR / Face Verification

**Requirements:** Provider onboarding · Identity verification  

| Candidate | OCR docs | Face/liveness | Privacy | Cost | Adapter effort | Score | Decision |
|-----------|----------|---------------|---------|------|----------------|-------|----------|
| Option A | ☐ | ☐ | | | | /10 | |
| Option B | ☐ | ☐ | | | | /10 | |
| Option C | ☐ | ☐ | | | | /10 | |

| Field | Value |
|-------|-------|
| OCR vendor | TBD / Deferred |
| Face vendor | TBD / Deferred |
| Ports | `OcrPort` · `FaceVerificationPort` |
| Product risk ack if deferred | ☐ |
| Approved by | |
| Date | |

---

## 7. Adapter Compliance Checklist

| Rule | Confirmed |
|------|-----------|
| Domain services call ports only | ☐ |
| Vendor SDKs confined to adapter modules | ☐ |
| Config keys / credentials outside domain | ☐ |
| Swap vendor without rewriting booking/ledger/commission | ☐ |

---

## 8. Approval

| Role | Name | Date | Decision |
|------|------|------|----------|
| Engineering Lead | | | ☐ |
| Business / Product | | | ☐ |
| Security | | | ☐ |
| Solution Architect | | | ☐ Acknowledge ADR-025 |

**BLOCKER-003 closed only when Payment validation rows + required categories are Selected or Deferred with ack.**

---

**End of Vendor Evaluation Matrix**
