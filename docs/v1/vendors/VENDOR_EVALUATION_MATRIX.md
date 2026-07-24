# KHADAMATI V1 — Vendor Evaluation Matrix

**Document ID:** KHAD-V1-VENDOR-MATRIX  
**Version:** 1.0  
**Date:** 2026-07-24  
**Status:** Preparation — selections pending (BLOCKER-003)  
**ADR:** ADR-025  

**Related:** [`../READINESS_BLOCKER_CLOSURE_STATUS.md`](../READINESS_BLOCKER_CLOSURE_STATUS.md)

```text
All integrations MUST use ports/adapters.
NO vendor-specific business logic in domain services.
```

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

| Item | Status | Notes |
|------|--------|-------|
| Gateway locked for V1 | ✅ | Payment.js only |
| Port | `PaymentGatewayPort` | Adapter: IXOPAY |
| Sandbox availability | ☐ Validate | |
| Payment.js requirements | ☐ Validate | Docs, CSP, origins |
| Mobile flow | ☐ Validate | Ties to BLOCKER-007 |
| Webhook process | ☐ Validate | Auth, idempotency, retries |
| Credentials process | ☐ Validate | Secrets manager; rotation |

| Field | Value |
|-------|-------|
| Sandbox account owner | TBD |
| Webhook URL pattern (staging) | TBD |
| Credential custody | TBD |
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
