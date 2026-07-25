# BLOCKER-005 — Commission Model Approval

| Field | Value |
|-------|-------|
| **Document ID** | EVD-005-COMMISSION-001 |
| **Version** | 1.1 |
| **Date** | 2026-07-25 |
| **Blocker** | BLOCKER-005 — Finance |
| **Decision** | **APPROVED** |
| **Approver** | Project Owner / Business Owner |
| **Business approval** | **COMPLETE** |
| **Blocker closure** | **NOT CLOSED** — technical attestation pending |

---

## 1. Marketplace commission model — APPROVED

The approved marketplace commission model remains:

```text
Customer Payment → Payment Domain → Ledger → KHADAMATI Commission → Provider Settlement
```

| Rule | Status |
|------|--------|
| Ledger immutable source of truth | Confirmed (ADR-004) |
| Payment/settlement separation | Confirmed (ADR-030) |
| No manual financial bypass | Confirmed |

---

## 2. Governance change — Administrator configuration

**Final operational financial values are controlled by Administrator configuration.**

| Constraint | Rule |
|------------|------|
| Hardcoded financial values | **FORBIDDEN** (ADR-013) |
| Commission percentages in code | **FORBIDDEN** |
| Launch values | Set via **Administrator** after implementation — not in application source |

---

## 3. Commission management (Administrator controls)

| Setting | Administrator control |
|---------|----------------------|
| Commission percentage | **Configurable** |
| Commission rules | **Configurable** |
| Service-category commission | **Configurable** |
| Provider / store commission | **Configurable** |
| Activation status | **Configurable** |
| Effective dates | **Configurable** |

**Administrator cannot:** change business strategy without Project Owner approval · modify architecture · bypass governance approval.

---

## 4. Related documents

| Document | Purpose |
|----------|---------|
| `FINANCE_RULE_MATRIX.md` | Admin configuration governance framework |
| `SUBSCRIPTION_PRICING_APPROVAL.md` | Subscription + advertising configuration |
| `BUSINESS_APPROVAL_RECORD.md` | PO/BO consolidation |

**No payment implementation. No settlement code.**
