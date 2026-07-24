# 42. Sprint Breakdown

**Document ID:** KHAD-V1-SPRINTS  
**Status:** Draft for Approval  

Assumes **2-week sprints**. Capacity not estimated in calendar-people terms; scope is sequenced for a cross-functional team (backend, web, flutter, QA, DevOps).

---

## Sprint 0 — Mobilization
- Resolve blocking open questions workshop  
- Ingest design assets (if ready)  
- Repo structure, CI, compose, ADR template  
- Coding standards enforcement tooling  

## Sprint 1 — IAM Foundation
- Users/roles/permissions schema  
- Register/login/refresh/logout  
- Admin auth shell  
- Audit event writer  

## Sprint 2 — Profiles & Media
- Customer/craftsman/store profiles  
- Presigned media uploads  
- Admin user/customer/craftsman/store lists  

## Sprint 3 — Catalog
- Categories translations readiness  
- Craftsman/store services & products CRUD  
- Customer browse/search APIs + initial app screens  

## Sprint 4 — Booking Core
- Booking create/list/detail  
- Status history + transition service  
- Store/craftsman job lists  

## Sprint 5 — Booking UX + Reminders
- Customer booking wizard (per designs)  
- 24h reminder job  
- Notification outbox MVP (in-app)  

## Sprint 6 — Payments Adapter
- Payment intents + IXOPAY adapter  
- Webhook idempotency  
- Web Payment.js integration  
- Flutter payment WebView  

## Sprint 7 — Payables Connected
- Pay booking → confirm  
- Subscription plans + subscribe payment  
- Failure/retry UX  

## Sprint 8 — Onboarding & Approval
- Document checklist flow  
- Admin approval queue  
- OCR integration spike→prod path  

## Sprint 9 — Job Verification
- GPS checks  
- Selfie/face checks  
- QR/OTP challenges  
- Booking transitions gated by verification  

## Sprint 10 — Commissions & Settlements
- Rule admin (after Q-COM decisions)  
- Commission lines  
- Settlement overview  
- Withdrawal request/approve  

## Sprint 11 — Notifications Full Channel
- Templates admin  
- Email + SMS providers  
- Push  
- Event matrix completion  

## Sprint 12 — Ads, Ratings, Quality
- Ads campaign workflow  
- Ratings/reviews moderation  
- Surveys + scoring + restrictions  
- Ops dashboards  

## Sprint 13 — Reporting & Hardening
- Store/admin reports/exports  
- Security fixes, perf smoke  
- Pen test remediation buffer  

## Sprint 14 — Launch Prep
- Prod deploy dry run  
- Backup/restore drill  
- App store submissions  
- Support runbooks  

> Sprint count may compress/expand after velocity known and after open questions resolution. Do not treat sprint numbers as calendar commitments.

## Sprint Ceremonies (Suggested)
Planning, daily sync, mid-sprint integration demo, review against acceptance criteria, retrospective.
