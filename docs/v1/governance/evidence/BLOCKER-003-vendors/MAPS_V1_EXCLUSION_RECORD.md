# BLOCKER-003 — Maps V1 Exclusion Record

| Field | Value |
|-------|-------|
| **Document ID** | EVD-003-MAPS-EXCL-001 |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |
| **Decision** | **NOT APPROVED FOR V1** |
| **Approver** | Project Owner / Business Owner |
| **Scope impact** | V1 exclusion — no architecture change |

---

## Decision

Maps and location services are **excluded from KHADAMATI V1**.

---

## Forbidden in V1

- Maps integration
- GPS tracking
- Location tracking
- Location-based matching

---

## Future use

Maps/location may be considered **only** after separate governance approval and scope change process. **Not** part of current V1 frozen scope.

---

## Evidence checklist impact

BLOCKER-003 maps sandbox item: **N/A — V1 excluded** (see `EVIDENCE_CHECKLIST.md`).

**Architecture:** ADR-001 → ADR-032 unchanged; ports may exist but **no V1 maps implementation**.
