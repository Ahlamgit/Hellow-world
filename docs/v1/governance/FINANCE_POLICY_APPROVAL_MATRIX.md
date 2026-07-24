# KHADAMATI V1 — Finance Policy Approval Matrix

**Document ID:** KHAD-V1-FINANCE-APPROVAL-MATRIX  
**Version:** 1.0  
**Date:** 2026-07-24  
**Role:** Financial Governance Architect  
**BLOCKER-005 status:** **IN PREPARATION**  

**Sources:**  
[`../config/FINANCE_LEBANON_INITIAL_CONFIGURATION.md`](../config/FINANCE_LEBANON_INITIAL_CONFIGURATION.md) · ADR-013 · ADR-026 · [`STAKEHOLDER_SIGN_OFF_PACKAGE.md`](./STAKEHOLDER_SIGN_OFF_PACKAGE.md) · [`FINANCE_POLICY_LIFECYCLE.md`](./FINANCE_POLICY_LIFECYCLE.md)

```text
DO NOT write code.
DO NOT implement finance modules.
DO NOT create database schema.
DO NOT define commercial values.

Commercial values remain: Pending Business Decision.
```

---

## Purpose

Define **ownership and approval responsibility** for all KHADAMATI financial policies before Lebanon initial configuration values are set in Admin.

| Item | State |
|------|-------|
| Finance configuration | **IN PREPARATION** |
| Architecture (policy engines) | **APPROVED** |
| Financial rules model | **Admin configurable** (ADR-013) |
| Commercial values | **Pending Business Decision** |
| BLOCKER-005 COMPLETED | ☐ Only after approved values exist |

**Runtime authority:** Administration Portal — Finance Admin / Super Admin (MFA, RBAC, audit, change history).  
**Market context:** Lebanon default · USD (ADR-001 / ADR-026).

---

## Summary Matrix

| Policy domain | Decision owner | Approval required from | Values |
|---------------|----------------|------------------------|--------|
| Commission | Business / Product | Business Owner · Finance | **Pending Business Decision** |
| Subscription (provider / store / promo packages) | Product · Business | Product · Business | **Pending Business Decision** |
| Cancellation | Operations · Business | Operations · Business | **Pending Business Decision** |
| Refund | Finance · Operations · Legal | Finance · Operations · Legal | **Pending Business Decision** |
| Withdrawal | Finance | Finance | **Pending Business Decision** |
| Settlement | Finance | Finance | **Pending Business Decision** |

Super Admin may execute portal changes only **after** the approvals above are recorded (or under explicit emergency procedure with post-facto audit).

---

# 1. Commission Policy

| Field | Value |
|-------|-------|
| **Decision owner** | Business / Product |
| **Approval required from** | Business Owner · Finance |
| **Admin module** | Commission Configuration (ADR-013) |
| **Values** | **Pending Business Decision** |

### Includes (structure only — no commercial numbers)

| Element | Ownership note |
|---------|----------------|
| Commission percentage | Business proposes · Finance approves |
| Category exceptions | Business / Product |
| Provider exceptions (type or segment) | Business / Product · Finance acknowledge |
| Effective dates | Mandatory on every rule version |
| Fixed commission option | Business · Finance |
| Priority / override rules | Business · Finance |

Configuration artifact: [`../config/FINANCE_LEBANON_INITIAL_CONFIGURATION.md`](../config/FINANCE_LEBANON_INITIAL_CONFIGURATION.md) §2

---

# 2. Subscription Policy

| Field | Value |
|-------|-------|
| **Decision owner** | Product · Business |
| **Approval required from** | Product · Business |
| **Admin module** | Subscription plans (craftsman + store) · promotion entitlements |
| **Values** | **Pending Business Decision** |

### Ownership coverage

| Package area | Ownership |
|--------------|-----------|
| Provider (craftsman) plans | Product · Business |
| Store plans | Product · Business |
| Promotion packages / limits / visibility entitlements | Product · Business (Finance if priced fees interact with ledger) |

Pricing amounts and plan benefits remain **Pending Business Decision**.

---

# 3. Cancellation Policy

| Field | Value |
|-------|-------|
| **Decision owner** | Operations · Business |
| **Approval required from** | Operations · Business |
| **Admin module** | Cancellation Policy Management |
| **Values** | **Pending Business Decision** |

### Ownership of

| Element | Owner |
|---------|-------|
| Cancellation windows | Operations · Business |
| Penalties | Business · Operations (Finance if money outcome) |
| Provider vs customer responsibility | Operations · Business |
| Linkage to refund outcomes | Operations · Finance (coordinate) |

---

# 4. Refund Policy

| Field | Value |
|-------|-------|
| **Decision owner** | Finance (primary) with Operations · Legal |
| **Approval required from** | Finance · Operations · Legal |
| **Admin module** | Refund Rules Management |
| **Values** | **Pending Business Decision** |

### Define ownership of

| Element | Owner |
|---------|-------|
| Refund eligibility | Finance · Operations · Legal |
| Approval requirements (auto vs manual) | Finance · Operations |
| Ledger treatment | Finance · Architecture acknowledge (ADR-004 — reversing/refund entries; no silent deletes) |
| Gateway capability vs business rule | Finance coordinates with Payment (BLOCKER-003/007); business rule still Admin-configurable |

---

# 5. Withdrawal Policy

| Field | Value |
|-------|-------|
| **Decision owner** | Finance |
| **Approval required from** | Finance |
| **Admin module** | Withdrawal Configuration |
| **Values** | **Pending Business Decision** |

### Define ownership of

| Element | Owner |
|---------|-------|
| Withdrawal methods (enabled rails) | Finance |
| Minimum amounts | Finance |
| Approval flow | Finance |
| Processing rules / SLAs | Finance |
| Adapter/provider technical enablement | Eng under Finance direction (ports/adapters — ADR-025) |

---

# 6. Settlement Policy

| Field | Value |
|-------|-------|
| **Decision owner** | Finance |
| **Approval required from** | Finance |
| **Admin module** | Settlement Configuration |
| **Values** | **Pending Business Decision** |

### Define ownership of

| Element | Owner |
|---------|-------|
| Settlement timing / cadence | Finance |
| Holding periods | Finance |
| Reserves | Finance |
| Dispute impact on release | Finance · Operations (dispute process ADR-021) |

---

# 7. Governance Rules

Confirm for all financial policy domains:

| Rule | Status |
|------|--------|
| No financial rule is hardcoded as production business truth | ☑ Required (ADR-013 / ADR-026) |
| All changes are audited | ☑ Required |
| Historical policies remain preserved (change history / versioning) | ☑ Required |
| Effective dates are mandatory | ☑ Required |
| Changes require authorization (RBAC + MFA on Admin web) | ☑ Required |
| Financial/audit records protected under retention policy | ☑ Required (ADR-022) |
| Commercial values not invented in engineering docs | ☑ Required — **Pending Business Decision** until owners approve |

### Change control (post-value approval)

1. Business justification  
2. Impact review (ledger, bookings, customer/provider UX, gateway)  
3. Approvals per this matrix  
4. Admin Portal change with audit trail  
5. No silent scope or rate changes in code  

---

# 8. Approval log (fill when values decided)

| Policy | Values status | Approvers (names) | Date | Decision |
|--------|---------------|-------------------|------|----------|
| Commission | Pending Business Decision | | | ☐ |
| Subscription | Pending Business Decision | | | ☐ |
| Cancellation | Pending Business Decision | | | ☐ |
| Refund | Pending Business Decision | | | ☐ |
| Withdrawal | Pending Business Decision | | | ☐ |
| Settlement | Pending Business Decision | | | ☐ |

---

# 9. Link to BLOCKER-005 completion

BLOCKER-005 remains **IN PREPARATION** until:

- [ ] This matrix acknowledged by Finance + Product  
- [ ] Commercial values approved per domain (not Pending)  
- [ ] Recorded in [`../config/FINANCE_LEBANON_INITIAL_CONFIGURATION.md`](../config/FINANCE_LEBANON_INITIAL_CONFIGURATION.md)  
- [ ] Finance sign-off on that configuration document  

This matrix alone does **not** complete BLOCKER-005.

---

**End of Finance Policy Approval Matrix v1.0**
