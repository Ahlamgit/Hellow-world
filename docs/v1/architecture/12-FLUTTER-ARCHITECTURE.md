# 12. Flutter Architecture

**Document ID:** KHAD-V1-FLUTTER  
**Status:** Draft for Approval  
**Apps:** Customer Mobile · Craftsman Mobile  

---

## 12.1 Constraint

UI implementation waits for uploaded design video/screenshots/branding. This document defines structure, state management, navigation, and integration patterns only.

## 12.2 App Split

| App | Purpose |
|-----|---------|
| `customer_app` | Demand-side marketplace |
| `craftsman_app` | Supply-side operations + verification |

Shared Dart packages:

| Package | Purpose |
|---------|---------|
| `khad_core` | Result types, failures, value objects (Money, Id) |
| `khad_api` | OpenAPI/http client, DTO models, interceptors |
| `khad_auth` | Secure token storage, session cubit/bloc |
| `khad_ui` | Design-system widgets bound to brand tokens (later) |
| `khad_l10n` | Localization AR/EN + RTL |

## 12.3 Recommended Architecture Pattern

**Clean Architecture + feature-first modules**, with presentation state via **riverpod** or **bloc** (engineering choice Q-FL-001; both acceptable).

```text
features/<feature>/
  data/           # datasources, repositories impl, dto mapping
  domain/         # entities, repository ports, use cases
  presentation/   # screens, widgets, viewmodels
```

## 12.4 Cross-Cutting Mobile Concerns

| Concern | Approach |
|---------|----------|
| Auth tokens | flutter_secure_storage |
| Connectivity | Explicit offline messaging; queue non-critical actions carefully |
| Push | FCM; APNs via Firebase or native config |
| Deep links | Booking details, payment return, survey links |
| Media upload | Presigned URL flow to object storage |
| Maps/GPS | Platform geolocation permissions + accuracy metadata |
| Camera/selfie | Controlled capture pipeline for IDV |
| QR | camera QR scan |
| Payment.js | WebView/JS bridge or platform channel hosting Payment.js fields (see payment doc) |

## 12.5 Customer App Feature Map

`auth`, `profile`, `categories`, `search`, `stores`, `booking`, `payments`, `notifications`, `reviews`

## 12.6 Craftsman App Feature Map

`auth`, `profile`, `onboarding`, `documents`, `catalog`, `jobs`, `verification` (gps/selfie/qr-otp), `subscriptions`, `dashboard`, `earnings`, `withdrawals`, `notifications`

## 12.7 Navigation

- Auth shell vs main shell  
- Guard routes based on auth + onboarding/subscription entitlements  
- Craftsman: force onboarding wizard until submitted/approved per policy  

## 12.8 Payment.js on Mobile

Because Areeba IXOPAY Payment.js is a browser JavaScript library:

**Recommended V1 approach:**

1. Backend creates a payment session context (amount, currency, payable id)  
2. Mobile opens a secured Payment WebView hosting KHADAMATI-controlled HTML that loads Payment.js hosted fields  
3. JS tokenizes card → returns `transactionToken` to Flutter via JS bridge  
4. Flutter calls backend debit endpoint with token  
5. If 3DS required, continue in WebView using gateway URLs  
6. Finalize via webhook + client polling/return URL  

Alternative (if Areeba provides mobile SDK tokens): evaluate later; V1 constraint specifies Payment.js.

## 12.9 Verification UX Architecture

| Step | Capture | Client Responsibility | Server Responsibility |
|------|---------|----------------------|-----------------------|
| GPS | lat/lon/accuracy/timestamp | Collect & sign request | Compare to job location threshold |
| Selfie | image upload | Liveness UX TBD Q-IDV-004 | Face match + store metadata |
| QR | scan payload | Submit code | Validate challenge |
| OTP | user entry | Submit code | Validate one-time challenge |

## 12.10 Offline & Reliability

- Read-only cache for categories optional  
- Do not cache PAN/tokens  
- Idempotency keys generated client-side for booking/payment create  

## 12.11 Testing

- Widget tests for critical forms  
- Golden tests after design tokens exist  
- Integration tests for auth + booking happy path on staging  
- Contract tests against OpenAPI  

## 12.12 Questions Requiring Business Decision

- Q-FL-001: riverpod vs bloc  
- Q-IDV-004: liveness detection required in V1?  
- Minimum iOS/Android OS versions (Q-FL-002)
