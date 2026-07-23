# KHADAMATI — Mobile Migration Plan (Kotlin + Swift → React Native)

**Document type:** Migration analysis  
**Status:** Draft — awaiting approval  
**No implementation authorized by this document**

---

## 1. Purpose

Define how KHADAMATI would migrate from **two native mobile codebases** to a **single React Native + TypeScript** application while preserving the existing ASP.NET Core API and enterprise architecture.

---

## 2. Current mobile inventory

### 2.1 Android (`src/android`)

| Layer | Files / modules | Notes |
|-------|-----------------|-------|
| UI (Compose) | ~20 screens | Auth, home, services, booking wizard, bookings, profile, portals, chat, support, subscriptions |
| ViewModels | 9 | MVVM pattern |
| Repositories | 10 | One per domain |
| API | Retrofit `ApiService` | ~40 user-facing endpoints |
| Local | Room | Services + user cache |
| Push | FCM | `KhadamatiFirebaseMessagingService` |
| Location | GPS | Nearby craftsmen, addresses |
| i18n | EN + AR | `strings.xml`, `values-ar` |

**Approximate size:** ~78 Kotlin source files, ~7,000+ LOC

### 2.2 iOS (`src/ios`)

| Layer | Files / modules | Notes |
|-------|-----------------|-------|
| UI (SwiftUI) | ~25 views | Feature parity with Android |
| ViewModels | 8 | Observable pattern |
| API | `APIClient` + `APIEndpoints` | Same REST surface as Android |
| Local | None (network-first) | Weaker offline than Android |
| Push | FCM + APNs | `PushNotificationManager` |
| Location | CoreLocation | Booking wizard |
| i18n | EN + AR | `Localizable.strings` |

**Approximate size:** ~62 Swift files, ~5,500+ LOC

### 2.3 Feature parity matrix (current native)

| Feature | Android | iOS |
|---------|---------|-----|
| Login / Register | ✅ | ✅ |
| Password recovery | ✅ | ✅ |
| Services catalog | ✅ | ✅ |
| Booking wizard + nearby | ✅ | ✅ |
| Booking lifecycle | ✅ | ✅ |
| Payments | ✅ | ✅ |
| Profile + edit | ✅ | ✅ |
| Addresses + GPS | ✅ | ✅ |
| Notifications | ✅ | ✅ |
| Chat | ✅ | ✅ |
| Support (complaints/tickets) | ✅ | ✅ |
| Subscriptions | ✅ | ✅ |
| Craftsman portal | ✅ | ✅ |
| Store portal | ✅ | ✅ |
| Verification doc upload | ❌ | ❌ |
| Working hours editor | ❌ | ❌ |
| Admin | ❌ (by design) | ❌ |

---

## 3. Target React Native architecture

```
src/mobile/                          # New monorepo path (proposed)
├── app/                             # Expo Router or React Navigation
│   ├── (auth)/                      # login, register, recovery
│   ├── (tabs)/                      # home, services, bookings, profile
│   └── (modals)/                    # booking wizard, chat
├── features/                        # feature modules (mirror web domains)
│   ├── auth/
│   ├── bookings/
│   ├── services/
│   ├── portals/
│   └── ...
├── shared/
│   ├── api/                         # typed client (axios/fetch)
│   ├── i18n/                        # shared with web where possible
│   ├── storage/                     # secure tokens
│   └── types/                       # DTOs (shared package)
├── app.json / eas.json
└── package.json
```

**Recommended stack:**
- Expo SDK + **development builds**
- TypeScript (strict)
- React Navigation or Expo Router
- TanStack Query (server state)
- Zustand or Context (session)
- `expo-secure-store` (tokens)
- `expo-notifications` + Firebase
- `expo-location` + `react-native-maps`
- i18next (align with web)

---

## 4. Migration strategy

### 4.1 Recommended approach: **Strangler parallel run**

Do **not** big-bang delete native apps until RN reaches feature parity.

| Phase | Native apps | React Native |
|-------|-------------|--------------|
| Phase 0 | Production / internal testing | Bootstrap shell + auth |
| Phase 1 | Primary | Beta (TestFlight / internal track) |
| Phase 2 | Maintenance only | Primary |
| Phase 3 | Deprecated | Sole mobile client |

### 4.2 Screen migration order (by risk and dependency)

| Wave | Screens | Rationale |
|------|---------|-----------|
| **W1** | Splash, login, register, password recovery | Auth foundation |
| **W2** | Home, services list, profile (read-only) | Read-heavy, low risk |
| **W3** | Booking list, booking detail | Core value |
| **W4** | Booking wizard (GPS, slots, payment) | Highest complexity |
| **W5** | Notifications, chat, support | Real-time / polling |
| **W6** | Subscriptions, craftsman portal, store portal | Role-specific |
| **W7** | Addresses, profile edit, verification upload | Completeness |

### 4.3 What is reused without migration

| Asset | Reusable? | How |
|-------|-----------|-----|
| **REST API** | ✅ 100% | Same `api/v1` contract |
| **JWT auth** | ✅ 100% | Login, refresh, revoke |
| **DTO shapes** | ✅ ~95% | Generate TypeScript from OpenAPI or hand-port from Kotlin/Swift models |
| **Business logic** | ✅ 100% on server | MediatR handlers unchanged |
| **Validation rules** | ✅ Server-side | FluentValidation remains authoritative |
| **RBAC / permissions** | ✅ Server-side | Mobile is a thin client |
| **Database** | ✅ 100% | No mobile data layer migration |
| **Push backend** | ✅ 100% | Same `POST /devices/push-token` |
| **Payment gateway** | ✅ 100% | Moyasar webhooks unchanged |
| **Web admin** | ✅ 100% | Unaffected |

### 4.4 What must be rewritten

| Asset | Effort | Notes |
|-------|--------|-------|
| All UI screens | High | ~45 screens/modals across both platforms → ~25 RN screens |
| Navigation | Medium | Single graph vs two |
| API client | Medium | Port Retrofit/URLSession → axios + TanStack Query |
| Token storage | Low | Secure store wrappers |
| Room / local cache | Medium | Optional SQLite or query cache |
| Platform permissions | Low | Expo permission APIs |
| i18n | Medium | Consolidate 3 sources → 2 (web + mobile shared core) |
| CI/CD pipelines | Medium | EAS Build + store submission |

**Code reuse estimate:** ~0% UI, ~70% API contract, ~100% backend logic, ~30% TypeScript types shared with web.

---

## 5. Complexity estimate

| Workstream | Complexity (1–5) | Duration indicator |
|------------|------------------|--------------------|
| RN project bootstrap + CI | 2 | Short |
| Auth + secure storage | 2 | Short |
| Services + home | 2 | Short |
| Bookings (full wizard) | 5 | Long — GPS, slots, state machine |
| Portals (craftsman/store) | 3 | Medium |
| Chat + notifications | 3 | Medium |
| Subscriptions | 3 | Medium |
| i18n + RTL | 3 | Medium — Arabic RTL critical for Lebanon |
| Parity testing | 4 | Medium-long |
| Store submission | 2 | Short per platform |

**Overall migration complexity:** **High (4/5)** — not because RN is hard, but because **booking + payments + multi-role** flows demand careful QA.

**Calendar-free effort characterization:** Roughly equivalent to **building one new native app plus 60% of the second** — but yields a single codebase going forward.

---

## 6. Risks

| Risk | Severity | Mitigation |
|------|----------|------------|
| MVP delay if migration starts before launch | **High** | Ship native MVP OR freeze native and accept delay — see `ROADMAP.md` |
| RN performance on low-end Android | Medium | List virtualization, Hermes, profiling |
| Push notification regressions | Medium | Parallel run; device matrix testing |
| Payment WebView UX | Medium | Deep links; test Moyasar return URLs |
| RTL layout bugs | Medium | Early Arabic QA; use RN RTL support |
| Team lacks RN experience | Medium | Training; hire 1 senior RN lead |
| Expo native module limits | Low | Dev builds + config plugins |
| Security regression (token storage) | Medium | Security review; pen test before prod |
| API contract drift | Low | OpenAPI codegen in CI |

---

## 7. Benefits

| Benefit | Impact |
|---------|--------|
| Single mobile codebase | **−40% mobile maintenance** (est.) |
| Shared TypeScript with web | Faster feature delivery |
| One release train | Synchronized iOS + Android |
| Easier hiring | React developers are more available than Swift+Kotlin pairs |
| Close feature gaps once | Verification upload, schedules in one place |
| Consistent offline strategy | Replace Android-only Room asymmetry |

---

## 8. Authentication migration

### Current flow (both platforms)

```
POST /api/v1/auth/login → JWT access + refresh
POST /api/v1/auth/refresh → rotated tokens
POST /api/v1/auth/revoke → logout
GET  /api/v1/users/me → profile
```

### RN implementation (proposed)

1. `expo-secure-store` for access + refresh tokens
2. Axios interceptor attaches `Authorization: Bearer`
3. 401 → refresh → retry (same as `RetrofitClient` / `APIClient` today)
4. Send `Device.Platform: "iOS" | "Android"` on login for mobile role policy
5. **Block admin roles** — handle 403 from planned server policy

**No backend auth rewrite required** — only optional mobile-client detection enhancement.

---

## 9. DTO and type sharing strategy

### Option A — OpenAPI codegen (recommended)

1. Export Swagger from `Khadamati.API` (already available)
2. Generate `@khadamati/api-client` TypeScript package
3. Consumed by `src/web` and `src/mobile`

### Option B — Shared manual types

1. Create `packages/shared-types/` in monorepo
2. Manually sync when API changes
3. Lower tooling cost, higher drift risk

---

## 10. Decommission plan (post-parity)

| Step | Action |
|------|--------|
| 1 | RN app at 100% feature parity checklist |
| 2 | 2 weeks internal dogfood |
| 3 | Beta via TestFlight + Play Internal Testing |
| 4 | Production release; native apps marked legacy |
| 5 | 90-day freeze on `src/android` and `src/ios` |
| 6 | Archive native repos or move to `legacy/` |
| 7 | Remove native CI jobs |

---

## 11. Decision gate

Proceed with React Native migration **only if**:

- [ ] API v1 is frozen for mobile-critical flows
- [ ] Mobile scope document approved (Customer/Craftsman/Store only)
- [ ] Team has or will hire RN lead
- [ ] MVP timeline allows parallel run OR native MVP ships first
- [ ] Budget for EAS + device testing farm

**Do not proceed** if immediate MVP launch is the only priority and native apps are already test-ready — in that case, **ship native first**, migrate in Phase 2.

---

## 12. Related documents

- `TECHNOLOGY_EVALUATION.md`
- `ROADMAP.md`
- `FINAL_RECOMMENDATION.md`
