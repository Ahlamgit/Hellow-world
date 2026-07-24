# 40. Development Roadmap

**Document ID:** KHAD-V1-ROADMAP  
**Status:** Draft for Approval  

Roadmap is capability-based (not calendar-day estimates). Sequencing minimizes rework around money, auth, and booking.

---

## 40.1 Phase 0 — Approval & Foundations

- Approve architecture pack  
- Resolve blocking `Q-*` decisions (see register)  
- Upload UI/branding assets  
- Repository cutover structure for Spring/Flutter/React  
- Establish CI skeletons, compose, coding standards  

## 40.2 Phase 1 — Platform Kernel & IAM

- Users, roles, permissions, JWT auth  
- Admin login  
- Audit skeleton  
- Settings/feature flags  
- OpenAPI baseline  

## 40.3 Phase 2 — Profiles & Catalog

- Customer/craftsman/store profiles  
- Categories, services, products  
- Media upload  
- Store dashboard catalog CRUD  
- Customer browse/search read APIs  

## 40.4 Phase 3 — Booking Engine

- Booking aggregate + history  
- Customer create/track  
- Craftsman/store job views  
- Reminder scheduler foundation  

## 40.5 Phase 4 — Payments (IXOPAY)

- Payment port + adapter  
- Payment.js web + Flutter WebView  
- Webhooks + idempotency  
- Pay booking + pay subscription  

## 40.6 Phase 5 — Onboarding, IDV, Approval

- Document upload + OCR integration  
- Face/GPS/QR/OTP  
- Admin approval queue  
- Entitlement hooks  

## 40.7 Phase 6 — Subscriptions, Commissions, Withdrawals

- Plans & entitlements  
- Commission engine after rules decided  
- Settlement overview  
- Withdrawal requests + admin decision  

## 40.8 Phase 7 — Notifications Completeness

- Templates admin  
- Email/SMS/push/in-app production wiring  
- Event coverage matrix  

## 40.9 Phase 8 — Ads, Ratings, Quality

- Ads workflow  
- Ratings/reviews moderation  
- Surveys, scoring, restrictions  
- Ops dashboards  

## 40.10 Phase 9 — Reporting, Hardening, Launch

- Reports/exports  
- Security hardening & pen test  
- Performance smoke  
- Staging → production runbooks  
- App store releases  

## 40.11 Parallel Tracks

| Track | Parallel with |
|-------|---------------|
| Admin portal vertical slices | Each backend phase |
| Store dashboard | Phases 2–4, 8 |
| Customer app | Phases 2–4, 7–8 |
| Craftsman app | Phases 2–6, 7–8 |
| DevOps | Continuous from Phase 0 |

## 40.12 Exit Criteria for V1

All P0 FRS items accepted; blocking questions resolved or explicitly deferred with safe disabled flags; production deployment + monitoring live.
