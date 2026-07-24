# 11. Frontend Architecture

**Document ID:** KHAD-V1-FE  
**Status:** Draft for Approval  
**Apps:** Administration Portal · Store Dashboard  
**Stack:** React · TypeScript · Material UI  

---

## 11.1 Important Constraint

**No UI implementation until design assets are uploaded and approved.**  
This document defines technical architecture only. Visual system (colors, typography, layout) will follow provided branding — not generic MUI templates.

## 11.2 Application Split

| App | Package | Audience |
|-----|---------|----------|
| `admin-portal` | `apps/admin-portal` | Platform admins |
| `store-dashboard` | `apps/store-dashboard` | Store operators |

Shared libraries are encouraged:

| Lib | Purpose |
|-----|---------|
| `packages/ui` | Design-system wrappers around MUI mapped to brand tokens |
| `packages/api-client` | OpenAPI-generated or hand-maintained typed client |
| `packages/auth` | Token storage, refresh interceptor, route guards |
| `packages/i18n` | i18next resources, RTL support |
| `packages/utils` | formatting (money/date), validation helpers |

Monorepo tool recommendation: pnpm workspaces + Vite (Q-FE-001 finalization).

## 11.3 Layering (Each App)

```text
app/          # routes, providers, bootstrap
pages/        # route-level compositions
features/     # domain features (users, bookings, ads)
entities/     # shared entity types & queries (optional)
shared/       # ui kit usage, hooks, lib
```

Prefer **feature-based** folders over type-based dumping (`components/`, `hooks/` only at shared level).

## 11.4 State & Data Fetching

Recommended V1 stack:

| Concern | Choice |
|---------|--------|
| Server state | TanStack Query |
| Client UI state | React state / lightweight store (Zustand optional) |
| Forms | React Hook Form + Zod |
| Routing | React Router |
| Tables | MUI DataGrid or branded table wrapper per designs |
| Charts | Deferred to design; architecture allows pluggable chart lib |

Avoid duplicating server state in Redux unless product complexity demands it.

## 11.5 Auth UX Architecture

1. Login page submits credentials  
2. Store access token in memory; refresh token in HttpOnly cookie **or** secure persistent storage (decision Q-FE-002 — cookies preferred for web)  
3. Axios/fetch interceptor refreshes on 401 once  
4. Route guards check auth + permission claims  
5. Admin portal enforces permission-based menu visibility (hide) **and** server enforces authz (deny)

## 11.6 Internationalization & RTL

- All user-facing strings via i18n keys  
- RTL switch for Arabic  
- MUI theme direction bound to locale  
- Date/number/money formatting via Intl APIs with currency code from API  

## 11.7 Theming Architecture (Brand-Ready)

```text
theme/
  tokens.ts      # colors, radii, spacing from brand (filled later)
  typography.ts  # brand fonts (filled later)
  components.ts  # MUI component overrides
  ThemeProvider
```

Until brand assets arrive: **do not invent a marketing look**. Architecture only.

## 11.8 Security Considerations (Web)

- CSP headers from hosting layer  
- No tokens in localStorage if cookie model chosen  
- Sanitize any HTML from templates preview  
- Least privilege UI; never trust hidden buttons as security  

## 11.9 Admin Portal Feature Modules

`users`, `customers`, `craftsmen`, `stores`, `onboarding`, `notifications`, `commissions`, `settlements`, `subscriptions`, `ads`, `ratings`, `settings`, `audit`, `analytics`, `reports`, `quality`

## 11.10 Store Dashboard Feature Modules

`profile`, `products`, `services`, `orders`, `bookings`, `ads`, `customers`, `reports`

## 11.11 Error & Empty States

Central error boundary + normalized API problem-details mapping to toasts/inline alerts. Exact visuals follow design system.

## 11.12 Questions Requiring Business Decision

- Q-FE-001: Monorepo tooling (pnpm/nx/turbo)  
- Q-FE-002: Web token storage strategy (HttpOnly cookie vs Bearer in memory + refresh)
