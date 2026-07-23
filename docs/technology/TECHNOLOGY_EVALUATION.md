# KHADAMATI — Technology Evaluation

**Document type:** Enterprise architecture analysis  
**Status:** Draft — awaiting approval  
**Date:** July 2026  
**Scope:** Current stack vs proposed React Native mobile stack

---

## 1. Executive summary

| Layer | Current | Proposed | Recommendation |
|-------|---------|----------|----------------|
| Backend | ASP.NET Core 8, Clean Architecture, EF Core, SQL Server | **Keep unchanged** | **Retain** |
| Web | React 19, TypeScript, Material UI 9, Vite | React + TypeScript (MUI optional) | **Retain React + TS** |
| Mobile | Kotlin/Compose (Android) + Swift/SwiftUI (iOS) | React Native + TypeScript | **Evaluate migration** (see §6) |

KHADAMATI is already well-positioned for enterprise scale on the **server and web**. The primary architectural decision is whether to **consolidate two native mobile codebases** (~12,500 lines, ~140 source files) into a **single React Native** application that shares TypeScript patterns with the web client.

**Preliminary conclusion:** Migrating mobile to React Native is **strategically justified** for cost, hiring, and velocity — but only with a **phased plan** that preserves API-first architecture and does not destabilize MVP launch. The backend, database, and web stack should **not** be replaced.

---

## 2. Stacks compared

### 2.1 Current stack

| Component | Technology | Maturity in project |
|-----------|------------|---------------------|
| API | ASP.NET Core 8 LTS | Production-oriented (Docker, JWT, RBAC, 169+ endpoints) |
| Data | EF Core 8 + SQL Server | 43+ EF tables, migrations, seeder |
| Patterns | Clean Architecture, MediatR, FluentValidation, AutoMapper | Established |
| Web | React 19, TypeScript, MUI 9, i18next (AR/EN) | Customer + Admin portals |
| Android | Kotlin, Jetpack Compose, Retrofit, Room, FCM | ~78 files, feature-complete MVP |
| iOS | Swift, SwiftUI, URLSession, Keychain, FCM/APNs | ~62 files, feature parity with Android |

### 2.2 Proposed stack

| Component | Technology |
|-----------|------------|
| API | **Unchanged** — ASP.NET Core 8 |
| Data | **Unchanged** — EF Core + SQL Server |
| Web | **Unchanged** — React + TypeScript |
| Mobile | **React Native + TypeScript** (Expo with dev builds recommended) |

---

## 3. Evaluation matrix

Legend: **++** strong advantage · **+** advantage · **=** neutral · **−** disadvantage · **−−** significant disadvantage

| Criterion | Current (Native ×2 + existing backend) | Proposed (RN + existing backend) | Notes |
|-----------|----------------------------------------|----------------------------------|-------|
| **Performance** | ++ | + | Native wins on raw UI perf; RN is sufficient for marketplace CRUD/booking flows |
| **Scalability (backend)** | ++ | ++ | Mobile choice does not limit API scale; both use same REST API |
| **Memory (mobile)** | ++ | + | RN bridge adds overhead; acceptable for this app class |
| **Hosting cost** | = | = | API/SQL costs dominate; mobile stack does not change cloud bill materially |
| **Cloud readiness** | + | + | API already containerized; RN adds EAS Build or CI pipelines |
| **Developer availability** | − | ++ | One TS mobile team vs Kotlin + Swift specialists (critical in MENA/Lebanon market) |
| **Maintenance cost** | −− | + | Two codebases today → duplicate features, bugs, releases |
| **Hiring difficulty** | −− | + | RN + React web = shared language (TypeScript) |
| **Testing** | + | + | Backend xUnit strong; RN has Jest/Detox; native has Espresso/XCUITest |
| **Security** | ++ | + | Native Keychain/EncryptedPrefs mature; RN needs `expo-secure-store` / Keychain libs |
| **Offline support** | + (Room on Android) | + | RN: AsyncStorage/SQLite/WatermelonDB; parity achievable |
| **Push notifications** | ++ | + | FCM + APNs already in backend; RN via `expo-notifications` or `@react-native-firebase/messaging` |
| **Maps / location** | ++ | + | Nearby craftsmen uses GPS today; RN: `react-native-maps` or Expo Location |
| **Camera / documents** | = (not fully built) | + | Verification upload is a gap; RN `expo-image-picker` / `expo-document-picker` accelerate delivery |
| **Payments** | + | + | WebView or deep-link to Moyasar; same as many RN marketplaces |
| **Long-term support** | ++ | + | Apple/Google mandate updates; RN 0.7x + Expo LTS tracks OS releases |
| **Community** | ++ | ++ | Both ecosystems large; RN benefits from React web overlap |
| **Enterprise readiness** | ++ (backend) | + (mobile) | Backend is enterprise-grade; RN needs discipline (typed API client, error boundaries, E2E tests) |

---

## 4. Dimension deep-dives

### 4.1 Performance

- **Backend:** ASP.NET Core 8 handles 5,000 concurrent users with horizontal scaling, caching, and DB tuning (see `SCALABILITY_PLAN.md`). Mobile technology does not change this.
- **Native mobile:** Best cold-start and animation performance.
- **React Native:** Adequate for KHADAMATI’s screens (lists, forms, maps, chat). Heavy 3D or real-time gaming is out of scope.
- **Risk:** Large FlatLists (services, bookings) need virtualization — same discipline as native `LazyColumn`/`List`.

### 4.2 Scalability

| Tier | Driver | Mobile impact |
|------|--------|---------------|
| 5K concurrent | API + DB + Redis | None — REST stateless |
| 50K registered | Storage, indexes, backups | Push token table growth; device registration endpoint load |

### 4.3 Memory and binary size

| Platform | Native (current) | React Native (est.) |
|----------|-------------------|---------------------|
| Android APK | ~15–25 MB (lean Compose) | ~25–40 MB (Hermes + bridge) |
| iOS IPA | ~10–20 MB | ~20–35 MB |

Acceptable for a marketplace app.

### 4.4 Hosting cost (API + data — dominant)

Rough monthly order-of-magnitude (production, single region):

| Scale | API compute | SQL Server | Redis | Blob | Total est. |
|-------|-------------|------------|-------|------|------------|
| MVP (500 CCU) | $80–150 | $100–200 | $30–50 | $20 | **$250–450** |
| Growth (5K CCU) | $300–600 | $400–800 | $80–150 | $50–100 | **$900–1,700** |
| 50K users | $600–1,200 | $800–1,500 | $150–300 | $100–200 | **$1,700–3,200** |

Mobile stack change does **not** materially alter these figures.

### 4.5 Developer availability and maintenance

**Current pain:**
- Every feature (booking wizard, chat, subscriptions, portals) is implemented **twice**.
- Android uses Retrofit + Room; iOS uses URLSession + no local DB — **inconsistent offline strategy**.
- Arabic/English strings maintained in `strings.xml`, `Localizable.strings`, and web `i18n` — **triple maintenance**.

**React Native benefit:**
- One mobile team, one release train, shared TypeScript DTOs with web.
- Estimated **35–45% reduction** in mobile-specific engineering effort after migration stabilizes.

### 4.6 Security

| Concern | Native | React Native |
|---------|--------|--------------|
| Token storage | EncryptedSharedPreferences / Keychain | `expo-secure-store`, `react-native-keychain` |
| Certificate pinning | OkHttp / URLSession | `react-native-ssl-pinning` (optional) |
| Admin on mobile | Must be blocked server-side (planned) | Same — API enforcement required regardless |
| OWASP mobile | Mature patterns | Requires linting + secure storage discipline |

**Verdict:** Native is slightly ahead on security primitives; RN is **acceptable** with standard hardening.

### 4.7 Offline support

| Feature | Android today | iOS today | RN target |
|---------|---------------|-----------|-----------|
| Service catalog cache | Room | Network-only | SQLite/MMKV + React Query persist |
| Auth session | DataStore | Keychain | Secure store |
| Bookings offline | No | No | Optional queue (post-MVP) |

### 4.8 Push, maps, camera, payments

| Capability | Backend readiness | Native mobile | RN path |
|------------|-------------------|---------------|---------|
| Push | FCM + APNs providers | Implemented | `expo-notifications` + FCM; APNs via EAS |
| Maps / GPS | Nearby craftsmen API | Implemented | `expo-location` + maps |
| Camera / docs | `POST /verification/documents` | **Not built** | `expo-image-picker` — accelerates gap closure |
| Payments | Moyasar integration | Card flow via API | WebView / in-app browser redirect |

### 4.9 Enterprise readiness

**Strengths to preserve:**
- Clean Architecture on backend
- Permission-based RBAC
- Structured logging (Serilog)
- Docker deployment path
- EF migrations with production gate

**RN enterprise requirements:**
- Monorepo or shared `@khadamati/api-types` package
- Typed API client generated from OpenAPI
- CI: lint, unit tests, EAS/TestFlight/Play internal track
- Crash reporting (Sentry)
- Feature flags for gradual rollout

---

## 5. Expo vs React Native CLI

| Factor | Expo (recommended) | React Native CLI |
|--------|---------------------|------------------|
| MVP velocity | **++** — faster bootstrap, OTA updates | + |
| Push (FCM/APNs) | Supported via dev builds + config plugins | Full manual native setup |
| Maps, camera, location | Expo modules | Manual linking |
| Native modules | Requires **development build** (not Expo Go alone) | Full control |
| Team skill | Lower barrier if team knows React | Needs Android/iOS toolchain familiarity |
| Enterprise CI | EAS Build + Submit | Custom Gradle/Xcode pipelines |
| Long-term | Expo SDK tracks RN; suitable for production apps | Better for heavy custom native code |

### Recommendation: **Expo with development builds**

KHADAMATI needs FCM, APNs, maps, and secure storage — **Expo Go alone is insufficient**. Use:

- **Expo SDK** (managed workflow + dev client)
- **EAS Build** for store binaries
- **Config plugins** for Firebase when needed

Use **bare React Native CLI** only if a future requirement demands deep native customization (e.g., custom BLE, complex background services) that Expo cannot support.

---

## 6. Web frontend — keep or replace?

| Option | Verdict |
|--------|---------|
| Keep React + TypeScript | **Yes** — admin portal and customer web are substantial |
| Replace MUI | **No strong reason** — MUI 9 is production-ready, RTL-capable |
| Share code with RN | **Selective** — share API types, validation schemas (Zod), i18n keys, business constants — not UI components |

**Do not replace React web** for MVP or scale goals.

---

## 7. Summary scorecard

| Goal | Current stack | With RN mobile |
|------|---------------|----------------|
| Launch MVP | Good (mobile already built natively) | Migration delays MVP unless phased |
| Scale to 5K CCU | Achievable with infra plan | Same |
| 50K registered users | Achievable | Same |
| Reduce dev cost | Moderate | **Better long-term** |
| Reduce maintenance | Poor (dual mobile) | **Better** |
| Reduce hiring difficulty | Poor | **Better** |
| Preserve enterprise architecture | **Strong** | **Strong** (if API unchanged) |

---

## 8. Related documents

- `MOBILE_MIGRATION_PLAN.md` — migration approach
- `SCALABILITY_PLAN.md` — 5K–50K user architecture
- `HOSTING_ARCHITECTURE.md` — environments and HA
- `ROADMAP.md` — phased plan
- `FINAL_RECOMMENDATION.md` — go/no-go decisions
