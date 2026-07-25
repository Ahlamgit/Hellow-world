# BLOCKER-003 — Localhost Testing Vendor Strategy

| Field | Value |
|-------|-------|
| **Document ID** | EVD-003-LOCALHOST-001 |
| **Version** | 1.1 |
| **Date** | 2026-07-25 |
| **Scope** | **Development / localhost testing only** |
| **Production vendors** | **Deferred** — select before launch |
| **Approver** | Ahlam (PO/BO · IL · TA) |

```text
Production V1 rules unchanged. This document covers LOCALHOST TESTING ONLY.
```

---

## 1. Approved localhost substitutes (PO/BO 2026-07-25)

| Category | V1 production | **Localhost testing — APPROVED** |
|----------|---------------|--------------------------------|
| **Storage** | Cloud storage vendor (TBD) | ☑ **Local disk** — e.g. `./storage/` or `uploads/` on dev machine |
| **SMS** | SMS provider (TBD) | ☑ **PASS / mock** — OTP printed to console, fixed test code in `.env`, or skip verification in `DEV_MODE` |
| **Email** | Email provider (TBD) | ☑ Console log / Mailpit / file log |
| **Maps** | **NOT in V1 release** | ☑ **Dev-only exception** — see `MAPS_LOCALHOST_TESTING_EXCEPTION.md` |

---

## 2. Storage — local (testing)

| Rule | Detail |
|------|--------|
| Path | Project-local folder (not committed to git) |
| Git | Add `storage/` to `.gitignore` |
| Production | Cloud object storage vendor required before launch |
| Secrets | No production credentials on localhost |

---

## 3. SMS — pass for testing

| Rule | Detail |
|------|--------|
| Dev mode | SMS verification **bypassed or mocked** |
| Test OTP | e.g. `000000` or logged to terminal |
| Production | Real SMS vendor + sandbox required before launch |
| Security | `DEV_MODE` must be **disabled** in production builds |

---

## 4. Maps — testing exception (NOT V1 release)

| Rule | Detail |
|------|--------|
| V1 release | **No maps** — `MAPS_V1_EXCLUSION_FINAL.md` unchanged |
| Localhost dev | PO allows **temporary** map experimentation only |
| PO commitment | **Remove all map-dependent features before public launch** |
| Providers | No paid map vendor contract for V1; dev stubs only |

Full terms: `MAPS_LOCALHOST_TESTING_EXCEPTION.md`

---

## 5. Production vendor selection (still deferred)

| Category | Named vendor | Required before launch |
|----------|--------------|------------------------|
| SMS | TBD | ☑ |
| Email | TBD | ☑ |
| Storage | TBD | ☑ |
| Maps | **None for V1** | N/A |

---

## 6. Approvals

| Approver | Role | Date | Decision |
|----------|------|------|----------|
| **Ahlam** | Project Owner / Business Owner | 2026-07-25 | ☑ Local storage · SMS pass · maps dev exception |
| **Ahlam** | Integration Lead | 2026-07-25 | ☑ |
| **Ahlam** | Technical Architect | 2026-07-25 | ☑ |

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | Initial localhost vendor substitutes |
| 1.1 | 2026-07-25 | PO approves local storage, SMS pass, maps dev-only exception |
