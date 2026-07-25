# KHADAMATI — Sprint 0 Foundation Charter

| Field | Value |
|-------|-------|
| **Document ID** | GOV-S0FC-001 |
| **Version** | 1.0 |
| **Sprint** | Sprint 0 — Foundation |
| **Status** | **Planned — gated execution** |
| **Gate (program)** | B — NOT READY — CODING BLOCKED |
| **Prepared by** | Program Governance Manager |
| **Date** | 2026-07-25 |

---

## Purpose

Define the **authorized scope** for Sprint 0 — Foundation Implementation: what may be built, what is explicitly excluded, gate dependencies per workstream, and exit criteria.

Sprint 0 establishes **platform foundations only** — not product features, payments, bookings, or production release.

---

## Sprint 0 Scope Boundary

### Allowed

| # | Workstream | Description |
|---|------------|-------------|
| A1 | Repository setup | Monorepo structure, branch policy, README, env templates |
| A2 | CI/CD setup | Build, lint, test pipelines; **no production deployment** |
| A3 | Backend skeleton | NestJS (or approved stack) module layout, health checks, config |
| A4 | Authentication foundation | JWT/session structure, password hashing hooks — **no production IdP** |
| A5 | RBAC foundation | Role model, guards, permission interfaces — **no business permissions matrix** |
| A6 | Database migration framework | Migration tooling, baseline migration, **no business domain tables** |
| A7 | Design system implementation | Tokens, primitives per approved design system — **gated** |
| A8 | App shells | Customer, Craftsman, Store, Admin navigation shells — **gated** |
| A9 | Localization foundation | i18n structure, AR RTL / EN LTR, default LB/+961/USD config |
| A10 | Logging / audit foundation | Structured logging, audit event interfaces — **no compliance retention values** |

### Not allowed

| # | Exclusion | Reason |
|---|-----------|--------|
| N1 | Payment implementation | BLOCKER-003, BLOCKER-005, BLOCKER-007 |
| N2 | Booking completion flow | Product feature — post Sprint 0 |
| N3 | Settlement logic | BLOCKER-005 finance values |
| N4 | Production deployment | Gate B / Sprint 0 boundary |
| N5 | Feature expansion | Frozen scope |

---

## Gate and Blocker Dependencies

Sprint 0 workstreams **must not** violate program gates or ADRs.

| Workstream | Minimum gate / blocker | Notes |
|------------|------------------------|-------|
| A1 Repository setup | **Sprint 0 kickoff** | May start when Sprint 0 formally authorized |
| A2 CI/CD (non-prod) | **Sprint 0 kickoff** | Dev/staging pipeline only; no prod deploy |
| A3 Backend skeleton | **Sprint 0 kickoff** | Ports/adapters layout per approved architecture |
| A4 Auth foundation | **Sprint 0 kickoff** | No PII retention assumptions — BLOCKER-006 for policies |
| A5 RBAC foundation | **Sprint 0 kickoff** | Roles: Customer, Craftsman, Store, Admin (governance-level) |
| A6 Migration framework | **Sprint 0 kickoff** | Tooling + empty/schema-bootstrap only until BLOCKER-006 closed for domain schema |
| A7 Design system | **BLOCKER-001 Closed** | ADR-023 — UI blocked until design assets approved |
| A8 App shells | **BLOCKER-001 Closed** | ADR-023 — no Flutter/React screens beyond shells until design sign-off |
| A9 Localization foundation | **Sprint 0 kickoff** | Structure only; copy from approved specs when available |
| A10 Logging / audit foundation | **Sprint 0 kickoff** | Retention durations remain *Pending* until BLOCKER-006 closed |

### Program gate entry for Sprint 0 execution

| Condition | Requirement |
|-----------|-------------|
| **Full Sprint 0 (all workstreams)** | Gate A — **READY FOR IMPLEMENTATION** + all blockers **Closed** |
| **Partial Sprint 0 (A1–A6, A9–A10 only)** | Explicit program waiver **or** Gate A with BLOCKER-001/006 still open — **not recommended** |
| **Current state (2026-07-25)** | Gate B — **Sprint 0 coding not authorized** until Gate A or documented waiver |

**Ready for approval (not closed):** BLOCKER-001, 002, 005, 006 — signatures pending.

---

## Workstream Detail

### A1 — Repository setup

- Monorepo: `apps/` (api, customer-mobile, craftsman-mobile, store-web, admin-web) + `packages/` (shared)
- Branch strategy, PR template, `.env.example`
- No business logic

### A2 — CI/CD setup

- Lint, typecheck, unit test jobs on PR
- Build artifacts for apps/api
- **Excluded:** Production deploy, secrets for prod, BLOCKER-004 cloud provisioning

### A3 — Backend skeleton

- App bootstrap, config module, health `/health`
- Module boundaries per clean architecture
- **Excluded:** Booking, payment, settlement modules

### A4 — Authentication foundation

- Register/login/logout **structure** (interfaces, DTOs, guards)
- Password hashing (bcrypt), JWT refresh pattern
- **Excluded:** Email/SMS vendors (BLOCKER-003), MFA prod config

### A5 — RBAC foundation

- Role enum: `CUSTOMER`, `CRAFTSMAN`, `STORE`, `ADMIN`, `FINANCE_ADMIN`, `SUPER_ADMIN`
- `@Roles()` guard, policy interface
- **Excluded:** Full permission matrix implementation

### A6 — Database migration framework

- Prisma (or approved ORM) + `migrate` scripts in CI
- Initial migration: extensions only or empty baseline
- **Excluded:** Users, bookings, ledger, KYC tables until BLOCKER-006 + schema ADRs

### A7 — Design system implementation *(gated)*

- Implement `DESIGN_SYSTEM_TOKENS.md` after BLOCKER-001 closed
- Shared package: colors, typography, spacing, core components
- **Excluded:** Feature screens

### A8 — App shells *(gated)*

- Flutter: Customer + Craftsman app shells (routing, theme provider)
- React: Store dashboard + Admin portal shells
- Placeholder routes only
- **Excluded:** Booking, payment, catalog flows

### A9 — Localization foundation

- `ar` (RTL) + `en` (LTR)
- Default locale: Lebanon, USD display, +961 phone formatting hooks
- **Excluded:** Hardcoded country-specific legal text

### A10 — Logging / audit foundation

- Structured logging (request ID, correlation)
- Audit event publisher interface + admin action hook points
- **Excluded:** Retention enforcement until BLOCKER-006 values approved

---

## Sprint 0 Execution Order

```
Wave 1 (infrastructure — after Sprint 0 authorized):
  A1 Repository → A2 CI/CD → A3 Backend skeleton

Wave 2 (security foundations — parallel):
  A4 Auth → A5 RBAC → A6 Migration framework → A10 Logging/audit

Wave 3 (client foundations — parallel):
  A9 Localization

Wave 4 (UI — after BLOCKER-001 Closed only):
  A7 Design system → A8 App shells
```

---

## Sprint 0 Exit Criteria

| Criterion | Evidence |
|-----------|----------|
| Repo builds in CI | Green pipeline on `main` |
| API health check passes | `/health` returns 200 |
| Auth + RBAC smoke tests | Unit/integration tests pass |
| Migration framework runs | `migrate` succeeds on empty DB |
| Localization toggles AR/EN | Demo screen or test |
| Audit log emits on admin hook | Test event captured |
| Design system + shells | Only if BLOCKER-001 closed — Storybook or shell demo |
| **No** payment/booking/settlement code | Code review checklist |
| **No** production deployment | Infra review |

---

## Explicit Out-of-Scope Reminder

The following remain **prohibited** in Sprint 0 regardless of engineering readiness:

- Payment.js / IXOPAY integration (BLOCKER-007)
- Ledger posting, commission, withdrawal, settlement
- Booking state machine (beyond placeholder types if any)
- KYC document upload/processing (BLOCKER-003 OCR vendor)
- Account deletion workflow implementation (BLOCKER-006)
- Ecommerce, cart, inventory (frozen scope exclusion)

---

## Governance Cross-References

| Document | Relevance |
|----------|-----------|
| `FINAL_IMPLEMENTATION_GATE_REPORT.md` | Gate status |
| `IMPLEMENTATION_READINESS_EXECUTION_PLAN.md` | Phase model |
| `BLOCKER_CLOSURE_EXECUTION_PLAN.md` | Blocker order |
| ADR-023 | UI blocked until design |
| ADR-022 | Deletion — not Sprint 0 |
| BLOCKER-001 | Design before A7/A8 |
| BLOCKER-006 | Domain schema + retention |

---

## Authorization Record

| Field | Value |
|-------|-------|
| Sprint 0 coding authorized | **No** (Gate B — pending Gate A or waiver) |
| Sprint 0 plan approved | **This charter v1.0** |
| Authorized by | — |
| Date | — |

---

## Document Control

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | Initial Sprint 0 foundation charter |
