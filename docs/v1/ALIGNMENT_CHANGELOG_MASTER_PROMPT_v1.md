# Architecture Alignment Changelog — Master Prompt v1.0

**Date:** 2026-07-24  
**Trigger:** Adoption of KHADAMATI Master Implementation & Architecture Prompt v1.0

This changelog records **superseding decisions**. Where earlier `docs/v1` drafts conflict, **Master Prompt + ADRs win**.

| Topic | Previous draft stance | New controlling stance | ADR |
|-------|----------------------|------------------------|-----|
| Launch market | Undecided (Q-LOC-*) | Lebanon default; USD; +961; AR-RTL + EN-LTR; Asia/Beirut; no hardcode | ADR-001 |
| Store products | Products/orders in scope | **Out** — stores are service providers only | ADR-002 |
| Provider model | Separate craftsman/store catalogs | Unified **Provider** + **Listing** | ADR-003 |
| Ledger | Recommended in audit | **Mandatory** | ADR-004 |
| Booking vs payment order | Often pay-then-confirm | **Confirm then pay** | ADR-005 |
| Admin channel | Web-only (BR-008) | Web-only + **no mobile admin APIs** + **MFA required** | ADR-006 |
| Ads | Store self-serve possible | **Admin-managed promotions** only | ADR-007 |
| Quality bans | Auto-restrictions possible | Warn/flag/restrict; **no permanent auto-block** | ADR-008 |
| Chat | Default V2 | **In V1** (customer) | ADR-009 |
| Design fidelity | Faithful reproduction | **Inspiration + analyze/improve** + design system deliverables first | ADR-010 |
| Mobile stack | Flutter | Flutter confirmed as approved mobile direction | ADR-011 |
| Money policies | Hardcoded TBD values | **Admin-configurable** commission/cancel/refund/withdrawal/settlement | ADR-013 |

## Implementation gate (from Master Prompt §20)

Coding remains blocked until: final architecture, DB model, API spec, **Feature Traceability Matrix**, UI/UX specification, security review, payment flow review, deployment architecture — all approved.
