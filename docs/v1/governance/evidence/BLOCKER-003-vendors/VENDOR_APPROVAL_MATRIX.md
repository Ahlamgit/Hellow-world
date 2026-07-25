# BLOCKER-003 — Vendor Approval Matrix

| Field | Value |
|-------|-------|
| **Document ID** | EVD-003-MATRIX-001 |
| **Version** | 1.1 |
| **Date** | 2026-07-25 |
| **Blocker** | BLOCKER-003 — Vendors |
| **Decision** | **APPROVED WITH V1 RESTRICTIONS** |
| **Business approver** | Project Owner / Business Owner |
| **Blocker closure** | **NOT CLOSED** — technical validation pending |

---

## Vendor capability matrix

| Vendor capability | Business decision | V1 implementation | Technical validation |
|-------------------|-------------------|-------------------|----------------------|
| SMS — phone / OTP / account verification | **Approved** | Allowed — purpose-limited | **Pending** |
| Email — verification & security communication | **Approved** | Allowed — purpose-limited | **Pending** |
| Storage — files, documents, media | **Approved** | Allowed | **Pending** |
| Marketing SMS | **Not approved** | **Forbidden** | N/A |
| Unnecessary / promotional notifications | **Not approved** | **Forbidden** | N/A |
| Marketing email campaigns | **Not approved** | **Forbidden** | N/A |
| Maps / location (all providers) | **Excluded V1** | **Forbidden** | N/A |
| Payment (IXOPAY) | Business approved (007) | Pending dossier | **Pending** |
| OCR / face | — | Evaluation pending | **Pending** |

---

## SMS provider — APPROVED purpose

**ONLY:** phone verification · OTP verification · account verification

**NOT:** marketing SMS · unnecessary notifications

---

## Email provider — APPROVED purpose

**ONLY:** email verification · account communication · security-related emails

---

## Storage provider — APPROVED purpose

- Application file storage
- User / provider documents
- Approved media storage

---

## Maps — NOT APPROVED FOR V1

See `MAPS_V1_EXCLUSION_RECORD.md`. **No V1 maps dependency.**

---

## Technical validation required

| Role | Status |
|------|--------|
| Integration Lead | **Pending** — vendor dossier · sandbox |
| Technical Architect | **Pending** — ports/adapters · sign-off |

**BLOCKER-003 NOT CLOSED.**
