# KHADAMATI — Blocker Evidence Management Framework

| Field | Value |
|-------|-------|
| **Document ID** | GOV-BEMF-001 |
| **Version** | 1.0 |
| **Status** | Active |
| **Gate** | B — NOT READY — CODING BLOCKED |
| **Closed blockers** | 0 / 7 |
| **Prepared by** | Program Governance Manager |
| **Date** | 2026-07-25 |

---

## Authority and Constraints

This framework is subordinate to:

- `MASTER_IMPLEMENTATION_PROMPT_v1.0.md`
- `FINAL_SCOPE_BASELINE.md`
- `FINAL_IMPLEMENTATION_GATE_REPORT.md`
- `READINESS_BLOCKER_CLOSURE_STATUS.md`
- `BLOCKER_CLOSURE_EXECUTION_PLAN.md`
- `IMPLEMENTATION_AUTHORIZATION_CHECKLIST.md`
- `FINAL_GATE_A_TRANSITION_AND_BLOCKER_CLOSURE_PACKAGE.md` (GOV-GATC-001)
- ADR-001 through ADR-032

**This framework does not authorize:**

- Production code
- UI implementation
- Database schema
- Infrastructure deployment
- Vendor selection
- Architecture changes
- Scope changes

---

## Objective

Define how KHADAMATI **collects, stores, reviews, approves, and tracks** evidence required to close BLOCKER-001 through BLOCKER-007.

### Evidence-to-authorization flow

```
Evidence received
        ↓
Evidence reviewed
        ↓
Evidence approved
        ↓
Blocker closed
        ↓
Gate A authorization
```

Implementation remains **blocked** until Gate A is recorded with all seven blockers **Closed** and evidence archived per this framework.

---

## 1. Evidence Governance Rules

### 1.1 Mandatory documentation

| Rule | Requirement |
|------|-------------|
| E-001 | Every blocker **must** have documented evidence before status may move to **Closed** |
| E-002 | **Verbal approval is not sufficient** — all approvals use `APPROVAL_RECORD_TEMPLATE.md` |
| E-003 | Evidence must be **versioned** and **traceable** to a blocker ID and evidence ID |
| E-004 | Evidence repository location is **`docs/v1/governance/evidence/`** (or approved linked store with index in-repo) |
| E-005 | `READINESS_BLOCKER_CLOSURE_STATUS.md` is updated **within one business day** of any status change |

### 1.2 Approval record requirements

Every approval **must** identify:

| Field | Required |
|-------|----------|
| Approver | Full name |
| Role | Authorized role per §3 |
| Date | ISO 8601 approval date |
| Version reviewed | Document/asset version (e.g. v1.0, 2026-07-25) |
| Decision | `Approved` \| `Approved with conditions` \| `Rejected` |
| Comments | Material conditions, caveats, or rejection rationale |

### 1.3 Blocker completion rule

**No blocker may be marked Completed (Closed) without:**

1. All required evidence items **received** and **archived**
2. All required approval records **Approved** or **Approved with conditions** (conditions tracked to resolution)
3. Closure artifact named in `BLOCKER_CLOSURE_EXECUTION_PLAN.md` on file
4. Program Governance Manager confirmation logged in `READINESS_BLOCKER_CLOSURE_STATUS.md`

### 1.4 Rejection and resubmission

- **Rejected** evidence returns to submitter with documented `Change Required`
- Blocker status reverts to **In Progress** until resubmission passes review
- Prior rejected versions remain archived (not deleted) for audit trail

---

## 2. Evidence Repository Structure

**Root path:** `docs/v1/governance/evidence/`

| Path | Purpose |
|------|---------|
| `evidence/_templates/` | Approval record templates and submission forms |
| `evidence/_index/` | Master evidence register (optional spreadsheet or markdown index) |
| `evidence/BLOCKER-001-design/` | Design and brand evidence |
| `evidence/BLOCKER-002-stakeholder/` | Stakeholder sign-offs |
| `evidence/BLOCKER-003-vendors/` | Vendor readiness evidence |
| `evidence/BLOCKER-004-cloud/` | Cloud readiness decisions |
| `evidence/BLOCKER-005-finance/` | Finance rule approvals |
| `evidence/BLOCKER-006-compliance/` | Compliance and privacy evidence |
| `evidence/BLOCKER-007-payment/` | Payment.js validation evidence |

### 2.1 BLOCKER-001 — `BLOCKER-001-design/`

| Subfolder / artifact | Content |
|----------------------|---------|
| `logo/` | Logo approval, master assets, usage rules |
| `brand/` | Brand approval, colour palette sign-off |
| `ui-ux/` | UI/UX specification approval |
| `design-system/` | Design tokens, component spec approval |
| `screens/customer-app/` | Approved customer app screen inventory |
| `screens/craftsman-app/` | Approved craftsman app screen inventory |
| `screens/store-dashboard/` | Approved store dashboard screens |
| `screens/admin-portal/` | Approved admin portal screens |
| `DESIGN_APPROVAL_SIGNOFF_v1.0.md` | Consolidated closure artifact |

### 2.2 BLOCKER-002 — `BLOCKER-002-stakeholder/`

| Subfolder / artifact | Content |
|----------------------|---------|
| `product/` | Product approval — scope frozen confirmation |
| `business/` | Business approval — commercial model |
| `operations/` | Operations approval — fulfilment workflows |
| `technical-architecture/` | Technical Architect — ADR alignment |
| `STAKEHOLDER_APPROVAL_REGISTER_v1.0.md` | Consolidated closure artifact |

### 2.3 BLOCKER-003 — `BLOCKER-003-vendors/`

| Subfolder / artifact | Content |
|----------------------|---------|
| `payment/` | Vendor evaluation record, contract reference, sandbox approval, technical validation |
| `sms/` | SMS vendor evidence |
| `email/` | Email vendor evidence |
| `maps/` | Maps vendor evidence |
| `ocr-face/` | OCR/Face vendor evidence |
| `storage/` | Storage vendor evidence |
| `VENDOR_READINESS_DOSSIER_v1.0.md` | Consolidated closure artifact |

**Constraint:** Evidence must confirm ports/adapters architecture **unchanged**.

### 2.4 BLOCKER-004 — `BLOCKER-004-cloud/`

| Subfolder / artifact | Content |
|----------------------|---------|
| `cloud-decision/` | Cloud provider decision record |
| `budget/` | Budget approval |
| `security/` | Security approval |
| `dr/` | DR / backup / RPO-RTO approval |
| `CLOUD_READINESS_DECISION_RECORD_v1.0.md` | Consolidated closure artifact |

**Rule:** No cloud resources created as part of evidence collection.

### 2.5 BLOCKER-005 — `BLOCKER-005-finance/`

| Subfolder / artifact | Content |
|----------------------|---------|
| `commission/` | Commission rules approval |
| `subscription/` | Subscription plans approval |
| `settlement/` | Settlement rules approval |
| `withdrawal/` | Withdrawal rules approval |
| `cancellation/` | Cancellation rules approval |
| `refund/` | Refund rules approval |
| `FINANCE_RULE_MATRIX_v1.0.md` | Consolidated closure artifact |

**Constraint:** Rules remain **Admin configurable** — no hardcoding evidence may imply fixed code values.

### 2.6 BLOCKER-006 — `BLOCKER-006-compliance/`

| Subfolder / artifact | Content |
|----------------------|---------|
| `retention/` | Retention periods approval |
| `kyc/` | KYC retention approval |
| `financial-records/` | Financial record retention approval |
| `chat/` | Chat retention approval |
| `deletion/` | Account deletion / privacy approval |
| `COMPLIANCE_APPROVAL_PACK_v1.0.md` | Consolidated closure artifact |

### 2.7 BLOCKER-007 — `BLOCKER-007-payment/`

| Subfolder / artifact | Content |
|----------------------|---------|
| `sandbox/` | Sandbox access evidence |
| `payment-js/` | Payment.js validation results |
| `webhooks/` | Webhook validation evidence |
| `3ds/` | 3DS validation evidence |
| `lifecycle/` | Payment lifecycle test evidence |
| `ledger/` | Ledger verification evidence |
| `finance-signoff/` | Finance approval of validation |
| `PAYMENT_JS_VALIDATION_REPORT_v1.0.md` | Consolidated closure artifact |

---

## 3. Blocker Evidence Requirements

| Blocker | Required Evidence | Approval Owner | Status |
|---------|-------------------|----------------|--------|
| BLOCKER-001 | Logo assets; colour palette; design tokens; UI/UX spec; customer, craftsman, store, admin screen approvals; `DESIGN_APPROVAL_SIGNOFF_v1.0` | Design Lead + Product Owner | **Pending** |
| BLOCKER-002 | Product, business, operations, technical architecture approvals; scope frozen / workflows / exclusions confirmed; `STAKEHOLDER_APPROVAL_REGISTER_v1.0` | Program Sponsor (coordinates); per-function approvers | **Pending** |
| BLOCKER-003 | Per-vendor: evaluation, contract reference, sandbox access, technical validation (payment, SMS, email, maps, OCR/face, storage); ports/adapters unchanged attestation; `VENDOR_READINESS_DOSSIER_v1.0` | Technical Lead / Integration Lead | **Pending** |
| BLOCKER-004 | Cloud decision, budget, environments definition, backup/DR, RPO/RTO, security approval; `CLOUD_READINESS_DECISION_RECORD_v1.0` | Technical Architect + DevOps Lead | **Pending** |
| BLOCKER-005 | Commission, subscription, settlement, withdrawal, cancellation, refund rule approvals; `FINANCE_RULE_MATRIX_v1.0` | Finance + Business Operations | **Pending** |
| BLOCKER-006 | Retention (general, KYC, financial, chat), deletion/privacy approvals; `COMPLIANCE_APPROVAL_PACK_v1.0` | Legal / Compliance Officer | **Pending** |
| BLOCKER-007 | Sandbox access; Android/iOS Payment.js validation; lifecycle, webhooks, 3DS, failure handling, ledger verification; `PAYMENT_JS_VALIDATION_REPORT_v1.0` | Technical Lead + Finance Ops | **Pending** |

**Aggregate:** 0 / 7 blockers closed · Implementation **blocked**

---

## 4. Approval Record Template

Use for **every** evidence item. Store completed records under the relevant blocker folder and reference in `_index/`.

**Template file:** `evidence/_templates/APPROVAL_RECORD_TEMPLATE.md`

---

### Approval Record

| Field | Value |
|-------|-------|
| **Evidence ID** | `EVD-{BLOCKER}-{SEQ}` e.g. `EVD-001-003` |
| **Blocker** | BLOCKER-00X |
| **Document / Asset** | |
| **Version** | |
| **Submitted By** | Name · Role · Date |
| **Reviewed By** | Name · Role · Date |
| **Decision** | `Approved` \| `Approved with conditions` \| `Rejected` |
| **Approval Date** | YYYY-MM-DD |
| **Comments** | |
| **Change Required** | (if Rejected or conditional) |

**Signatures / attestation:** Electronic approval via steering register or signed PDF reference path: `evidence/.../approvals/EVD-XXX.pdf`

---

## 5. Evidence Validation Process

### 5.1 Workflow

```
Submission
    ↓
Technical / Business Review
    ↓
Approval or Rejection
    ↓
Evidence Archived
    ↓
Blocker Status Updated
```

### 5.2 Stage definitions

| Stage | Actor | Actions | Output |
|-------|-------|---------|--------|
| **Submission** | Evidence owner (per blocker) | Place artifacts in repository path; complete approval record draft; notify Program Governance Manager | Evidence ID assigned; status → **In Progress** or **Under Review** |
| **Technical / Business Review** | Designated reviewer(s) per §3 | Validate completeness, version, alignment with frozen scope and ADRs; no legal/finance assumptions by reviewers outside role | Review notes on approval record |
| **Approval or Rejection** | Approval owner per §3 | Record decision with all mandatory fields (§1.2) | **Approved** / **Approved with conditions** / **Rejected** |
| **Evidence Archived** | Program Governance Manager | Immutable archive; index updated; link closure artifact | Archived path + checksum optional |
| **Blocker Status Updated** | Program Governance Manager | Update `READINESS_BLOCKER_CLOSURE_STATUS.md`; if all items complete → **Closed** | Blocker row updated |

### 5.3 Review SLAs (recommended)

| Priority | Review target |
|----------|---------------|
| Standard evidence | 5 business days |
| Gate-critical (BLOCKER-002, BLOCKER-006) | 3 business days |
| Resubmission after rejection | 3 business days |

Escalation path: Program Governance Manager → Program Sponsor.

---

## 6. Gate A Readiness Review

### 6.1 Entry criteria

- All **7** blockers status = **Closed**
- All closure artifacts on file per §2
- `IMPLEMENTATION_AUTHORIZATION_CHECKLIST.md` complete
- `FINAL_IMPLEMENTATION_GATE_REPORT.md` draft updated for Gate A

### 6.2 Required attendees

| Attendee | Role |
|----------|------|
| Product Owner | Product |
| Business Owner | Business |
| Technical Architect | Architecture |
| Security / Compliance representative | Security & compliance |

**Chair:** Program Governance Manager or Program Sponsor

### 6.3 Review agenda

| # | Topic | Pass criteria |
|---|-------|---------------|
| 1 | Blocker evidence | All seven blockers closed with archived evidence |
| 2 | Architecture | No unresolved issues vs ADR-001 → ADR-028 |
| 3 | Scope | No unresolved changes vs `FINAL_SCOPE_BASELINE.md` |
| 4 | Authorization checklist | `IMPLEMENTATION_AUTHORIZATION_CHECKLIST.md` all items satisfied |
| 5 | Implementation rules | Team acknowledges post–Gate A constraints (§7) |

### 6.4 Outcomes

| Outcome | Action |
|---------|--------|
| **Gate A granted** | Update `FINAL_IMPLEMENTATION_GATE_REPORT.md` to **READY FOR IMPLEMENTATION**; date and attendees recorded |
| **Gate A deferred** | Document gaps; blockers reopened if evidence insufficient |
| **Conditional Gate A** | **Not permitted** — all blockers must be fully closed |

---

## 7. Change Control

After evidence approval:

| Baseline type | Rule |
|---------------|------|
| Approved evidence documents | Become **readiness baseline**; changes require new version + re-approval |
| Design / UX baselines | Change → Design Lead + Product Owner re-approval |
| Architecture | Change → **new ADR** + Technical Architect approval |
| Scope | Change → Product approval + change control; **frozen scope default** |
| Financial rules | Remain **Admin configurable**; matrix updates require Finance re-approval, not code hardcoding |

Evidence supersession: retain prior versions in `evidence/.../archive/` subfolders.

---

## 8. Roles and Responsibilities

| Role | Responsibility |
|------|----------------|
| Program Governance Manager | Framework owner; evidence index; blocker status integrity; Gate A pack |
| Blocker owners | Submit complete evidence per §3 |
| Approval owners | Review and sign approval records |
| Program Sponsor | Escalation; BLOCKER-002 coordination |
| Technical Architect | Architecture compliance reviews |

---

## 9. Final Recommendation

| Item | Determination |
|------|---------------|
| **Current gate** | **B — NOT READY — CODING BLOCKED** |
| **Closed blockers** | **0 / 7** |
| **Required action** | Collect and validate evidence per this framework only |
| **Implementation authorization** | **Not granted** |

---

## Document Control

| Version | Date | Author | Change |
|---------|------|--------|--------|
| 1.0 | 2026-07-25 | Program Governance Manager | Initial evidence management framework |

**Related documents:** `BLOCKER_CLOSURE_EXECUTION_PLAN.md`, `READINESS_BLOCKER_CLOSURE_STATUS.md`, `IMPLEMENTATION_AUTHORIZATION_CHECKLIST.md`
