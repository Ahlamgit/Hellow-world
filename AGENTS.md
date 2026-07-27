# AGENTS.md

## Cursor Cloud specific instructions

KHADAMATI V1 monorepo — Gate A **localhost development** authorized.

### Layout

| Path | Purpose |
|------|---------|
| `apps/web` | Single React app — **4 logins**: Customer, Craftsman, Store, Admin (port 5173) |
| `apps/mobile` | Single Flutter app — **Android + iOS** — Customer, Craftsman & Store login |
| `apps/api` | Spring Boot 3.x API (Java 21, Maven, Flyway) |
| `packages/shared` | Design tokens + en/ar i18n JSON |
| `storage/` | Local uploads (gitignored) |

Admin is **web-only** (ADR-006). Mobile: Customer, Craftsman, Store.

### Services (localhost)

1. **PostgreSQL + Redis:** `docker compose up -d` (optional; API has `dev-inmemory` profile)
2. **API:** `cd apps/api && mvn spring-boot:run`
3. **Web:** `cd apps/web && npm run dev` → http://localhost:5173 (proxies `/api` to port 8080)

Web auth uses `POST /api/v1/auth/login` (JWT). Dev seed users are documented in README. Admin MFA: `/admin/login` (OTP `000000` in dev).

### Lint / test / build

| App | Command |
|-----|---------|
| API | `cd apps/api && mvn test` |
| Web | `cd apps/web && npm run build` |
| Mobile | `cd apps/mobile && flutter test` |

Mobile builds: `flutter build apk` (Android), `flutter build ios` (macOS + Xcode only).

CI: `.github/workflows/ci.yml`

### Sprint 0 constraints

No payment, booking, settlement, or business DB tables. No production deploy. Vendor mocks per BLOCKER-003.

### Flutter on Cloud VM

Flutter is not pre-installed in the base VM; CI runs `flutter analyze` / `flutter test` via `subosito/flutter-action`.
