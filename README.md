# KHADAMATI V1

Service-first marketplace for Lebanon — **localhost development** authorized under Gate A (GOV-GAIR-001 v2.0).

## Monorepo structure

```
apps/
  web/                 ONE React app — 4 logins (Customer, Craftsman, Store, Admin)
  mobile/              ONE Flutter app — Android + iOS (Customer, Craftsman & Store)
  api/                 Spring Boot 3.x API (Java 21)
packages/shared/       Design tokens + i18n (en/ar)
storage/               Local file uploads (gitignored)
docs/v1/               Governance, ADRs, scope
```

## Surfaces

| Platform | App | Login types |
|----------|-----|-------------|
| **Web** | `apps/web` | Customer · Craftsman · Store · Admin |
| **Mobile** | `apps/mobile` | Customer · Craftsman · Store (Admin — web only) |
| **API** | `apps/api` | Shared backend |

## Quick start

### Web

```powershell
cd apps\web
npm install
npm run dev
```

http://localhost:5173/login

### Mobile (one codebase → Android + iOS)

```powershell
cd apps\mobile
flutter pub get
flutter run
```

| Build | Command |
|-------|---------|
| Android APK | `flutter build apk` |
| iOS (on Mac) | `flutter build ios` |

Device picker: `flutter devices` then `flutter run -d <device-id>`

Example: `flutter run -d chrome` (quick test in browser)

### API

```powershell
cd apps\api
mvn spring-boot:run "-Dspring-boot.run.profiles=dev-inmemory"
```

http://localhost:8080/health

## Governance

Gate A localhost only — not production. See `docs/v1/FINAL_SCOPE_BASELINE.md`.
