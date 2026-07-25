# BLOCKER-004 — Development Environment Decision (Localhost)

| Field | Value |
|-------|-------|
| **Document ID** | EVD-004-DEV-ENV-001 |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |
| **Blocker** | BLOCKER-004 — Cloud |
| **Decision** | **Development / testing on localhost** |
| **Approver** | Ahlam (Project Owner / Business Owner) |
| **Production hosting** | **Still TBD** — TA + DevOps required |
| **Deployment** | **NOT AUTHORIZED** (no cloud deploy during Gate B) |

```text
Localhost is approved for LOCAL DEVELOPMENT AND TESTING ONLY.
This does NOT replace production cloud hosting decisions.
```

---

## 1. Approved environment matrix (PO/BO 2026-07-25)

| Environment | Host | Purpose | Approved |
|-------------|------|---------|----------|
| **Development** | `localhost` (e.g. `http://localhost:3000`, `http://localhost:4000`) | Local engineering, integration testing, sandbox trials | ☑ **Approved** |
| **Staging** | **TBD** (cloud) | Pre-production validation | ☐ Pending TA + DevOps |
| **Production** | **TBD** (cloud) | Live platform | ☐ Pending TA + DevOps |

---

## 2. Localhost testing scope (approved)

| Use case | Allowed on localhost |
|----------|----------------------|
| API development | ☑ |
| Admin portal dev | ☑ |
| Mobile app → local API | ☑ |
| IXOPAY sandbox webhook testing (via tunnel e.g. ngrok) | ☑ — design only until Gate A code |
| Production data | ✗ **Forbidden** |
| Public production traffic | ✗ **Forbidden** |

---

## 3. Security rules (localhost)

| Rule | Requirement |
|------|-------------|
| Secrets | `.env` local only — never commit |
| Sandbox credentials | IXOPAY sandbox keys only |
| TLS | Optional locally; required in staging/production |
| PII | Test data only on localhost |

---

## 4. Production cloud (unchanged — pending)

Production hosting, backup, DR, RPO/RTO remain per `CLOUD_READINESS_DECISION_RECORD.md` — **not decided**.

**BLOCKER-004 NOT CLOSED** until TA + DevOps approve full cloud readiness including production strategy.

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | PO/BO approves localhost for dev/testing |
