# BLOCKER-005 — Commission Model Approval

| Field | Value |
|-------|-------|
| **Document ID** | EVD-005-COMMISSION-001 |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |
| **Blocker** | BLOCKER-005 — Finance |
| **Evidence status** | **Prepared** — awaiting Finance attestation + matrix values |
| **Blocker closure** | **NOT CLOSED** |

---

## 1. Marketplace commission concept — APPROVED

| Decision | Status | Approver |
|----------|--------|----------|
| Marketplace commission model accepted | **Approved** | Project Owner / Business Owner |
| Commission applies on completed marketplace transactions | **Approved** | Project Owner / Business Owner |

---

## 2. Commission flow (architecture-aligned)

```text
Customer Payment
        ↓
Payment Domain
        ↓
Ledger (immutable source of truth)
        ↓
KHADAMATI Commission
        ↓
Provider Settlement Process
```

| Rule | Status |
|------|--------|
| Payment and settlement separated | Confirmed (ADR-030) |
| No manual financial manipulation | Confirmed |
| Ledger immutable | Confirmed (ADR-004) |

---

## 3. Configurable commission

| Requirement | Status |
|-------------|--------|
| Commission percentage **configurable** | **Approved** — administrator configuration |
| No hardcoded commission values in application code | **Required** (ADR-013) |
| Settlement calculation ownership | Finance operations + platform configuration governance |

---

## 4. Commission matrix (values)

| Item | Status | Owner |
|------|--------|-------|
| `FINANCE_RULE_MATRIX_v1.0` — commission values | **Pending** | Finance Owner |
| Finance attestation on values | **Pending** | Finance Owner |
| Technical Architect financial architecture alignment | **Pending** | Technical Architect |

**Incorrect:** Stating "commission approved" without matrix and Finance attestation.  
**Correct:** Business model approved; **values pending** Finance evidence.

---

## 5. Related documents

| Document | Purpose |
|----------|---------|
| `SUBSCRIPTION_PRICING_APPROVAL.md` | Subscription tiers |
| `BUSINESS_APPROVAL_RECORD.md` | PO/BO business consolidation |
| `APPROVAL_RECORD.md` | Full blocker approval (partial) |

**No payment implementation. No settlement code.**
