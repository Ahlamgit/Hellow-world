# BLOCKER-003 — Maps V1 Exclusion Record

| Field | Value |
|-------|-------|
| **Document ID** | EVD-003-MAPS-EXCL-001 |
| **Version** | 1.1 |
| **Date** | 2026-07-25 |
| **Decision** | **NOT APPROVED FOR V1** |
| **Approver** | Project Owner / Business Owner |

---

## Governance decision

Maps and location services are **intentionally excluded from KHADAMATI V1**.

---

## V1 position

| Statement | Confirmation |
|-----------|--------------|
| No maps implementation in V1 | **Yes** |
| No V1 dependency on maps/location providers | **Yes** |
| Reserved for future product evolution only | **Yes** |
| Separate governance approval required for future inclusion | **Yes** |

---

## Forbidden in V1

- Maps integration
- GPS tracking
- Location tracking
- Location-based matching

---

## Architecture note

ADR-001 → ADR-032 unchanged. Ports/adapters pattern may exist at architecture level; **no V1 maps implementation or integration dependency**.

**BLOCKER-003 checklist:** Maps sandbox item = **N/A — V1 excluded**.
