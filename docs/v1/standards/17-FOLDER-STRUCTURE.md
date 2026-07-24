# 17. Folder Structure

**Document ID:** KHAD-V1-FOLDERS  
**Status:** Draft for Approval  

Proposed monorepo layout for the **new** V1 codebase. Prior `src/backend` (.NET) etc. are legacy and must not be extended for V1.

---

## 17.1 Repository Root

```text
/
├── apps/
│   ├── admin-portal/                 # React + TS + MUI
│   ├── store-dashboard/              # React + TS + MUI
│   ├── customer_app/                 # Flutter
│   └── craftsman_app/                # Flutter
├── packages/
│   ├── ui/                           # shared web UI kit
│   ├── api-client/                   # shared TS API client
│   ├── auth/                         # web auth helpers
│   └── i18n/                         # web i18n resources
├── backend/
│   ├── pom.xml
│   ├── khadamati-api/
│   ├── khadamati-commons/
│   └── khadamati-modules/
│       ├── khadamati-platform/
│       ├── khadamati-iam/
│       ├── khadamati-customer/
│       ├── khadamati-craftsman/
│       ├── khadamati-store/
│       ├── khadamati-catalog/
│       ├── khadamati-booking/
│       ├── khadamati-payment/
│       ├── khadamati-commission/
│       ├── khadamati-subscription/
│       ├── khadamati-notification/
│       ├── khadamati-identity-verification/
│       ├── khadamati-ads/
│       ├── khadamati-rating/
│       ├── khadamati-quality/
│       ├── khadamati-reporting/
│       ├── khadamati-media/
│       └── khadamati-audit/
├── db/
│   └── migration/                    # Flyway SQL
├── docs/
│   └── v1/                           # this architecture pack
├── deploy/
│   ├── docker/
│   ├── compose/
│   └── k8s/                          # optional later
├── scripts/
├── .github/workflows/
├── docker-compose.yml
├── README.md
└── .env.example
```

## 17.2 Backend Module Internal Layout

```text
khadamati-booking/
  src/main/java/com/khadamati/booking/
    domain/
    application/
      port/in/
      port/out/
      service/
    adapter/in/web/
    adapter/in/scheduler/
    adapter/out/persistence/
    adapter/out/messaging/
  src/test/java/...
```

## 17.3 Flutter App Layout

```text
apps/customer_app/
  lib/
    main.dart
    app/
    features/
    l10n/
    di/
  test/
  integration_test/
```

## 17.4 Web App Layout

```text
apps/admin-portal/src/
  main.tsx
  app/
  routes/
  features/
  shared/
  theme/          # brand tokens injected later
```

## 17.5 Migration Note

During transition, legacy folders under current `/src` may remain read-only reference but V1 implementation must live in the new structure above (or replace root after approval). Exact cutover mechanics are an engineering release decision, not a business rule.
