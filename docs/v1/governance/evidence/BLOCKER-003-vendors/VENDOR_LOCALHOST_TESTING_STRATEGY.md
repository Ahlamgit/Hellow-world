# BLOCKER-003 — Localhost Testing Vendor Strategy

| Field | Value |
|-------|-------|
| **Document ID** | EVD-003-LOCALHOST-001 |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |
| **Scope** | **Development / localhost testing only** |
| **Production vendors** | **Deferred** — select before launch |

---

## 1. What is BLOCKER-003? (plain language)

**Vendors** = external services KHADAMATI uses (SMS, email, file storage, etc.).

| Role | Meaning |
|------|---------|
| **IL — Integration Lead** | Person who chooses integrations and runs sandbox tests |
| **TA — Technical Architect** | Person who confirms integrations fit the architecture |

On a small team, **one person (e.g. Ahlam) may hold both roles** for testing.

---

## 2. Approved categories (V1)

| Category | Production purpose | **Localhost testing substitute** |
|----------|-------------------|----------------------------------|
| **SMS** | Phone OTP only | Console log / mock OTP in `.env` dev mode |
| **Email** | Verification + security mail only | Log to console / Mailpit / file log |
| **Storage** | Media & documents | Local folder `storage/` or temp disk |
| **Maps** | **NOT APPROVED** | **Excluded** — see `MAPS_V1_EXCLUSION_FINAL.md` |

---

## 3. Production vendor selection (deferred)

| Category | Named vendor | Contract | Sandbox |
|----------|--------------|----------|---------|
| SMS | TBD pre-launch | TBD | TBD |
| Email | TBD pre-launch | TBD | TBD |
| Storage | TBD pre-launch | TBD | TBD |

**Allowed for localhost testing without named vendors.**

---

## 4. Approvals (testing track)

| Approver | Role | Date | Decision |
|----------|------|------|----------|
| **Ahlam** | Integration Lead | 2026-07-25 | ☑ Approved — localhost testing strategy |
| **Ahlam** | Technical Architect | 2026-07-25 | ☑ Approved — ports/adapters; no maps V1 |

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | Localhost vendor substitutes for dev testing |
