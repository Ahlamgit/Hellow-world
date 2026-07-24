# 41. Milestones

**Document ID:** KHAD-V1-MS  
**Status:** Draft for Approval  

Milestones are outcome-based gates.

| Milestone | Outcome | Entry Criteria | Exit Criteria |
|-----------|---------|----------------|---------------|
| **M0 Architecture Approved** | Docs signed off | Pack reviewed | Open questions dispositioned (answered/deferred) |
| **M1 Design Assets In** | UI video/screens/brand available | M0 | Design tokens extracted; no coding UI before this |
| **M2 Skeleton Running** | API + web shells + flutter shells + CI green | M0 | Health endpoints; auth login demo on staging |
| **M3 Marketplace Read** | Browse catalog works end-to-end | M2 | Customer can browse categories/stores/services |
| **M4 Booking Vertical Slice** | Create & track booking (without final payment if needed) | M3 | State machine + history + basic notifications |
| **M5 Payments Live (Sandbox)** | IXOPAY Payment.js debit + webhook | M4 + payment questions | Captured payment confirms booking/subscription |
| **M6 Trusted Supply** | Onboarding + IDV + admin approval | M5 | Approved craftsman can access gated features |
| **M7 Monetization** | Subscriptions + commissions + withdrawals overview | M6 + commission decisions | Finance admin can reconcile overview |
| **M8 Quality & Growth** | Ads + ratings + quality follow-up | M7 | Reminders/surveys/restrictions operational |
| **M9 Production Readiness** | Hardening complete | M8 | Security/perf/runbooks/backup restore tested |
| **M10 V1 Launch** | Production go-live | M9 | Smoke green; support playbooks active |

Each milestone requires demo evidence (API + relevant clients) on staging.
