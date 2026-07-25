# KHADAMATI — Blocker Closure Execution Plan

| Field | Value |
|-------|-------|
| **Document ID** | GOV-BCEP-001 |
| **Version** | 1.1 |
| **Status** | Active |
| **Gate** | B — NOT READY (CODING BLOCKED) |
| **Target Gate** | A — READY FOR IMPLEMENTATION |
| **Product Scope** | FROZEN |
| **Architecture** | APPROVED (ADR-001 → ADR-028) |
| **Engineering Standards** | DEFINED |
| **Prepared by** | Program Governance Manager |
| **Date** | 2026-07-25 |

---

## Authority and Constraints

This plan is derived from and subordinate to:

- `MASTER_IMPLEMENTATION_PROMPT_v1.0.md`
- `FINAL_IMPLEMENTATION_GATE_REPORT.md`
- `FINAL_PRE_IMPLEMENTATION_READINESS_REVIEW.md`
- `IMPLEMENTATION_AUTHORIZATION_CHECKLIST.md`
- `BLOCKER_EVIDENCE_MANAGEMENT_FRAMEWORK.md`
- `READINESS_BLOCKER_CLOSURE_STATUS.md`
- `IMPLEMENTATION_READINESS_EXECUTION_PLAN.md`
- `FEATURE_TRACEABILITY_MATRIX.md`
- ADR-001 through ADR-028

**Explicit exclusions from this plan:**

- No production code
- No UI implementation
- No database schema creation
- No infrastructure deployment or provisioning
- No vendor selection or contracting
- No architecture changes
- No scope changes

---

## Objective

Move KHADAMATI from **Gate B: NOT READY — CODING BLOCKED** to **Gate A: READY FOR IMPLEMENTATION** by closing all seven implementation blockers with auditable evidence only.

**Evidence process:** All evidence must be collected, reviewed, approved, and archived per `BLOCKER_EVIDENCE_MANAGEMENT_FRAMEWORK.md` (GOV-BEMF-001) before any blocker may be marked **Closed**.

---

## 1. Current Gate Status

| Blocker | Description | Current Status | Required Closure Evidence | Owner |
|---------|-------------|----------------|---------------------------|-------|
| BLOCKER-001 | Design approval | **Open — Pending** | Signed design approval package (see §2.1) | Design Lead + Product Owner |
| BLOCKER-002 | Stakeholder approval | **Open — Pending** | Multi-function sign-off register (see §2.2) | Program Sponsor |
| BLOCKER-003 | Vendor readiness | **Open — Pending** | Vendor readiness dossier per integration (see §2.3) | Technical Lead / Integration Lead |
| BLOCKER-004 | Cloud readiness | **Open — Pending** | Cloud decision record + approvals (see §2.4) | Technical Architect + DevOps Lead |
| BLOCKER-005 | Finance configuration | **Open — Pending** | Approved finance rule matrix (see §2.5) | Finance + Business Operations |
| BLOCKER-006 | Compliance approval | **Open — Pending** | Compliance sign-off pack (see §2.6) | Legal / Compliance Officer |
| BLOCKER-007 | Payment.js validation | **Open — Pending** | Payment validation report (see §2.7) | Technical Lead + Finance Ops |

**Aggregate gate position:** **Gate B — NOT READY — CODING BLOCKED**

No implementation work (coding, schema, UI build, deployment) may proceed until Gate A is formally recorded in `FINAL_IMPLEMENTATION_GATE_REPORT.md`.

---

## 2. Blocker Closure Requirements

### BLOCKER-001 — Design

**Objective:** Obtain approved visual and UX specification sufficient to implement without design rework.

**Required evidence (all mandatory):**

| # | Evidence item | Acceptance criteria |
|---|---------------|---------------------|
| 1 | Final logo assets | Master logo, variants, clear-space rules; export formats documented |
| 2 | Approved colour palette | Primary, secondary, semantic (success/warning/error), accessibility contrast notes |
| 3 | Approved design tokens | Typography scale, spacing, radius, elevation, breakpoints — versioned token file or spec |
| 4 | Approved UI/UX specification | Interaction patterns, navigation model, error/empty/loading states, accessibility baseline |
| 5 | Customer app screens | Complete screen inventory with approved wireframes/high-fidelity for frozen scope |
| 6 | Craftsman app screens | Complete screen inventory with approved wireframes/high-fidelity for frozen scope |
| 7 | Store dashboard screens | Complete screen inventory with approved wireframes/high-fidelity for frozen scope |
| 8 | Admin portal screens | Complete screen inventory with approved wireframes/high-fidelity for frozen scope |

**Closure artifact:** `DESIGN_APPROVAL_SIGNOFF_v1.0` (signed by Design Lead + Product Owner)

**Evidence path:** `docs/v1/governance/evidence/BLOCKER-001-design/`

**Rule:** **No UI implementation until BLOCKER-001 is closed.**

**Traceability:** Map screens to `FEATURE_TRACEABILITY_MATRIX.md` feature IDs.

---

### BLOCKER-002 — Stakeholder Approval

**Objective:** Confirm frozen scope, approved workflows, and accepted exclusions across governing functions.

**Required approvals:**

| Function | Approver role | Must confirm |
|----------|---------------|--------------|
| Product | Product Owner | Scope frozen; feature set matches `MASTER_IMPLEMENTATION_PROMPT_v1.0.md` |
| Business | Business Sponsor | Commercial model and operating assumptions accepted |
| Operations | Operations Lead | Service fulfilment workflows approved |
| Technical architecture | Technical Architect | No conflict with ADR-001 → ADR-028 |

**Required confirmations (documented in sign-off):**

- Scope is **frozen** — no net-new features without change control
- End-to-end workflows are **approved** (customer, craftsman, store, admin)
- Documented **exclusions** are **accepted** by all signatories

**Closure artifact:** `STAKEHOLDER_APPROVAL_REGISTER_v1.0`

**Rule:** BLOCKER-002 may close only after written alignment; verbal approval is insufficient.

---

### BLOCKER-003 — Vendor Readiness

**Objective:** Confirm integration vendors are contractually and technically ready without altering ports/adapters architecture.

**Payment vendor — required evidence:**

| # | Evidence | Status |
|---|----------|--------|
| 1 | Vendor selected (record only — selection is prerequisite, not this plan's action) | Required |
| 2 | Contract available (executed or approved for execution) | Required |
| 3 | Sandbox access credentials issued to integration team | Required |
| 4 | Technical validation plan completed (connectivity, auth, test cases) | Required |

**Other vendors — required evidence per integration:**

| Integration | Evidence required |
|-------------|-------------------|
| SMS | Contract/access, sandbox or test sender, delivery receipt validation approach |
| Email | Contract/access, domain/DKIM/SPF readiness plan, template approval path |
| Maps | Contract/access, API key management approach, quota/billing alignment |
| OCR / Face | Contract/access, data handling alignment with compliance (BLOCKER-006) |
| Storage | Contract/access, bucket/container naming convention, encryption at rest decision |

**Architecture constraint (non-negotiable):**

- Ports/adapters pattern per approved architecture **remains unchanged**
- No direct vendor SDK coupling in domain or application core
- All integrations via defined adapter interfaces only

**Closure artifact:** `VENDOR_READINESS_DOSSIER_v1.0`

---

### BLOCKER-004 — Cloud Readiness

**Objective:** Record infrastructure decisions and obtain approvals **without creating cloud resources**.

**Required decisions and evidence:**

| # | Decision / approval | Evidence |
|---|---------------------|----------|
| 1 | Cloud provider | Written decision aligned with ADR cloud references |
| 2 | Budget approval | Finance-approved cost envelope by environment |
| 3 | Environments | Named environments (e.g. dev, staging, prod) and purpose |
| 4 | Backup strategy | Backup scope, frequency, retention alignment with compliance |
| 5 | RPO / RTO approval | Business-approved recovery objectives per tier |
| 6 | Security approval | Security review sign-off on architecture controls |

**Explicit prohibition:** **No cloud resources shall be created** as part of blocker closure. This phase is decision and approval only.

**Closure artifact:** `CLOUD_READINESS_DECISION_RECORD_v1.0`

---

### BLOCKER-005 — Finance Configuration

**Objective:** Obtain approved business rules for admin-configurable financial behaviour.

**Required business inputs (approved by Finance + Business Operations):**

| Domain | Required definition |
|--------|---------------------|
| Commission rules | Rates, tiers, applicability by service type |
| Subscription plans | Plan catalogue, billing cycle, entitlements |
| Withdrawal rules | Eligibility, limits, holds, approval workflow |
| Settlement rules | Cycle, cut-off, reconciliation approach |
| Cancellation rules | Customer/craftsman/store cancellation rights and fees |
| Refund rules | Full/partial, timing, dispute path |

**Constraints:**

- Rules **remain Admin configurable** in implementation
- **No hardcoding** of financial rules in application code
- Configuration model must align with approved architecture and ADRs

**Closure artifact:** `FINANCE_RULE_MATRIX_v1.0` (approved)

**Dependency note:** Required before settlement, commission, and payment settlement implementation sprints.

---

### BLOCKER-006 — Compliance

**Objective:** Obtain legal/compliance approval for data handling without legal assumptions by engineering.

**Required approvals:**

| Policy area | Approval required |
|-------------|-------------------|
| Retention periods | General platform data retention schedule |
| KYC retention | Identity document and verification data lifecycle |
| Financial record retention | Transaction, ledger, audit trail retention |
| Chat retention | Messaging data retention and export rules |
| Account deletion rules | Right to erasure, anonymisation vs deletion, exceptions |

**Rule:** Engineering **must not assume** retention or legal basis. All periods and rules must be **explicitly approved** in writing.

**Closure artifact:** `COMPLIANCE_APPROVAL_PACK_v1.0`

**Dependency note:** Required before KYC, chat, ledger, and account deletion implementation.

---

### BLOCKER-007 — Payment.js Validation

**Objective:** Complete technical and financial validation of Payment.js integration in sandbox before booking-payment implementation.

**Final validation checklist:**

| # | Check | Pass criteria |
|---|-------|---------------|
| 1 | Sandbox access | Successful authenticated connection to payment sandbox |
| 2 | Android validation | Payment.js flow completes on approved Android test matrix |
| 3 | iOS validation | Payment.js flow completes on approved iOS test matrix |
| 4 | Payment lifecycle | Authorize → capture (or equivalent) → receipt states verified |
| 5 | Webhook validation | Webhook signature, idempotency, replay handling verified |
| 6 | 3DS validation | 3DS challenge path tested for required card scenarios |
| 7 | Failure handling | Decline, timeout, cancel paths produce correct user and system state |
| 8 | Ledger verification | Payment events reconcile to ledger model per finance rules (BLOCKER-005) |

**Closure artifact:** `PAYMENT_JS_VALIDATION_REPORT_v1.0` (signed Technical Lead + Finance Ops)

**Dependency note:** Closes after BLOCKER-003 (payment vendor sandbox) and BLOCKER-005 (finance rules for ledger expectations).

---

## 3. Dependency Order

Recommended blocker closure sequence:

```
Phase 1 — Governance alignment (parallel where possible)
├── BLOCKER-002 Stakeholder approval
└── BLOCKER-006 Compliance approval

Phase 2 — Design and financial rules (parallel)
├── BLOCKER-001 Design approval          → blocks all UI implementation
└── BLOCKER-005 Finance configuration    → blocks settlement/payment rules implementation

Phase 3 — Platform decisions (parallel after Phase 1)
├── BLOCKER-004 Cloud readiness          → decisions only, no provisioning
└── BLOCKER-003 Vendor readiness         → contracts + sandbox access

Phase 4 — Payment validation (sequential)
└── BLOCKER-007 Payment.js validation    → requires BLOCKER-003 (payment) + BLOCKER-005 (ledger rules)
```

**Implementation sequencing rules (post Gate A):**

| Rule | Rationale |
|------|-----------|
| Design before UI development | BLOCKER-001 |
| Payment vendor + Payment.js validation before booking payment | BLOCKER-003, BLOCKER-007 |
| Finance rules before settlement implementation | BLOCKER-005 |
| Compliance before KYC, chat, deletion, financial retention | BLOCKER-006 |

---

## 4. Gate A Approval Checklist

| Requirement | Evidence | Approved By | Status |
|-------------|----------|-------------|--------|
| Architecture | ADR-001 → ADR-028; architecture baseline docs | Technical Architect | **Pending** |
| Scope | Frozen scope sign-off; `FEATURE_TRACEABILITY_MATRIX.md` | Product Owner | **Pending** |
| Design | `DESIGN_APPROVAL_SIGNOFF_v1.0` + assets | Design Lead + Product Owner | **Pending** |
| Vendors | `VENDOR_READINESS_DOSSIER_v1.0` | Technical Lead | **Pending** |
| Cloud | `CLOUD_READINESS_DECISION_RECORD_v1.0` | Technical Architect + Ops | **Pending** |
| Finance | `FINANCE_RULE_MATRIX_v1.0` | Finance | **Pending** |
| Compliance | `COMPLIANCE_APPROVAL_PACK_v1.0` | Legal / Compliance | **Pending** |
| Payment | `PAYMENT_JS_VALIDATION_REPORT_v1.0` | Technical Lead + Finance Ops | **Pending** |

**Gate A criteria:** All rows **Approved** with dated signatures and artifact references recorded in `FINAL_IMPLEMENTATION_GATE_REPORT.md`.

---

## 5. Implementation Authorization Rules

### Current authorization (Gate B)

| Activity | Authorized? |
|----------|-------------|
| Blocker closure and evidence collection | **Yes** |
| Sprint planning for implementation | **No** |
| Database implementation | **No** |
| API implementation | **No** |
| UI implementation | **No** |
| Testing of unapproved build artifacts | **No** |
| Infrastructure deployment | **No** |

### After Gate A — Allowed

- Sprint planning and backlog refinement against frozen scope
- Database implementation per approved schema ADRs
- API implementation per approved architecture
- UI implementation per approved design package (BLOCKER-001)
- Automated and manual testing per engineering standards

### After Gate A — Not Allowed (without change control)

- Unapproved scope additions
- Hardcoded financial rules (must remain Admin configurable)
- Vendor coupling outside ports/adapters
- Architecture changes without new ADR and approval

---

## 6. Execution Cadence and Reporting

| Activity | Frequency | Output |
|----------|-----------|--------|
| Blocker status review | Weekly | Update `READINESS_BLOCKER_CLOSURE_STATUS.md` |
| Evidence submission review | Per submission | Approval record per `BLOCKER_EVIDENCE_MANAGEMENT_FRAMEWORK.md` §4 |
| Steering checkpoint | Bi-weekly | Escalate stuck blockers |
| Gate readiness assessment | On any blocker closure | Update `FINAL_IMPLEMENTATION_GATE_REPORT.md` |
| Traceability check | Per blocker closure | Confirm evidence maps to gate checklist |

---

## 7. Final Recommendation

| Item | Determination |
|------|---------------|
| **Current Gate** | **B — NOT READY — CODING BLOCKED** |
| **Required action** | **Close blockers only** — collect and approve evidence per §2 |
| **Implementation authorization** | **Not granted** |
| **Next program milestone** | All seven blockers closed → Gate A review → update `FINAL_IMPLEMENTATION_GATE_REPORT.md` |

---

## Document Control

| Version | Date | Author | Change |
|---------|------|--------|--------|
| 1.0 | 2026-07-25 | Program Readiness Manager | Initial blocker closure execution plan |
| 1.1 | 2026-07-25 | Program Governance Manager | Linked evidence management framework (GOV-BEMF-001) |

**Related documents:** `BLOCKER_EVIDENCE_MANAGEMENT_FRAMEWORK.md`, `IMPLEMENTATION_AUTHORIZATION_CHECKLIST.md`
