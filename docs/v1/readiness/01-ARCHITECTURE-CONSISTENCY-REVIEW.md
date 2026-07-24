# Architecture Consistency Review

**Document ID:** KHAD-V1-CONSISTENCY  
**Date:** 2026-07-24  
**Sources:** Master Prompt v1.0 · FTM · ADR-001..013 · Admin Financial Rules · Architecture pack  

---

## 1. Consistency Summary

| Area | Status | Notes |
|------|--------|-------|
| Market (Lebanon default, multi-market ready) | Consistent | ADR-001 |
| Stores = services only (no products) | Consistent | ADR-002; older ER mentions of products superseded |
| Provider + Listing model | Consistent | ADR-003 |
| Ledger mandatory | Consistent | ADR-004 |
| Confirm → Pay booking | Consistent | ADR-005 |
| Admin web-only + MFA | Consistent | ADR-006 |
| Admin-managed promotions | Consistent | ADR-007 |
| No permanent auto-ban | Consistent | ADR-008 |
| Chat in V1 | Consistent | ADR-009; module added |
| Design = inspiration + improve | Consistent | ADR-010 |
| Flutter mobile | Consistent | ADR-011 |
| Redis + workers | Consistent | ADR-012 |
| Admin-configurable finance rules | Consistent | ADR-013 |

---

## 2. Conflicts Found (Resolved or ADR’d)

| Conflict | Resolution |
|----------|------------|
| Earlier drafts included store products/orders | **Superseded** by ADR-002 — treat as out of scope |
| Pay-before-confirm vs confirm-then-pay | **Superseded** by ADR-005 |
| Chat deferred vs in-scope | **Superseded** by ADR-009 |
| Pixel-perfect UI vs analyze/improve | **Superseded** by ADR-010 |
| Soft-delete applied globally vs financial immutability | **Clarified:** soft-delete for master data only; money + policy history immutable (ADR-004/013) |
| `craftsman`/`store` modules vs `provider`/`listing` | **Canonical:** Provider + Listing; old names alias only |
| Commission “open rates” vs needing engine | **Clarified:** structure configurable; values entered by Finance Admin (not hardcoded) |

---

## 3. Missing Entities (to finalize in DB design)

| Gap | Action |
|-----|--------|
| `markets` | Required — add to final DB |
| `providers` + profile extensions | Required |
| `listings` (replace dual service tables) | Required |
| `availability_windows` | Required |
| `otp_challenges` / `mfa_credentials` | Required |
| `conversations` / `messages` (chat) | Required |
| Policy history tables | Required (ADR-013) |
| `ledger_accounts` / `ledger_entries` | Required |
| `escrow_holds` (or ledger account subtype) | Required |
| `booking_cancellations` | Required |
| `completion_records` | Required |
| `locations` / address normalization | Required |

---

## 4. Missing / Thin Workflows

| Workflow | Gap | ADR |
|----------|-----|-----|
| User deletion / anonymization | Retention steps underspecified | ADR-014 |
| Chat lifecycle (create, abuse, retention) | Transport & scoping open | ADR-015 |
| Scheduling inventory (slots vs free datetime) | Open product choice | ADR-016 |
| Dispute after completion | Mentioned but not fully modeled | ADR-017 |
| Payment reconcile job SLA | Operational detail thin | Covered in readiness; vendor ops runbook later |

---

## 5. Duplicated Responsibilities

| Duplication | Ownership decision |
|-------------|-------------------|
| `rating` vs `quality` surveys | **Canonical module `trust`** owns ratings, reviews, surveys, restrictions |
| Admin “ops” module vs domain modules | Admin is **API facade** only — no separate business ownership |
| Store services vs craftsman services | **Unified `listings`** |
| Wallet balance vs ledger | **Ledger is source of truth**; balances are projections |

---

## 6. Unclear Ownership (Clarified)

| Concern | Owner module |
|---------|--------------|
| Commission rule CRUD | `commission` (+ admin facade) |
| Cancellation policy CRUD | `policy` / booking-policy service |
| Refund rule CRUD | `policy` / payment-policy service |
| Withdrawal method config | `ledger` + policy |
| Settlement rules | `ledger` + policy |
| Escrow hold/release | `ledger` (triggered by payment/booking events) |
| Chat | `chat` |
| Promotions | `ads` (admin-managed) |

---

## 7. Unresolved Architectural Decisions → New ADRs

| ADR | Topic | Why not assumed |
|-----|-------|-----------------|
| ADR-014 | Account deletion & data retention | Legal/compliance sensitive |
| ADR-015 | Chat transport & scoping | Realtime tech + abuse model |
| ADR-016 | Scheduling model (slots vs datetime window) | Affects booking schema/UX |
| ADR-017 | Dispute workflow depth in V1 | Finance + trust impact |
| ADR-018 | Dark theme in V1 design system | Design asset dependency |

These block **full** “Ready for implementation” for affected slices only; core marketplace can proceed once UI assets + vendor choices land.

---

## 8. Document Hygiene Actions Completed in This Package

- Final logical DB design published  
- API architecture map published  
- RBAC matrix published  
- Workflows validated  
- UI screen inventory (spec only)  
- Security checklist  
- Deployment readiness  
- Final readiness report with explicit **Blocked** list  
