# BLOCKER-003 — Vendor Dossier (Preparation)

| Field | Value |
|-------|-------|
| **Document ID** | EVD-003-DOSSIER-001 |
| **Version** | 1.0 (preparation) |
| **Date** | 2026-07-25 |
| **Blocker** | BLOCKER-003 — Vendors |
| **Status** | **PREPARATION** — not closure dossier |
| **Closure artifact target** | `VENDOR_READINESS_DOSSIER_v1.0` |

---

## 1. Approved vendor categories (business)

| Category | Business decision | See |
|----------|-----------------|-----|
| SMS | **Approved** — phone/OTP only | `VENDOR_APPROVAL_MATRIX.md` |
| Email | **Approved** — verification/security only | Same |
| Storage | **Approved** | Same |
| Maps | **Excluded V1** | `MAPS_V1_EXCLUSION_RECORD.md` |

---

## 2. Per-integration dossier slots

| Integration | Vendor selected | Contract ref | Sandbox ready | Security review | TA validated |
|-------------|-----------------|--------------|---------------|-----------------|--------------|
| SMS | **Pending** | ☐ | ☐ | ☐ | ☐ |
| Email | **Pending** | ☐ | ☐ | ☐ | ☐ |
| Storage | **Pending** | ☐ | ☐ | ☐ | ☐ |
| Payment (IXOPAY) | **Pending** | ☐ | ☐ | ☐ | ☐ |
| OCR / face | **Pending** | ☐ | ☐ | ☐ | ☐ |

---

## 3. Security review (preparation)

| Control area | SMS | Email | Storage | Payment |
|--------------|-----|-------|---------|---------|
| API credential management | Pending | Pending | Pending | Pending |
| Data in transit encryption | Pending | Pending | Pending | Pending |
| Access control / IAM | Pending | Pending | Pending | Pending |
| Purpose limitation (no marketing abuse) | Required | Required | N/A | N/A |
| Ports/adapters isolation (ADR-025) | Pending TA | Pending TA | Pending TA | Pending TA |

**Security review record:** `VENDOR_SECURITY_REVIEW_RECORD.md`

---

## 4. Required approvers

| Role | Responsibility | Status |
|------|----------------|--------|
| **Integration Lead** | Complete dossier · sandbox evidence | **Pending** |
| **Technical Architect** | Architecture compliance · sign-off | **Pending** |

**BLOCKER-003 NOT CLOSED.**
