# BLOCKER-003 — Maps V1 Exclusion (Final)

| Field | Value |
|-------|-------|
| **Document ID** | EVD-003-MAPS-EXCL-FINAL-001 |
| **Version** | 1.1 |
| **Date** | 2026-07-25 |
| **Supersedes** | `MAPS_V1_EXCLUSION_RECORD.md` (v1.2) — canonical final statement |
| **Decision** | **NOT APPROVED FOR V1** |
| **Approver** | Project Owner / Business Owner |

```text
Maps and location services are EXCLUDED from KHADAMATI V1.
Future versions may consider maps/location ONLY under separate governance approval.
```

---

## 1. Final decision

Maps and location capabilities are **intentionally excluded** from KHADAMATI V1 **release** scope.

**Localhost-only dev exception:** `MAPS_LOCALHOST_TESTING_EXCEPTION.md` — PO will order removal of map features before launch.

---

## 2. Forbidden in V1 (do NOT implement)

| Category | Forbidden items |
|----------|-----------------|
| Map providers | Google Maps · Mapbox · any third-party map SDK |
| Location services | Geolocation APIs · GPS tracking · device location permission flows for maps |
| Map UI | Map screens · map pins · map-based provider discovery |
| Distance | Distance calculation · radius search · geo-fencing |
| Location APIs | Location-based matching · coordinates in booking flows (beyond non-map address text if separately approved) |

---

## 3. Approved vendor categories (V1)

| Provider category | V1 purpose | Status |
|-------------------|------------|--------|
| **SMS** | Phone verification · OTP/security only | Approved (category) |
| **Email** | Email verification · security notifications only | Approved (category) |
| **Storage** | Media and document storage | Approved (category) |
| **Maps / Location** | — | **NOT APPROVED** |

---

## 4. Future consideration

Maps and location **may be considered in future product versions only**, subject to:

- Separate scope governance approval
- Updated ADR if architecture impact
- New blocker or change-control evidence
- Legal/compliance review where applicable

**V1 rule:** Document as future possibility only — **no V1 design, vendor selection, or implementation.**

---

## 5. Evidence linkage

| Document | Purpose |
|----------|---------|
| `MAPS_V1_EXCLUSION_RECORD.md` | Prior exclusion record (v1.2) |
| `VENDOR_APPROVAL_MATRIX.md` | Vendor category matrix |
| `FINAL_SCOPE_BASELINE.md` | Scope freeze (BLOCKER-002) |

**BLOCKER-003 CLOSED** — see `BLOCKER_003_CLOSURE_RECORD.md`.

**Localhost testing updates (v1.1):** local storage · SMS pass · maps dev exception — `VENDOR_LOCALHOST_TESTING_STRATEGY.md` · `MAPS_LOCALHOST_TESTING_EXCEPTION.md`

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | Final maps V1 exclusion statement — Gate B evidence finalization |
| 1.1 | 2026-07-25 | Addendum: localhost dev exception does NOT change V1 release exclusion |
