# AGENTS.md

## Cursor Cloud specific instructions

KHADAMATI V1 monorepo — Gate A **localhost development** authorized. Governance lives under `docs/v1/`.

### Repository layout

| Path | Purpose |
|------|---------|
| `apps/api` | Spring Boot 3.x API (Java 21, Maven, Flyway, PostgreSQL) |
| `apps/admin-web` | React + Vite + MUI admin shell (port 5173) |
| `apps/store-web` | React + Vite + MUI store shell (port 5174) |
| `apps/customer-mobile` | Flutter customer shell |
| `apps/craftsman-mobile` | Flutter craftsman shell |
| `packages/shared` | Design tokens + `en`/`ar` i18n JSON |
| `storage/` | Local uploads — gitignored (BLOCKER-003) |
| `docs/v1/` | ADRs, governance, Gate A records |

### Services (localhost)

1. **PostgreSQL + Redis:** `docker compose up -d` from repo root (requires Docker).
2. **API:** `cd apps/api && mvn spring-boot:run` — health at `/health`.
3. **Admin web:** `cd apps/admin-web && npm run dev` (5173).
4. **Store web:** `cd apps/store-web && npm run dev` (5174).

Copy `.env.example` to `.env` before starting the API. Tests use H2 in-memory and do **not** require Docker.

### Lint / test / build

| App | Command |
|-----|---------|
| API | `cd apps/api && mvn test` |
| Admin web | `cd apps/admin-web && npm run build` |
| Store web | `cd apps/store-web && npm run build` |
| Customer mobile | `cd apps/customer-mobile && flutter test` |
| Craftsman mobile | `cd apps/craftsman-mobile && flutter test` |

CI: `.github/workflows/ci.yml` runs all of the above on PR.

### Sprint 0 constraints (do not violate)

- **No** payment, booking completion, settlement, or business domain DB tables.
- **No** production deploy or production vendor keys.
- **No** hardcoded commission/subscription prices (ADR-013).
- Maps: dev experiments only — **forbidden in V1 public release**.
- Vendor integrations must use ports/adapters; localhost uses mocks per `docs/v1/governance/evidence/BLOCKER-003-vendors/VENDOR_LOCALHOST_TESTING_STRATEGY.md`.

### Flutter note

Flutter SDK is not installed in the base VM image. Install stable Flutter 3.x locally or rely on CI for `flutter analyze` / `flutter test`.

### Governance references

- Sprint 0 charter: `docs/v1/governance/SPRINT_0_FOUNDATION_CHARTER.md`
- Gate A authorization: `docs/v1/governance/GATE_A_IMPLEMENTATION_AUTHORIZATION_RECORD.md`
- Tech stack: `docs/v1/FINAL_SCOPE_BASELINE.md` §6
