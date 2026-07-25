# BLOCKER-003 — Vendors Explained (IL + TA)

| Field | Value |
|-------|-------|
| **Document ID** | EVD-003-GUIDE-001 |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |
| **Status** | **CLOSED** (localhost testing track) |

---

## What is BLOCKER-003?

**Vendors** = third-party services KHADAMATI connects to.

| Vendor type | V1 use | Localhost testing |
|-------------|--------|-------------------|
| **SMS** | Phone OTP only | **PASS** — mock OTP / console |
| **Email** | Verification mail only | Log / Mailpit |
| **Storage** | Files & images | **Local folder** `./storage/` |
| **Maps** | **NOT in V1 launch** | Dev experiments only — remove before launch |

---

## What does IL + TA mean?

| Abbreviation | Full title | Job |
|--------------|------------|-----|
| **IL** | **Integration Lead** | Chooses vendors, runs sandbox tests, confirms APIs work |
| **TA** | **Technical Architect** | Confirms integrations fit architecture (ports/adapters, no tight coupling) |

On your project **Ahlam** signed as **both IL and TA** on 2026-07-25.

---

## Evidence files

| File | Purpose |
|------|---------|
| `VENDOR_LOCALHOST_TESTING_STRATEGY.md` | Local storage, SMS pass, email mock |
| `MAPS_LOCALHOST_TESTING_EXCEPTION.md` | Maps allowed on dev only — remove before launch |
| `MAPS_V1_EXCLUSION_FINAL.md` | **No maps in V1 release** |
| `BLOCKER_003_CLOSURE_RECORD.md` | Blocker closed |

---

## Production (before public launch)

You will still need real vendor contracts for SMS, email, and cloud storage — **not required to start coding on localhost**.
