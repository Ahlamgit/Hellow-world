# BLOCKER-005 — Subscription Pricing Approval

| Field | Value |
|-------|-------|
| **Document ID** | EVD-005-SUBSCRIPTION-001 |
| **Version** | 1.1 |
| **Date** | 2026-07-25 |
| **Blocker** | BLOCKER-005 — Finance |
| **Decision** | **APPROVED** |
| **Approver** | Project Owner / Business Owner |
| **Business approval** | **COMPLETE** |
| **Blocker closure** | **NOT CLOSED** — technical attestation pending |

---

## 1. Provider subscription plans — APPROVED

| Tier | Business decision |
|------|-------------------|
| **Basic** | **Approved** |
| **Pro** | **Approved** |
| **Premium** | **Approved** |

---

## 2. Subscription management (Administrator controls)

| Setting | Administrator control |
|---------|----------------------|
| Price | **Configurable** |
| Duration | **Configurable** |
| Included features | **Configurable** |
| Limits | **Configurable** |
| Activation | **Configurable** |

**No hardcoded plan prices in application code.**

---

## 3. Advertisement management (Administrator controls)

| Setting | Administrator control |
|---------|----------------------|
| Advertisement packages | **Configurable** |
| Pricing | **Configurable** |
| Duration | **Configurable** |
| Featured listing rules | **Configurable** |
| Activation | **Configurable** |

---

## 4. Financial governance rule

| Role | May configure | May NOT |
|------|---------------|---------|
| **Administrator** | Operational commission, subscription, and ad values via platform configuration | Change business strategy without PO approval · modify architecture · bypass governance |
| **Project Owner / Business Owner** | Business model and revenue model approval | — |

See: `FINANCE_RULE_MATRIX.md` · `COMMISSION_MODEL_APPROVAL.md`
