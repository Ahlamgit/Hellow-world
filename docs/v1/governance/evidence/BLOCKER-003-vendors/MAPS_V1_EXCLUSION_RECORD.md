# BLOCKER-003 — Maps V1 Exclusion Record

| Field | Value |
|-------|-------|
| **Document ID** | EVD-003-MAPS-EXCL-001 |
| **Version** | 1.2 |
| **Date** | 2026-07-25 |
| **Decision** | **NOT APPROVED FOR V1** |
| **Approver** | Project Owner / Business Owner |

---

## Decision

Maps and location services are **intentionally excluded from KHADAMATI V1**.

Architecture may remain **future-ready**; **V1 implementation must contain NO maps dependency.**

---

## DO NOT implement in V1

| Forbidden |
|-----------|
| Google Maps |
| Mapbox |
| Location APIs |
| Distance calculation |
| Map screens |
| Geolocation dependency |
| GPS tracking |
| Location-based matching |

---

## V1 rule

| Statement | Confirmation |
|-----------|--------------|
| No maps integration | **Yes** |
| No geolocation dependency | **Yes** |
| Reserved for future product evolution | **Yes** |
| Separate governance approval required for future inclusion | **Yes** |

**BLOCKER-003 checklist:** Maps items = **N/A — V1 excluded**.
