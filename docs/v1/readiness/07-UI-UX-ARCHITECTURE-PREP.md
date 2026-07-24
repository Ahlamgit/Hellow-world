# UI/UX Architecture Preparation (Specification Only)

**Document ID:** KHAD-V1-UI-SPEC-PREP  
**Status:** Spec inventory — **NOT implementation**  
**Design stance:** ADR-010 — inspiration from assets; analyze & improve; produce tokens before UI coding  

---

## 1. Preconditions Before UI Build

1. Design assets uploaded (video, screenshots, branding, colors, UX notes)  
2. UI analysis + UX analysis completed  
3. Design tokens extracted  
4. Component inventory mapped to screens below  
5. Navigation maps approved  
6. Accessibility + RTL review  

---

## 2. Screen Inventory

### Customer App (Flutter)

| Area | Screens |
|------|---------|
| Authentication | Splash, Register, Login, OTP, Reset |
| Home | Home / discovery |
| Search | Search, Filters, Results, Proximity |
| Listing | Listing details, Provider profile |
| Booking | Schedule, Address, Notes, Request confirm, Status tracker |
| Payment | Payment.js WebView, 3DS, Result |
| Tracking | Active job, Verification waiting states |
| Reviews | Rate/review, Survey |
| Chat | Conversation list, Thread |
| Profile | Profile, Addresses, Settings, Notifications inbox |
| History | Booking history detail |

### Craftsman App (Flutter)

| Area | Screens |
|------|---------|
| Auth | Register, Login, OTP |
| Dashboard | KPIs overview |
| Jobs | Inbox, Detail, Accept/Reject, Schedule/day view |
| Verification field | GPS, Selfie, QR/OTP |
| Job progress | Start/Complete flows |
| Availability | Windows editor |
| Catalog | Listings/services/pricing |
| Onboarding | Wizard, Documents, Status |
| Earnings | Earnings, Withdrawal request/history |
| Subscription | Plans, Status |
| Chat | Threads |
| Profile / Notifications | |

### Store Dashboard (React Web)

| Area | Screens |
|------|---------|
| Auth | Login |
| Profile | Store profile, verification status |
| Services | Listing CRUD (bookable services) |
| **Catalog** | Promotional product catalog CRUD (no cart) |
| Inquiries | Catalog leads |
| Staff | Affiliations |
| Bookings | List, detail, respond |
| Subscription | Plan status / limits |
| Promotions | Service + catalog ad eligibility |
| Analytics | Views, impressions, inquiries, conversions |
| Notifications | |

### Admin Portal (React Web Only)

| Area | Screens |
|------|---------|
| Auth | Login, MFA |
| Dashboard | Ops KPIs |
| Users | Customers, Craftsmen, Stores, Admin users/roles |
| Providers | Onboarding queues, IDV dossier, status |
| Finance | Ledger views, withdrawals approval, settlements, financial reports |
| **Policies** | Commission, Cancellation, Refund, Withdrawal config, Settlement rules (+ history) |
| Subscriptions | Plans, subscribers |
| Promotions | Packages, featured, slots |
| Notifications | Templates, channels, triggers |
| Trust | Ratings moderation, restrictions review |
| Reports / Analytics | |
| Settings | Global settings, markets |
| Audit | Audit log explorer |

---

## 3. Navigation Principles

- Audience-specific shells (never admin in mobile)  
- Deep links: booking, payment return, survey, chat  
- Entitlement gates → subscription paywall for craftsmen  

---

## 4. Out of Scope for This Document

Pixel layouts, colors, and component visuals — wait for design analysis deliverables.
