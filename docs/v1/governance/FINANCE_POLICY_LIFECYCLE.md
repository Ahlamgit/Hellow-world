# KHADAMATI V1 — Finance Policy Lifecycle

**Document ID:** KHAD-V1-FINANCE-POLICY-LIFECYCLE  
**Version:** 1.0  
**Date:** 2026-07-24  
**Role:** Financial Governance Architect  
**BLOCKER-005 status:** **IN PREPARATION**  

**Sources:**  
[`../config/FINANCE_LEBANON_INITIAL_CONFIGURATION.md`](../config/FINANCE_LEBANON_INITIAL_CONFIGURATION.md) · [`FINANCE_POLICY_APPROVAL_MATRIX.md`](./FINANCE_POLICY_APPROVAL_MATRIX.md) · ADR-013 · ADR-026

```text
DO NOT write code.
DO NOT create database schema.
DO NOT implement finance modules.
DO NOT define commercial values.

Commercial values remain Pending Business Decision.
This document defines lifecycle governance only.
```

---

## Purpose

Define how KHADAMATI financial policies are **created, approved, changed, activated, and audited**.

Applies to Admin-configurable domains (ADR-013):

| Domain |
|--------|
| Commission |
| Cancellation |
| Refund |
| Withdrawal |
| Settlement |
| Related subscription commercial entitlements that affect money (where Admin-managed) |

| Item | State |
|------|-------|
| BLOCKER-005 | **IN PREPARATION** |
| Architecture | **APPROVED** (policy engines + Admin config) |
| Commercial values | **Pending Business Decision** |

Approvers per domain: see [`FINANCE_POLICY_APPROVAL_MATRIX.md`](./FINANCE_POLICY_APPROVAL_MATRIX.md).

---

# 1. Policy Versioning

Every financial policy (or policy rule set) **must** carry:

| Attribute | Required | Notes |
|-----------|----------|-------|
| Version number | Yes | Monotonic per policy family / Market (e.g. `v3`, or major.minor) |
| Effective date | Yes | `effective_from` mandatory; `effective_to` optional/open-ended |
| Created by | Yes | Admin user identity |
| Approved by | Yes | Role + identity per Approval Matrix |
| Approval timestamp | Yes | Immutable audit field |
| Change reason | Yes | Human-readable justification |

### Version rules

- New commercial terms → **new version** (do not silently overwrite prior version content).  
- Draft versions may exist before approval; only **approved + activated** versions bind runtime evaluation for new events.  
- Superseded versions remain readable in history.  
- No hardcoded rates/windows as production truth (ADR-013 / ADR-026).

---

# 2. Historical Protection

| Rule | Confirm |
|------|---------|
| Existing bookings retain the policy version active at **booking / payment / commission calculation time** (as applicable per event) | ☑ Required |
| New policy versions do **not** rewrite historical transactions | ☑ Required |
| Ledger entries remain immutable; corrections use reversing entries (ADR-004) | ☑ Required |
| Refunds / settlements after the fact evaluate using the **rules intended for that event** (version stamp or effective-dated snapshot) | ☑ Required |
| Reporting can show which policy version applied | ☑ Required |

**Intent:** Customers and providers are not retroactively re-priced by a later Admin change.

---

# 3. Change Workflow

```text
Request change
  → Business review
  → Finance review
  → Approval (per Approval Matrix; Legal where required)
  → Admin configuration update (draft version)
  → Activation date (effective_from)
  → Audit record
```

| Step | Owner | Output |
|------|-------|--------|
| Request change | Product / Business / Ops / Finance | Change reason + proposed structure |
| Business review | Business / Product | Fit with revenue & marketplace model |
| Finance review | Finance | Ledger, cash, risk impact |
| Approval | Per [`FINANCE_POLICY_APPROVAL_MATRIX.md`](./FINANCE_POLICY_APPROVAL_MATRIX.md) | Named approval + timestamp |
| Admin configuration update | Finance Admin / Super Admin (MFA) | Draft policy version in Admin Portal |
| Activation date | Finance Admin after approval | `effective_from` set; prior version closed if needed |
| Audit record | System | Who / what / when / before-after / reason |

Emergency changes: Super Admin + Finance only, with mandatory post-change audit and reason; still versioned — never silent DB edits.

---

# 4. Admin Controls

Administration Portal (web only — ADR-006) **must** support:

| Capability | Required |
|------------|----------|
| Create policy version | ☑ |
| Submit for approval (workflow or recorded approval gate) | ☑ |
| Activate policy (respecting effective date) | ☑ |
| View history | ☑ |
| Compare versions | ☑ |

| Forbidden | Confirm |
|-----------|---------|
| Direct database modification of financial policy as operating practice | ☑ Forbidden |
| Mobile admin finance configuration | ☑ Forbidden |
| Activating without authorization / MFA | ☑ Forbidden |

Implementation of these screens is **not** authorized by this document (Implementation Gate still **B**).

---

# 5. Multi-market Readiness

Policies **can vary by** (configuration dimensions — not hardcoded Lebanon-only logic):

| Dimension | Confirm |
|-----------|---------|
| Country / Market | ☑ |
| Currency | ☑ |
| Provider type | ☑ (optional criterion) |
| Service category | ☑ (optional criterion) |
| Subscription plan | ☑ (optional criterion / entitlement link) |

V1 launch Market: **Lebanon** · default currency **USD**. Additional markets later via Market + policy rows (ADR-001 / ADR-026).

---

# 6. Compliance

| Requirement | Confirm |
|-------------|---------|
| Financial changes are auditable | ☑ |
| Previous values / versions are preserved | ☑ |
| Unauthorized changes are prevented (RBAC + MFA + approval) | ☑ |
| Financial and audit records protected under retention (ADR-022) | ☑ |
| Effective dates mandatory on versions | ☑ |

---

# 7. Lifecycle states (logical)

| State | Meaning |
|-------|---------|
| Draft | Created; not approved |
| Pending approval | Submitted; awaiting approvers |
| Approved | Authorized; not yet effective (or waiting `effective_from`) |
| Active | Effective for new evaluations |
| Superseded | Replaced by newer version; retained for history / historical bookings |
| Retired / disabled | Explicitly inactive before natural supersession |

Exact UI labels may refine at implementation time without changing these semantics.

---

# 8. Relation to BLOCKER-005

| Artifact | Role |
|----------|------|
| [`../config/FINANCE_LEBANON_INITIAL_CONFIGURATION.md`](../config/FINANCE_LEBANON_INITIAL_CONFIGURATION.md) | What to configure (structure; values PBD) |
| [`FINANCE_POLICY_APPROVAL_MATRIX.md`](./FINANCE_POLICY_APPROVAL_MATRIX.md) | Who approves |
| **This document** | How policies version, change, activate, and audit |

BLOCKER-005 remains **IN PREPARATION** until actual financial values are approved and recorded.  
This lifecycle document alone does **not** complete BLOCKER-005.

---

**End of Finance Policy Lifecycle v1.0**
