# BLOCKER-003 — Vendor Evidence

| Field | Value |
|-------|-------|
| **Blocker ID** | BLOCKER-003 |
| **Folder** | `evidence/BLOCKER-003-vendors/` |
| **Tracker** | `BLOCKER_CLOSURE_EXECUTION_TRACKER.md` (GOV-BLOCKER-TRACKER-001) |

## Purpose

Document vendor evaluation, selection decisions, contract references, and sandbox readiness for all external integrations behind ports/adapters (ADR-025).

## Owner

**Technical Lead / Integration Lead** (accountable)

## Required evidence

| Integration | Subfolder |
|-------------|-----------|
| Payment | `payment/` |
| SMS | `sms/` |
| Email | `email/` |
| Maps | `maps/` |
| OCR / Face | `ocr-face/` |
| Storage | `storage/` |

Plus: `APPROVAL_RECORD.md`, `EVIDENCE_CHECKLIST.md`

**Closure artifact:** `VENDOR_READINESS_DOSSIER_v1.0`

**Note:** Governance evidence only — ports/adapters architecture unchanged.

## Approval authority

| Role | Authority |
|------|-----------|
| Technical Lead / Integration Lead | Dossier completeness, technical validation |
| Technical Architect | Architecture compliance (ports/adapters) |
| Program Sponsor | Contract / commercial escalation if required |

## Current status

| Field | Value |
|-------|-------|
| **Status** | **Open** |
| **Closed** | **No** |

## Closure criteria

- [ ] Per-integration evidence in subfolders
- [ ] `EVIDENCE_CHECKLIST.md` complete
- [ ] `APPROVAL_RECORD.md` — **Approved**
- [ ] `VENDOR_READINESS_DOSSIER_v1.0` filed
- [ ] Trackers updated
