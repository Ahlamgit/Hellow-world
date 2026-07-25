# Final Architecture Decisions Complete

**Document ID:** KHAD-V1-DECISIONS-COMPLETE  
**Date:** 2026-07-24  
**Phase:** Architecture finalization (no production code / no UI implementation)

---

## 1. Final ADR List (001–032)

| ADR | Decision | Status |
|-----|----------|--------|
| 001 | Lebanon default Market; multi-market ready | Accepted |
| 002 | Stores = service providers; no e-commerce (amended by 027) | Accepted |
| 003 | Unified Provider + Listing | Accepted |
| 004 | Financial ledger mandatory | Accepted |
| 005 | Booking: confirm then pay | Accepted |
| 006 | Admin web-only + MFA | Accepted |
| 007 | Admin-managed promotions | Accepted |
| 008 | No permanent auto-block without review | Accepted |
| 009 | Chat in V1 | Accepted |
| 010 | Design as inspiration; analyze & improve | Accepted |
| 011 | Flutter mobile stack | Accepted |
| 012 | Redis + workers required | Accepted |
| 013 | Admin-configurable financial business rules | Accepted |
| 014 | Account deletion (detail → 022) | Superseded by 022 |
| 015 | Chat scoping (detail → 020) | Superseded by 020 |
| 016 | Scheduling options | Superseded by 019 |
| 017 | Dispute options | Superseded by 021 |
| 018 | Dark theme conditional on assets | Accepted |
| 019 | Provider Availability Calendar scheduling | Accepted |
| 020 | Booking-scoped chat architecture | Accepted |
| 021 | Lightweight dispute management | Accepted |
| 022 | Account retention & deletion | Accepted |
| 023 | Design asset readiness gate | Accepted |
| 024 | Cloud & DR (vendor-neutral) | Accepted |
| 025 | External integration ports/adapters | Accepted |
| 026 | Lebanon finance values = Admin config | Accepted |
| **027** | Store promotional product catalog (non-transactional) | **Accepted** |
| **028** | Unified Provider capability model; service-first UX | **Accepted** |
| **029** | Simplified customer/provider payment experience; internal ledger control | **Accepted Architecture Direction** |
| **030** | Booking / payment / ledger / settlement state separation | **Accepted Architecture Direction** |
| **031** | Payment failure, retry, and recovery strategy | **Accepted Architecture Direction** |
| **032** | Notification delivery architecture (event-driven, vendor-agnostic) | **Accepted Architecture Direction** |

---

## 2. Approved Decisions (Summary)

### Marketplace & Booking
- **Service-first** discovery (no mandatory Craftsman/Store chooser) — ADR-028  
- Provider Type + **Capabilities**; booking via `CanAcceptBookings`  
- Unified payments/ledger for all providers  
- Provider Availability Calendar controls bookable times  
- Flow: Search service → Providers → Book → Confirm → Pay → Service   

### Chat
- Booking-scoped only; Customer ↔ Provider; optional audited admin support  
- Real-time + DB history + read status + push  
- No open marketplace messaging  

### Disputes
- Lightweight complaints + admin review + evidence + notes  
- Statuses: Open → Under Review → Resolved → Closed  
- May hold escrow/settlement; no complex arbitration  

### Account lifecycle
- Deletion request → verify → anonymize → retain finance/audit per configurable retention  

### Design
- UI coding blocked until branding/logo/colors/references/video/screens + direction exist  
- Then analysis, tokens, inventory, screen specs before UI build  

### Infrastructure & integrations
- Portable cloud topology; backups/DR/monitoring defined without vendor lock  
- All externals behind adapters (Payment.js/IXOPAY, SMS, Email, Maps, OCR, Face)  

### Finance
- Commission, cancellation, refund, withdrawal, settlement **policies** admin-configurable  
- Lebanon commercial **values** configured in Admin Portal — never hardcoded  
- **Customer/provider payment UX simplified** — ledger, commission, and settlement remain internal (ADR-029)  
- Customers see payment status only; providers see earnings summaries — not ledger operations  
- **Separate domain states** for booking, payment, ledger, settlement (ADR-030)  
- **Payment failure/recovery** event-driven and idempotent (ADR-031)  

### Notifications
- Business modules emit events; notification module delivers (ADR-032)  
- In-app primary; push secondary; SMS/email optional via ports  
- Chat remains booking-scoped (ADR-020); notifications alert only — no open messaging  

---

| Risk | Severity | Mitigation |
|------|----------|------------|
| Design assets delayed | High | ADR-023 hard gate |
| Payment.js Flutter WebView/3DS | High | Spike before payment UI |
| Policy misconfiguration by Finance | High | Staging validation + audit + feature flags |
| Vendor selection delays (SMS/OCR/Face) | Medium | Ports ready (ADR-025) |
| Availability UX complexity | Medium | Clear calendar APIs + conflict rules |
| Chat abuse within bookings | Medium | Report + admin access |
| Cloud vendor undecided | Low–Med | ADR-024 portable |

---

## 4. Implementation Blockers (Non-Code)

Coding remains **blocked** until:

| # | Blocker | Type |
|---|---------|------|
| 1 | Stakeholder **sign-off** on this decisions package | Governance |
| 2 | **Design assets** uploaded + UI/UX specification produced (ADR-023) | Design |
| 3 | **Vendor selections** for SMS, Email, Storage, Maps, OCR, Face (adapters only) | Procurement/Eng |
| 4 | **Cloud vendor** approval for prod (architecture ready) | DevOps/Business |
| 5 | **Finance** ready to load Lebanon policy configs before money enablement | Finance |
| 6 | **Compliance** numeric retention defaults for Lebanon production | Compliance |

Architecture decisions **019–032 are no longer open product forks** — they are accepted (ADR-029…032: architecture direction only; no implementation).

---

## 5. Final Readiness Status

| Dimension | Status |
|-----------|--------|
| Core domain architecture | **Approved** |
| Scheduling model | **Approved (ADR-019)** |
| Chat architecture | **Approved (ADR-020)** |
| Dispute depth | **Approved (ADR-021)** |
| Retention/deletion | **Approved (ADR-022)** — numeric policy values via config |
| Financial policy model | **Approved (ADR-013/026)** |
| Integration strategy | **Approved (ADR-025)** |
| Cloud/DR pattern | **Approved (ADR-024)** — vendor TBD |
| Payment / state / recovery model | **Approved (ADR-029/030/031)** |
| Notification delivery model | **Approved (ADR-032)** |
| Design / UI implementation | **Blocked (ADR-023)** |
| Production coding | **Blocked** |

### Recommendation

**Architecture decisions: COMPLETE for product/domain forks listed above.**  

**Implementation status: NOT AUTHORIZED** until design gate + governance sign-off + vendor/cloud minimums above.

When design assets arrive and sign-off is recorded, amend readiness to **Ready for implementation (phased)** starting with non-UI backend foundation if Product explicitly allows — otherwise wait for full gate.

---

## 6. Document Cross-Links

- ADRs: [`docs/v1/adr/`](./adr/README.md) — includes **ADR-029…032** (payment & notification architecture)  
- **Consistency review:** [`FINAL_ARCHITECTURE_CONSISTENCY_REVIEW.md`](./FINAL_ARCHITECTURE_CONSISTENCY_REVIEW.md) — **validated 2026-07-25**  
- Master Prompt: [`MASTER_IMPLEMENTATION_PROMPT_v1.0.md`](./MASTER_IMPLEMENTATION_PROMPT_v1.0.md)  
- FTM: [`FEATURE_TRACEABILITY_MATRIX.md`](./FEATURE_TRACEABILITY_MATRIX.md)  
- Readiness report: [`FINAL_ARCHITECTURE_READINESS_REPORT.md`](./FINAL_ARCHITECTURE_READINESS_REPORT.md)  
