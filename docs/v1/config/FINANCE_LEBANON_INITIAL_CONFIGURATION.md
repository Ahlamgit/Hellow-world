# KHADAMATI V1 — Finance Lebanon Initial Configuration

**Document ID:** KHAD-V1-FINANCE-LEBANON  
**Version:** 1.0  
**Date:** 2026-07-24  
**Role:** Financial Domain Architect  
**BLOCKER-005 status:** **IN PREPARATION**  

**Market:** Lebanon (default) — ADR-001  
**ADR:** ADR-013 · ADR-026  

**Sources:**  
[`../FINAL_ARCHITECTURE_DECISIONS_COMPLETE.md`](../FINAL_ARCHITECTURE_DECISIONS_COMPLETE.md) · ADR-013 · ADR-026 · [`../payment/PAYMENT_JS_MOBILE_VALIDATION_REPORT.md`](../payment/PAYMENT_JS_MOBILE_VALIDATION_REPORT.md) · [`../governance/STAKEHOLDER_SIGN_OFF_PACKAGE.md`](../governance/STAKEHOLDER_SIGN_OFF_PACKAGE.md)

**Companion (earlier template):** [`FINANCE_POLICY_INITIAL_CONFIGURATION.md`](./FINANCE_POLICY_INITIAL_CONFIGURATION.md) — superseded for tracking by this document.

```text
DO NOT write code.
DO NOT create database tables.
DO NOT implement payment.
DO NOT hardcode financial rules.

All commercial values are Admin-configurable.
Where business approval is required: Pending Business Decision.
```

---

## Status Snapshot

| Item | State |
|------|-------|
| Architecture (policy engines) | **APPROVED** |
| Payment architecture | **APPROVED** — mobile validation pending (BLOCKER-007) |
| Business policies | **Admin configurable** (ADR-013) |
| Lebanon initial values | **Pending Business Decision** |
| BLOCKER-005 | **IN PREPARATION** |
| BLOCKER-005 COMPLETED | ☐ No — requires approved finance values |

**Runtime source of truth:** Administration Portal (Finance Admin / Super Admin) with MFA, RBAC, audit, and change history.  
Seed/demo data (if any later) must never be production business truth.

---

# 1. Currency

| Item | Value |
|------|-------|
| Default market | **Lebanon** |
| Default currency | **USD** |

### 1.1 Currency handling

| Topic | Configuration stance |
|-------|----------------------|
| Market currency | Bound to Market config (Lebanon → USD default) |
| Amount storage | Integer **minor units** (cents) in money/ledger domain — architecture approved |
| Gateway currency | Must align with Market + Areeba enablement — see Payment vendor checklist |
| Conversion / FX | Not a V1 hardcoded engine; multi-currency later via Market config |

### 1.2 Decimal rules

| Topic | Stance |
|-------|--------|
| USD decimal places | **2** (standard minor units) |
| Display rounding | Present major units with 2 decimals unless Admin/Market display config says otherwise |
| Policy amounts | Stored/configured consistently with minor-unit evaluation at runtime |

Exact display formatting nuances beyond 2-decimal USD: **Pending Business Decision** (if any non-standard display needed).

### 1.3 Display rules

| Topic | Stance |
|-------|--------|
| Customer / provider UI | Show currency code or symbol per locale (AR/EN) — **Pending Business Decision** on symbol vs code preference |
| Admin reports | Always show currency code + amount |
| RTL amounts | Locale-aware formatting; no hardcoded Latin-only money strings as sole path |

### 1.4 Future multi-currency readiness

| Topic | Stance |
|-------|--------|
| Architecture | Market-scoped currency + policy rows (ADR-001 / ADR-013) |
| V1 launch | Single Market Lebanon / USD |
| Additional markets/currencies | Configuration + new Market — not hardcoded Lebanon constants |

---

# 2. Commission Configuration

### 2.1 Configuration structure (Admin-configurable)

| Dimension | Purpose |
|-----------|---------|
| Provider type | Optional criterion (Craftsman / Store) — capability/ledger remain unified |
| Service category | Optional criterion |
| Effective date (`effective_from` / `effective_to`) | Versioned applicability |
| Commission percentage | Outcome option |
| Fixed commission option | Outcome option (minor units) |
| Override / priority rules | Higher-priority matching rule wins |
| Market | Lebanon for this pack |
| Active flag | Enable/disable without delete |

### 2.2 Initial values

| Parameter | Initial value |
|-----------|---------------|
| Default commission % | **Pending Business Decision** |
| Default fixed commission | **Pending Business Decision** |
| Provider-type overrides | **Pending Business Decision** |
| Category overrides | **Pending Business Decision** |
| Promo / plan exceptions | **Pending Business Decision** |
| Priority order | **Pending Business Decision** |

Finance sign-off: ☐ Name ______ Date ______

---

# 3. Subscription Configuration

### 3.1 Provider (Craftsman) subscriptions

| Element | Structure | Initial value |
|---------|-----------|---------------|
| Plans | Named plan codes + Market | **Pending Business Decision** |
| Duration | Period length / billing cycle | **Pending Business Decision** |
| Benefits | Visibility, featured slots, limits | **Pending Business Decision** |
| Status rules | Active / grace / expired / suspended behavior | **Pending Business Decision** |
| Pricing | Amount + currency (USD) | **Pending Business Decision** |

### 3.2 Store subscriptions

| Element | Structure | Initial value |
|---------|-----------|---------------|
| Plans | Admin-managed store plans | **Pending Business Decision** |
| Promotion limits | Featured / ad / catalog promo caps | **Pending Business Decision** |
| Visibility rules | Search ranking / listing visibility entitlements | **Pending Business Decision** |
| Catalog advertising entitlement | Allowed under plan (not e-commerce) | **Pending Business Decision** |
| Pricing | Amount + currency (USD) | **Pending Business Decision** |

Subscriptions are charged via approved payment architecture when money features enabled — values still **Pending Business Decision**.

---

# 4. Cancellation Rules

Configurable policy dimensions (no hardcoded windows/penalties):

| Policy area | Structure fields (examples) | Initial value |
|-------------|----------------------------|---------------|
| Customer cancellation | Allowed states, time windows, fees/% | **Pending Business Decision** |
| Provider cancellation | Allowed states, penalties, reputation hooks (policy-linked) | **Pending Business Decision** |
| Late cancellation | Threshold before start + outcomes | **Pending Business Decision** |
| No-show | Who declared, evidence, outcomes | **Pending Business Decision** |
| Approval requirements | Auto vs manual (Finance/Ops) | **Pending Business Decision** |
| Link to refund policy | Which cancel outcomes trigger which refund rule | **Pending Business Decision** |

---

# 5. Refund Rules

| Outcome type | Structure | Initial value |
|--------------|-----------|---------------|
| Full refund | Conditions / windows / actors | **Pending Business Decision** |
| Partial refund | % or fixed + conditions | **Pending Business Decision** |
| No refund | Conditions | **Pending Business Decision** |
| Manual approval | Role, SLA, escalation | **Pending Business Decision** |

Gateway refund capability (full/partial API) is separately tracked under Payment vendor checklist — **business refund rules** remain Admin-configurable here.

Ledger: refunds post reversing/refund entries (ADR-004) — architecture approved; amounts from policy evaluation.

---

# 6. Withdrawal Rules

| Element | Structure | Initial value |
|---------|-----------|---------------|
| Available methods | Config rows + adapter keys (not hardcoded single rail) | **Pending Business Decision** |
| Minimum withdrawal | Minor units + currency | **Pending Business Decision** |
| Maximum / velocity limits | Optional | **Pending Business Decision** |
| Approval flow | Auto threshold vs manual Finance approval | **Pending Business Decision** |
| Processing status | Requested → Under review → Approved → Paid / Rejected (illustrative) | **Pending Business Decision** on exact labels/SLAs |
| Eligibility | Provider capability `CanReceivePayments` + KYC/state gates | Architecture approved; commercial thresholds **Pending Business Decision** |

---

# 7. Settlement Rules

| Element | Structure | Initial value |
|---------|-----------|---------------|
| Settlement timing / cadence | Batch schedule meaning via config | **Pending Business Decision** |
| Holding period | After completion before release | **Pending Business Decision** |
| Reserve rules | % or fixed reserve | **Pending Business Decision** |
| Dispute impact | Hold release while dispute open (lightweight ADR-021) | **Pending Business Decision** |
| Release conditions | No open dispute, completion confirmed, etc. | **Pending Business Decision** |

Escrow-ready ledger holds remain architectural capability; commercial hold days are **Pending Business Decision**.

---

# 8. Admin Permissions

### 8.1 Finance Admin

| Capability | Confirm |
|------------|---------|
| Configure commission / cancellation / refund / withdrawal / settlement policies | ☑ Required (ADR-013) |
| Review transactions / payment status (no manual DB money edits) | ☑ Required |
| Approve withdrawals (per withdrawal config) | ☑ Required |
| MFA on Administration Portal | ☑ Required (ADR-006) |

### 8.2 Super Admin

| Capability | Confirm |
|------------|---------|
| Full control (including finance modules) | ☑ Required |
| MFA | ☑ Required |

### 8.3 Audit requirements

| Requirement | Confirm |
|-------------|---------|
| All finance policy changes audited | ☑ Required |
| Actions logged | ☑ Required |
| Change history preserved | ☑ Required |
| Financial/audit records protected under retention (ADR-022) | ☑ Required |

No mobile admin finance configuration (web only).

---

# 9. Hardcoding prohibition

| Check | Required |
|-------|----------|
| No commission % as domain constants | ☑ |
| No fixed cancel windows as sole production truth | ☑ |
| No fixed refund % as sole production truth | ☑ |
| No single permanent withdrawal method in domain | ☑ |
| No fixed settlement hold days as sole production truth | ☑ |
| Lebanon commercial terms loaded via Admin config / approved seed process only | ☑ |

---

# 10. Approval → BLOCKER-005 COMPLETED

BLOCKER-005 may become **COMPLETED** only when:

- [ ] Currency / display decisions acknowledged (or accepted defaults)  
- [ ] Commission initial values approved (not Pending)  
- [ ] Subscription plan values approved (provider + store)  
- [ ] Cancellation rules approved  
- [ ] Refund rules approved  
- [ ] Withdrawal rules approved  
- [ ] Settlement rules approved  
- [ ] Finance (+ Product as needed) signatures below  

Until then: keep **IN PREPARATION**.

### Sign-off

| Role | Name | Date | Decision |
|------|------|------|----------|
| Finance | | | ☐ Approve Lebanon initial config |
| Product Owner | | | ☐ Acknowledge commercial fit |
| Solution Architect | | | ☐ Acknowledge config-only (ADR-013/026) |

---

# 11. Notes

| Date | Note |
|------|------|
| 2026-07-24 | Structure prepared for Lebanon. All commercial numeric/plan values **Pending Business Decision**. No code or schema created. |
| | |

---

**End of Finance Lebanon Initial Configuration v1.0**
