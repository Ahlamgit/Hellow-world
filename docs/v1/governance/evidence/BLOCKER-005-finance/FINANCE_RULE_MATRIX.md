# BLOCKER-005 — Finance Rule Matrix (Governance Framework)

| Field | Value |
|-------|-------|
| **Document ID** | EVD-005-MATRIX-001 |
| **Version** | 1.0 |
| **Date** | 2026-07-25 |
| **Blocker** | BLOCKER-005 — Finance |
| **Type** | **Governance framework** — not a values table |
| **Business approval** | **COMPLETE** — 2026-07-25 |
| **Blocker closure** | **NOT CLOSED** |

```text
This matrix defines WHO controls WHAT — not launch dollar/percent values.
Operational values are Administrator-configured after implementation (ADR-013).
```

---

## 1. Commission rules

| Rule domain | Business model | Value control | Hardcode allowed? |
|-------------|----------------|---------------|-------------------|
| Marketplace commission | **Approved** | Administrator | **No** |
| Service-category commission | **Approved** | Administrator | **No** |
| Provider/store commission | **Approved** | Administrator | **No** |
| Commission activation / effective dates | **Approved** | Administrator | **No** |
| Settlement calculation | Architecture-approved flow | System + Administrator config | **No** |

---

## 2. Subscription rules

| Plan / product | Tiers approved | Value control | Hardcode allowed? |
|----------------|----------------|---------------|-------------------|
| Provider Basic | **Yes** | Administrator | **No** |
| Provider Pro | **Yes** | Administrator | **No** |
| Provider Premium | **Yes** | Administrator | **No** |
| Store/service advertising packages | **Yes** | Administrator | **No** |
| Featured placement | **Yes** | Administrator | **No** |

---

## 3. Excluded (V1)

| Feature | Status |
|---------|--------|
| Customer wallet | **Excluded** |
| Provider wallet UI | **Excluded** |
| Instant withdrawal | **Excluded** |
| Manual financial adjustment screens | **Excluded** |

---

## 4. Required approvals for blocker closure

| Approver | Responsibility | Status |
|----------|----------------|--------|
| Project Owner / Business Owner | Business model + admin configuration governance | **Approved** — 2026-07-25 |
| Technical Architect | Configurable-rules architecture alignment | **Pending** |
| Finance Owner (optional attestation) | Operational finance process alignment | **Pending** |

**Closure artifact note:** If `FINANCE_RULE_MATRIX_v1.0` is required as signed closure artifact, this document is the governance baseline; launch values are **not** pre-filled.

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | Administrator configuration governance model; business approval complete |
