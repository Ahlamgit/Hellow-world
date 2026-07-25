# KHADAMATI — Stakeholder Approval Package

| Field | Value |
|-------|-------|
| **Document ID** | EVD-002-PKG-001 |
| **Blocker** | BLOCKER-002 — Stakeholder Approval |
| **Version** | 1.1 |
| **Status** | **READY FOR APPROVAL** (signatures pending) |
| **Gate** | B — NOT READY — CODING BLOCKED |
| **Prepared by** | Program Governance Manager |
| **Date** | 2026-07-25 |
| **Closure artifact (on completion)** | `STAKEHOLDER_APPROVAL_REGISTER_v1.0.md` |

---

## Authority and Purpose

This package is the formal stakeholder approval instrument required before KHADAMATI implementation may proceed toward Gate A. It is subordinate to:

- `FINAL_SCOPE_BASELINE.md`
- `MASTER_IMPLEMENTATION_PROMPT_v1.0.md`
- `FINAL_IMPLEMENTATION_GATE_REPORT.md`
- `BLOCKER_CLOSURE_EXECUTION_PLAN.md`
- `BLOCKER_EVIDENCE_MANAGEMENT_FRAMEWORK.md`
- ADR-001 through ADR-028

**Constraints:** No code, UI, architecture changes, database design changes, or deployment are authorized by this document. Verbal approval is not sufficient. Signatures in §6 are required to close BLOCKER-002.

---

## Instructions to Approvers

Each approver must:

1. Review all sections (§1–§5) against `FINAL_SCOPE_BASELINE.md` and frozen scope
2. Record **Approved**, **Approved with conditions**, or **Rejected** in §6
3. Sign and date the approval record
4. Return completed package to Program Governance Manager for archival under `docs/v1/governance/evidence/BLOCKER-002-stakeholder/`

Until §6 signatures exist, **BLOCKER-002 remains open** and implementation stays **blocked**.

---

## 1. Product Scope Approval

### 1.1 Applications and portals — approval requested

Stakeholders are requested to confirm approval of the following delivery surfaces as defined in frozen scope:

| Surface | Description | Approval requested |
|---------|-------------|-------------------|
| **Customer App** | Mobile application for customers to discover, book, and pay for services | ☐ Approved ☐ Rejected |
| **Craftsman App** | Mobile application for individual service providers (craftsmen) | ☐ Approved ☐ Rejected |
| **Store Dashboard** | Web dashboard for store-type providers | ☐ Approved ☐ Rejected |
| **Admin Web Portal** | Web portal for platform administration and configuration | ☐ Approved ☐ Rejected |

### 1.2 Service-first marketplace model — confirmation requested

Stakeholders confirm KHADAMATI operates as a **service-first marketplace** (not ecommerce-first):

| Principle | Confirmation |
|-----------|--------------|
| Primary unit of value is **service booking**, not product purchase | ☐ Confirmed |
| Listings represent **bookable services** (and store **advertised** catalog items without checkout) | ☐ Confirmed |
| Scope aligns with `FINAL_SCOPE_BASELINE.md` | ☐ Confirmed |

### 1.3 Customer discovery model — approval requested

Stakeholders confirm customers search and discover providers/services by:

| Discovery dimension | Included in V1 | Approval requested |
|---------------------|----------------|-------------------|
| **Service** | Yes | ☐ Approved |
| **Category** | Yes | ☐ Approved |
| **Location** | Yes | ☐ Approved |
| **Availability** | Yes | ☐ Approved |
| **Rating** | Yes | ☐ Approved |
| **Capability** | Yes | ☐ Approved |

### 1.4 Provider domain model — approval requested

Stakeholders confirm the approved provider hierarchy:

```
Provider
  → Type
    → Capabilities
      → Listings
        → Bookings
          → Payments
            → Ledger
```

| Model element | Role | Approval requested |
|---------------|------|-------------------|
| Provider | Legal/commercial entity offering services | ☐ Approved |
| Type | Craftsman vs Store (and approved typology) | ☐ Approved |
| Capabilities | Skills/services the provider can perform | ☐ Approved |
| Listings | Published service (and store advertisement) entries | ☐ Approved |
| Bookings | Customer service requests and lifecycle | ☐ Approved |
| Payments | Payment capture via approved payment approach | ☐ Approved |
| Ledger | Financial source of truth (per ADRs) | ☐ Approved |

**Approver attestation (§1):** ☐ Project Owner / Business Owner ☐ Administrator

---

## 2. Provider Model Approval

### 2.1 Craftsmen — approval requested

| Capability | Included | Approval requested |
|------------|----------|-------------------|
| Provide services | Yes | ☐ Approved |
| Receive bookings | Yes | ☐ Approved |
| Manage availability | Yes | ☐ Approved |
| Receive settlements | Yes | ☐ Approved |

### 2.2 Stores — approval requested

| Capability | Included in V1 | Approval requested |
|------------|----------------|-------------------|
| Provide services | Yes | ☐ Approved |
| Advertise services | Yes | ☐ Approved |
| Advertise product catalog items (display only) | Yes | ☐ Approved |
| **Product checkout** | **No — excluded** | ☐ Exclusion acknowledged |
| **Cart** | **No — excluded** | ☐ Exclusion acknowledged |
| **Inventory management** | **No — excluded** | ☐ Exclusion acknowledged |
| **Fulfilment workflow** | **No — excluded** | ☐ Exclusion acknowledged |

**Approver attestation (§2):** ☐ Project Owner / Business Owner ☐ Administrator

---

## 3. Booking Workflow Approval

Stakeholders are requested to approve the end-to-end booking workflow:

```
Customer request
        ↓
Provider confirmation
        ↓
Payment
        ↓
Service execution
        ↓
Completion acknowledgement
        ↓
Review
```

| Stage | Description | Approval requested |
|-------|-------------|-------------------|
| Customer request | Customer initiates booking against a listing | ☐ Approved |
| Provider confirmation | Provider accepts or declines per policy | ☐ Approved |
| Payment | Payment collected per approved payment model (§4) | ☐ Approved |
| Service execution | Provider delivers the booked service | ☐ Approved |
| Completion acknowledgement | Customer/provider confirms completion | ☐ Approved |
| Review | Post-service rating/review | ☐ Approved |

**Operations note:** Fulfilment, delivery, and product-shipping workflows are **out of scope** (see §5).

**Approver attestation (§3):** ☐ Project Owner / Business Owner ☐ Administrator

---

## 4. Payment and Financial Model Approval

Stakeholders confirm the following financial architecture principles (no implementation authorized by this section):

| Principle | Confirmation requested |
|-----------|-------------------------|
| **Payment.js / IXOPAY approach** | Payment processing via approved Payment.js / IXOPAY integration pattern per ADRs | ☐ Confirmed |
| **Ledger as financial source of truth** | All monetary events reconcile to the platform ledger | ☐ Confirmed |
| **Admin-configurable finance policies** | Commission, refund, settlement, withdrawal rules configured in Admin — not hardcoded | ☐ Confirmed |
| **No hardcoded commission/refund rules** | Engineering must not embed business financial rules in code | ☐ Confirmed |

**Cross-reference:** Detailed finance rules require separate closure of **BLOCKER-005** (`FINANCE_RULE_MATRIX_v1.0`). This section confirms stakeholder alignment with the **model**, not individual rate values.

**Approver attestation (§4):** ☐ Project Owner / Business Owner ☐ Administrator

---

## 5. Excluded Features Approval

Stakeholders are requested to **explicitly approve** the following **V1 exclusions** (not in scope):

| Excluded feature | Explicitly excluded from V1 | Acknowledgement |
|------------------|----------------------------|-----------------|
| Ecommerce checkout | Yes | ☐ Approved as excluded |
| Product ordering | Yes | ☐ Approved as excluded |
| Inventory | Yes | ☐ Approved as excluded |
| Delivery management | Yes | ☐ Approved as excluded |
| Open user messaging | Yes | ☐ Approved as excluded |
| Mobile admin application | Yes | ☐ Approved as excluded |

**Confirmation statements:**

| Statement | Acknowledgement |
|-----------|-----------------|
| Scope is **frozen** — excluded features require formal change control to add | ☐ Confirmed |
| Exclusions are **accepted** for V1 implementation planning | ☐ Confirmed |
| No implicit inclusion of excluded features via future phases without approval | ☐ Confirmed |

**Approver attestation (§5):** ☐ Project Owner / Business Owner ☐ Administrator

---

## 6. Approval Record

**BLOCKER-002 status:** **CLOSED** — 2026-07-25

**Corrected approval model (GOV-BLOCKER-002-ROLE-CORR-001):** Project Owner = Business Owner (unified). No separate mandatory Business Owner or Operations Owner signature.

| Role | Name | Decision | Date | Signature |
|------|------|----------|------|-----------|
| **Project Owner / Business Owner** | Project Owner | **Approved** | 2026-07-25 | Recorded |
| **Administrator** | Administrator | **Approved** | 2026-07-25 | Recorded |
| External Operations Stakeholder *(optional)* | | **N/A** | | |

### Decision values

- **Approved** — Section(s) reviewed; no material objections
- **Approved with conditions** — Approved subject to documented conditions in Comments
- **Rejected** — Package returned; BLOCKER-002 remains open

### Comments / conditions

| Role | Comments |
|------|----------|
| Project Owner / Business Owner | Scope baseline v1.0 accepted; Gate A transition authorized |
| Administrator | |
| External Operations Stakeholder | Not appointed |

---

## 7. Blocker Status

| Item | Status |
|------|--------|
| **BLOCKER-002** | **CLOSED** |
| **Evidence package** | `STAKEHOLDER_APPROVAL_PACKAGE.md` v1.1 |
| **Signatures** | **2 / 2 required** — complete |
| **BLOCKER-002 Closed** | **Yes** — 2026-07-25 |
| **Gate** | **B — NOT READY — CODING BLOCKED** |
| **Closed blockers** | **0 / 7** |
| **Implementation authorized** | **No** |

### Closure criteria (all required)

- [x] Project Owner / Business Owner — signed §6
- [x] Administrator — Governance Acceptance signed §6
- [x] Package archived in `evidence/BLOCKER-002-stakeholder/`
- [x] `BLOCKER_002_CLOSURE_RECORD.md` filed
- [x] `READINESS_BLOCKER_CLOSURE_STATUS.md` updated to **Closed**
- [x] Approval records filed per `BLOCKER_EVIDENCE_MANAGEMENT_FRAMEWORK.md`

---

## Document Control

| Version | Date | Author | Change |
|---------|------|--------|--------|
| 1.0 | 2026-07-25 | Program Governance Manager | Initial stakeholder approval package |
| 1.1 | 2026-07-25 | Program Governance Manager | Role ownership correction — PO/BO unified; Administrator required (GOV-BLOCKER-002-ROLE-CORR-001) |

**Distribution:** Project Owner / Business Owner, Administrator, Program Governance Manager
