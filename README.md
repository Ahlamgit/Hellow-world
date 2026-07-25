# KHADAMATI V1

Service-first marketplace for Lebanon — **localhost development** authorized under Gate A (GOV-GAIR-001 v2.0).

## Monorepo structure

```
apps/
  api/                 Spring Boot 3.x API (Java 21)
  admin-web/           React + TypeScript + MUI admin portal shell
  store-web/           React + TypeScript + MUI store dashboard shell
  customer-mobile/     Flutter customer app shell
  craftsman-mobile/    Flutter craftsman app shell
packages/
  shared/              Design tokens + i18n (en/ar)
storage/               Local file uploads (gitignored — BLOCKER-003)
docs/v1/               Governance, ADRs, scope
```

## Prerequisites

| Tool | Version |
|------|---------|
| Java | 21 |
| Maven | 3.8+ |
| Node.js | 22+ |
| Flutter | 3.x stable |
| Docker | For PostgreSQL + Redis (optional for API) |

## Quick start (localhost)

### 1. Environment

```bash
cp .env.example .env
```

### 2. Database & Redis

```bash
docker compose up -d
```

### 3. API

```bash
cd apps/api
mvn spring-boot:run
```

Health: http://localhost:8080/health

### 4. Web apps

```bash
cd apps/admin-web && npm install && npm run dev   # http://localhost:5173
cd apps/store-web && npm install && npm run dev   # http://localhost:5174
```

### 5. Mobile apps

```bash
cd apps/customer-mobile && flutter pub get && flutter run
cd apps/craftsman-mobile && flutter pub get && flutter run
```

## Localhost vendor substitutes (BLOCKER-003)

| Service | Dev approach |
|---------|----------------|
| Storage | `./storage/` folder |
| SMS/OTP | `DEV_MODE=true`, mock code `000000` |
| Email | Logged to console |
| Payment | Not in Sprint 0 (BLOCKER-007) |

## Sprint 0 scope

Foundation only — see `docs/v1/governance/SPRINT_0_FOUNDATION_CHARTER.md`.

**Included:** repo layout, CI, API skeleton, auth/RBAC structure, Flyway baseline, design tokens, app shells, i18n (AR/EN).

**Excluded:** payment, bookings, settlement, production deploy, business domain tables, maps in V1 release.

## Governance

- Gate A: localhost development authorized — **not** production launch
- Architecture: ADR-001 → ADR-032
- Scope: `docs/v1/FINAL_SCOPE_BASELINE.md`
