# BLOCKER-003 — Maps Localhost Testing Exception

| Field | Value |
|-------|-------|
| **Document ID** | EVD-003-MAPS-DEV-EXC-001 |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |
| **Approver** | Ahlam (Project Owner / Business Owner) |
| **Supersedes nothing** | `MAPS_V1_EXCLUSION_FINAL.md` remains authoritative for **V1 release** |

```text
V1 RELEASE = NO MAPS.
This exception is LOCALHOST DEVELOPMENT ONLY.
PO will order removal of map-dependent features before launch.
```

---

## 1. Two different rules

| Context | Maps allowed? |
|---------|----------------|
| **V1 public launch / production** | **NO** — forbidden |
| **localhost development / testing** | **YES** — temporary exception only |

---

## 2. PO decision (2026-07-25)

Ahlam (Project Owner / Business Owner) approves:

- Map-related code **may be experimented with on localhost** during development
- **No** Google Maps / Mapbox **production** contracts for V1
- **No** map features in V1 release scope
- **Before launch:** PO will **order removal** of all features that depend on maps
- Launch checklist must confirm **zero map dependencies** in production build

---

## 3. Allowed for localhost testing only

| Item | Allowed in dev? | Allowed at launch? |
|------|-----------------|------------------|
| Map UI prototypes | ☑ localhost | ✗ remove |
| Geolocation experiments | ☑ localhost | ✗ remove |
| Distance / radius tests | ☑ localhost | ✗ remove |
| Google Maps / Mapbox SDK | ☑ dev keys only | ✗ **forbidden** |
| Map-based booking flows | ☑ dev experiments | ✗ remove |

---

## 4. Mandatory pre-launch gate

Before production release, PO / TA must verify:

- [ ] All map screens removed or feature-flagged off
- [ ] No geolocation permissions in production app manifest
- [ ] No map vendor SDK in production bundle
- [ ] No distance/location APIs in production code paths
- [ ] Scope matches `MAPS_V1_EXCLUSION_FINAL.md`

**Document removal in launch checklist / Sprint 0 exit criteria.**

---

## 5. Relationship to frozen scope

| Document | Status |
|----------|--------|
| `MAPS_V1_EXCLUSION_FINAL.md` | **Unchanged** — V1 release exclusion |
| `FINAL_SCOPE_BASELINE.md` | **Unchanged** — no maps in V1 |
| This exception | **Dev-only** — does not expand V1 product scope |

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | PO localhost maps testing exception + pre-launch removal commitment |
