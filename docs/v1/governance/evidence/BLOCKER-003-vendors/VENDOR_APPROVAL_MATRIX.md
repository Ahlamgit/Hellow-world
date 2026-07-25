# BLOCKER-003 — Vendor Approval Matrix

| Field | Value |
|-------|-------|
| **Document ID** | EVD-003-MATRIX-001 |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |
| **Blocker** | BLOCKER-003 — Vendors |
| **Evidence status** | **Prepared** — awaiting technical validation |
| **Business approver** | Project Owner / Business Owner |

---

## Vendor capability matrix

| Vendor capability | Business decision | V1 implementation | Technical validation |
|-------------------|-------------------|-------------------|----------------------|
| SMS verification (OTP) | **Approved** | Allowed — purpose-limited | **Pending** — Integration Lead + TA |
| Email verification | **Approved** | Allowed — purpose-limited | **Pending** |
| Account security emails | **Approved** | Allowed | **Pending** |
| Storage (images, documents, attachments, verification files) | **Approved** | Allowed | **Pending** |
| Marketing SMS | **Not approved** | **Forbidden** | N/A |
| Marketing / campaign email | **Not approved** | **Forbidden** | N/A |
| Maps / location services | **Excluded V1** | **Forbidden** | N/A — see `MAPS_V1_EXCLUSION_RECORD.md` |
| Payment gateway (IXOPAY) | Business approved (BLOCKER-007) | Pending technical dossier | **Pending** |
| OCR / face verification | Not in business matrix | Evaluation pending | **Pending** |

---

## SMS provider — purpose scope

**ONLY:**

- Phone verification
- OTP delivery

**NOT:**

- Marketing SMS
- Promotional notifications
- Campaign messaging

---

## Email provider — purpose scope

**ONLY:**

- Email verification
- Account security emails

**NOT:**

- Marketing campaigns
- Marketing automation (without future governance approval)

---

## Storage provider — purpose scope

- Images
- Documents
- Attachments
- Provider verification files

**Requirements:** Secure access · backup policy · permission control.

---

## Required future approvals

| Role | Responsibility | Status |
|------|----------------|--------|
| Integration Lead | Vendor dossier · sandbox evidence | **Pending** |
| Technical Architect | Ports/adapters compliance · final approval | **Pending** |

**Closure artifact:** `VENDOR_READINESS_DOSSIER_v1.0` — not filed.

**BLOCKER-003 NOT CLOSED.**
