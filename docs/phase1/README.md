# Phase 1 — Production Stabilization

KHADAMATI Phase 1 is split into four waves. This folder holds planning and rollback documentation for each wave.

| Wave | Focus | Status |
|------|-------|--------|
| **1A** | Data integrity | In progress |
| **1B** | Security hardening | Pending |
| **1C** | API consolidation | Pending |
| **1D** | Global platform model | Pending |

## Phase 1A documents

- [Implementation plan](./PHASE1A_IMPLEMENTATION_PLAN.md)
- [Database migration plan](./PHASE1A_DATABASE_MIGRATION_PLAN.md)
- [Risk assessment](./PHASE1A_RISK_ASSESSMENT.md)
- [Test plan](./PHASE1A_TEST_PLAN.md)
- [Rollback plan](./PHASE1A_ROLLBACK_PLAN.md)

## Rules

- No direct production assumptions — all changes are validated in development and CI first.
- Each wave must pass backend unit tests, integration tests, and migration validation before completion.
- Do not rotate production secrets during Phase 1B until `docs/security/SECRET_ROTATION_PLAN.md` is approved.
