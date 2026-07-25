# KHADAMATI — Final Governance, Gate A Readiness & Implementation Control

| Field | Value |
|-------|-------|
| **Document ID** | GOV-MASTER-CTRL-001 |
| **Apex authorization prompt** | GOV-MASTER-IMPL-AUTH-001 v1.1 (`MASTER_GOVERNANCE_CONTROLLED_IMPLEMENTATION_AUTHORIZATION_PROMPT.md`) |
| **Version** | 2.0 |
| **Evidence completion** | GOV-GATE-B-EVIDENCE-001 |
| **Date** | 2026-07-25 |
| **Owner** | Program Governance Manager · Technical Program Manager |
| **Gate** | **B — NOT READY — CODING BLOCKED** |
| **Blockers closed** | **1 / 7** |
| **Implementation** | **NOT AUTHORIZED** |
| **Governance decision** | **REMAIN GATE B — CODING BLOCKED** |

```text
MASTER GOVERNANCE CONTROL DOCUMENT
Governance · validation · evidence · authorization management ONLY.
No code · schema · APIs · UI · infrastructure · vendors · scope changes.
```

---

## 1. Role and mandate

The Program Governance Manager and Technical Program Manager prepare KHADAMATI V1 for **controlled implementation** under frozen scope and validated architecture.

| Authorized at Gate B | Prohibited until Gate A |
|----------------------|-------------------------|
| Evidence collection and validation | Production code |
| Approval workflows and signature tracking | Database schema and migrations |
| Blocker closure execution | APIs, UI, payment, booking implementation |
| Gate A preparation and ceremony planning | Infrastructure deployment |
| Governance documentation updates | Vendor selection |
| Sprint 0 **planning** (GOV-S0FC-001) | Architecture or ADR changes |
| | Frozen scope expansion |

---

## 2. Program context

### Product

**KHADAMATI V1** — marketplace and service booking platform.

| Actor | Surface |
|-------|---------|
| Customer | Mobile App |
| Craftsman | Mobile App |
| Store | Dashboard |
| Platform operations | Admin Portal |

### Current program status

| Item | Status |
|------|--------|
| **Current gate** | **Gate B — NOT READY — CODING BLOCKED** |
| **Target gate** | Gate A — Ready for Implementation |
| **Architecture** | **APPROVED & VALIDATED** |
| **Scope** | **FROZEN** |
| **Implementation** | **NOT AUTHORIZED** |
| **Blockers closed** | **1 / 7** |
| **Sprint 0** | **NOT STARTED** |
| **Coding** | **BLOCKED** |

---

## 3. Source of truth

Always align with:

| Document | ID / path |
|----------|-----------|
| Master implementation brief | `MASTER_IMPLEMENTATION_PROMPT_v1.0.md` |
| Architecture decisions | `FINAL_ARCHITECTURE_DECISIONS_COMPLETE.md` · `adr/` (ADR-001…032) |
| Architecture validation | `FINAL_ARCHITECTURE_CONSISTENCY_REVIEW.md` (KHAD-V1-FACR-001) |
| Feature traceability | `FEATURE_TRACEABILITY_MATRIX.md` |
| Payment governance | `payment/PAYMENT_SECURITY_AND_RECONCILIATION_FRAMEWORK.md` |
| Evidence framework | `BLOCKER_EVIDENCE_MANAGEMENT_FRAMEWORK.md` (GOV-BEMF-001) |
| Gate A authorization | `GATE_A_IMPLEMENTATION_AUTHORIZATION_RECORD.md` (GOV-GAIR-001) |
| Authorization checklist | `IMPLEMENTATION_AUTHORIZATION_CHECKLIST.md` (GOV-IACL-001) |
| Business approval consolidation | `BUSINESS_APPROVAL_CONSOLIDATION_RECORD.md` (GOV-BUSINESS-APPROVAL-001) |
| Independent audit | `FINAL_GATE_A_READINESS_AUDIT_REPORT.md` (GOV-GA-AUDIT-001) |
| Scope baseline | `FINAL_SCOPE_BASELINE.md` *(restore to repo if missing)* |

---

## 4. Architecture status (governance summary)

Architecture is **complete and validated**. This section records governance-relevant constraints only — **no ADR or architecture changes** in this document.

### Provider and scope model

| Included | Excluded (V1) |
|----------|---------------|
| Craftsmen, service providers, stores as service/catalog advertisers | Ecommerce checkout |
| Unified marketplace model | Product cart |
| Service-first booking | Inventory management |
| | Delivery fulfilment |

### Booking lifecycle (service domain)

```text
Request → Provider Confirmation → Payment Request → Payment Success
       → Service Execution → Completion → Review
```

Booking owns **service lifecycle only**.

### Payment domain separation (ADR-029, ADR-030, ADR-031)

| Domain | Owns | Must not own |
|--------|------|--------------|
| **Booking** | Requested, Confirmed, Awaiting Payment, Paid, In Progress, Completed, Cancelled | Ledger, commission, settlement |
| **Payment** | Pending, Processing, Paid, Failed, Cancelled, Refunded | Service completion state |
| **Ledger** | Financial truth, immutable records, reconciliation | UI payment state |
| **Settlement** | Provider payout lifecycle | Booking lifecycle |

**Rules:**

- Payment success ≠ service completion  
- Service completion ≠ settlement completion  
- Ledger records are **immutable** (reversals only)  
- Customer sees: service, provider, amount, confirmation, status — **not** ledger, commission, settlement, gateway events  

**Integration direction:** Application → Payment Adapter Port → IXOPAY Payment.js → Gateway. No direct vendor coupling in business modules.

**Failure handling:** ADR-031 — failed payment, delayed confirmation, duplicate webhook, ledger failure recovery.

### Notification architecture (ADR-032)

```text
Business Event → Notification Event → Orchestrator → In-App / Push / SMS / Email
```

Business modules do not call notification vendors directly. Notification failure does not rollback business actions.

### Chat (ADR-020)

V1: **booking-scoped chat only**. No public, marketplace, or open social messaging.

---

## 5. Governance control plane

### Core instruments

| # | Document | ID | Purpose |
|---|----------|-----|---------|
| 1 | `FINAL_GATE_A_READINESS_AUDIT_REPORT.md` | GOV-GA-AUDIT-001 | Independent Gate A assessment — **remain Gate B** |
| 2 | `FINAL_GATE_A_TRANSITION_AND_BLOCKER_CLOSURE_PACKAGE.md` | GOV-GATC-001 | Blockers → Gate A execution roadmap |
| 3 | `BLOCKER_CLOSURE_EXECUTION_TRACKER.md` | GOV-BLOCKER-TRACKER-001 | Operational blocker tracking |
| 4 | `BLOCKER_EVIDENCE_MANAGEMENT_FRAMEWORK.md` | GOV-BEMF-001 | Evidence and approval rules |
| 5 | `GATE_A_IMPLEMENTATION_AUTHORIZATION_RECORD.md` | GOV-GAIR-001 | Gate A authorization — **Draft, unsigned** |
| 6 | `IMPLEMENTATION_AUTHORIZATION_CHECKLIST.md` | GOV-IACL-001 | Gate transition checklist |
| 7 | `FINAL_IMPLEMENTATION_GATE_REPORT.md` | GOV-FIGR-001 | Current gate declaration |
| 8 | `READINESS_BLOCKER_CLOSURE_STATUS.md` | GOV-RBCS-001 | Official blocker register |

### Phase 1 execution suite

| Document | ID |
|----------|-----|
| `PHASE_1_BLOCKER_APPROVAL_READINESS_REPORT.md` | GOV-P1-READINESS-001 |
| `PHASE_1_GOVERNANCE_EXECUTION_ROADMAP_STATUS.md` | GOV-P1-ROADMAP-001 |
| `GATE_A_CEREMONY_PREPARATION_PACKAGE.md` | GOV-GA-CEREMONY-001 |
| `ARCHITECTURE_COMPLIANCE_AND_IMPLEMENTATION_READINESS_REGISTER.md` | GOV-ARCH-READINESS-001 |
| `PHASE_1_CLOSURE_EXECUTION_REPORT.md` | GOV-P1-CLOSURE-001 |
| `PHASE_1_APPROVAL_FINALIZATION_REPORT.md` | GOV-P1-FINAL-001 |
| `PHASE_1_APPROVAL_EXECUTION_PACK.md` | GOV-P1-EXEC-001 |
| `PHASE_1_APPROVAL_TRACKING_REGISTER.md` | GOV-P1-TRACK-001 |

### Evidence repository

`docs/v1/governance/evidence/` — `BLOCKER-001-design/` … `BLOCKER-007-payment/`  
Each folder: `README.md` · `APPROVAL_RECORD.md` · `EVIDENCE_CHECKLIST.md`

---

## 6. Blocker dashboard

| ID | Name | Status | Owner | Evidence |
|----|------|--------|-------|----------|
| **BLOCKER-001** | Design | **Evidence Prepared** — Awaiting Design Lead | Design Lead + PO/BO | `DESIGN_APPROVAL_RECORD.md` |
| **BLOCKER-002** | Stakeholder | **Closed** | Project Owner / Business Owner | `BLOCKER_002_CLOSURE_RECORD.md` |
| **BLOCKER-003** | Vendors | **Evidence Prepared** — Awaiting TA | Integration Lead + TA | `VENDOR_APPROVAL_MATRIX.md` |
| **BLOCKER-004** | Cloud | **Evidence Prepared** — Pending technical validation | Architect + DevOps | `CLOUD_READINESS_RECORD.md` |
| **BLOCKER-005** | Finance | **Evidence Prepared** — Awaiting Finance | Finance + PO/BO | `COMMISSION_MODEL_APPROVAL.md` |
| **BLOCKER-006** | Compliance | **Evidence Prepared** — Awaiting Legal + TA | Legal + TA | `COMPLIANCE_GOVERNANCE_RECORD.md` |
| **BLOCKER-007** | Payment.js | **Evidence Prepared** — Awaiting technical validation | PO/BO + TA | `PAYMENT_FLOW_APPROVAL_RECORD.md` |

**Progress:** **1 / 7 CLOSED** · **6 / 7 Evidence Prepared** (human/technical approval pending)

**Report:** `GATE_B_EVIDENCE_COMPLETION_REPORT.md` (GOV-GATE-B-EVIDENCE-001)

---

## 7. Evidence and closure rules

A blocker is **CLOSED** only when:

```text
Evidence Submitted → Reviewed → Approved → Signed → Archived → Tracker Updated → Blocker Closed
```

| Rule | Reference |
|------|-----------|
| Verbal approval is **invalid** | GOV-BEMF-001 E-002 |
| Tracker update within **1 business day** | GOV-BEMF-001 E-005 |
| `APPROVAL_RECORD.md` Decision = **Approved** | Per-blocker evidence folder |
| `EVIDENCE_CHECKLIST.md` complete | Per-blocker evidence folder |
| Closure artifact filed | Per GOV-RBCS-001 register |

**Live tracking:** GOV-P1-TRACK-001 (Phase 1: BLOCKER-002 **closed**; BLOCKER-006 PO/BO direction ✓) · GOV-BUSINESS-APPROVAL-001

---

## 8. Closure sequence

| Phase | Close | Status |
|-------|-------|--------|
| **Phase 1** | BLOCKER-002, BLOCKER-006 | **In progress** — 002 closed; 006 open |
| **Phase 2** | BLOCKER-001, BLOCKER-005 | PO/BO business approved — Design Lead + Finance pending |
| **Phase 3** | BLOCKER-003, BLOCKER-004 | Open |
| **Phase 4** | BLOCKER-007 | Open (depends on 003, 005) |
| **Ceremony** | Gate A | **Gated** on 7/7 |

---

## 9. Gate A requirements

Before implementation authorization, **all** must be true:

| # | Requirement | Status |
|---|-------------|--------|
| 1 | **7 / 7 blockers CLOSED** | **Not met** (1/7) |
| 2 | Architecture **APPROVED** | **Met** |
| 3 | Scope **FROZEN** + stakeholder evidence | **Met** — BLOCKER-002 Closed |
| 4 | **Traceability complete** (FTM — V1 specified) | **Met** (specified); scope baseline file pending |
| 5 | GOV-GAIR-001 **signed** (§8) | **Not met** |
| 6 | GOV-IACL-001 **complete** | **Not met** |
| 7 | GOV-FIGR-001 amended to Gate A | **Not met** |
| 8 | Steering review (GOV-BEMF-001 §6) | **Not held** |

---

## 10. Post–Gate A implementation controls

### Sprint 0 (after Gate A only — GOV-S0FC-001)

| Wave | Allowed (after Gate A) | Blocker / gate |
|------|------------------------|----------------|
| **Wave 1** | Repository setup · CI/CD · backend skeleton · project structure · dev environments | Gate A |
| **Wave 2** | Authentication · RBAC · security baseline · audit/logging · localization | Gate A |
| **Wave 3** | Migration framework · database tooling · schema migration mechanism — **no business tables** | Gate A |
| **Wave 4** | Design system · UI shells · navigation foundation | Gate A **and** BLOCKER-001 **Closed** |

**Sprint 0 exclusions (never before Gate A):** Payment · booking completion · settlement · wallets · withdrawals · production deployment · business database schema · feature expansion

| Migration rule | Detail |
|----------------|--------|
| **Allowed** | Migration framework / tooling setup |
| **Not allowed** | Business domain schema migrations until BLOCKER-006 Closed + domain design authorized |

| Also not allowed in Sprint 0 | Reference |
|------------------------------|-----------|
| Customer/provider wallet UI | Finance governance |
| Manual financial adjustments | ADR-013 |
| Gateway split payment · instant withdrawal | Scope / finance exclusions |

---

## 10.1 Design governance (BLOCKER-001)

| Before BLOCKER-001 Closed | After BLOCKER-001 Closed (and Gate A) |
|---------------------------|--------------------------------------|
| No Flutter UI implementation | Design system allowed |
| No React UI implementation | App shells allowed |
| No design system code | UI foundations allowed |
| Governance packages only | ADR-023 satisfied |

---

## 10.2 Finance governance (BLOCKER-005)

Customer experience remains simple (ADR-029). Internal: ledger, commission, settlement, reporting.

**V1 excludes (governance):** customer wallet · provider wallet UI · manual financial adjustments · gateway split payment · instant withdrawal

---

## 10.3 Compliance governance (BLOCKER-006)

**Data classification categories (approval pending):** Personal · Identity/KYC · Financial · Operational · Audit

**Retention periods:** **PENDING LEGAL / COMPLIANCE APPROVAL** — never invent durations.

**Access control:** RBAC · least privilege · audit logging · Admin web only · MFA required · no mobile admin app.

---

## 10.4 Conditional implementation gates

| Activity | Requires |
|----------|----------|
| **UI implementation** | Gate A **and** BLOCKER-001 **Closed** (ADR-023) |
| **Business domain schema** | Gate A **and** BLOCKER-006 **Closed** |
| **Booking payment** | Gate A **and** BLOCKER-007 **Closed** |
| **Vendor integrations** | Gate A **and** BLOCKER-003 **Closed** |

---

## 11. Final governance decision

| Decision | Status |
|----------|--------|
| **REMAIN GATE B** | **ACTIVE** |
| **CODING BLOCKED** | **ACTIVE** |
| **Do not start implementation** | **CONFIRMED** |

**Execute only:** evidence collection · approval workflow · blocker closure · Gate A preparation.

---

## 12. Standard execution response format

Every governance execution response **must** include (in order):

| # | Field | Current value |
|---|-------|---------------|
| 1 | **Current Gate** | Gate B — NOT READY — CODING BLOCKED |
| 2 | **Implementation Status** | NOT AUTHORIZED |
| 3 | **Blockers** | Closed **1 / 7** |
| 4 | **Architecture Impact** | None — APPROVED & VALIDATED; no ADR changes |
| 5 | **Scope Impact** | None — FROZEN; no feature additions |
| 6 | **Executed Action** | *(governance action completed)* |
| 7 | **Files Created/Updated** | *(paths or None)* |
| 8 | **Next Allowed Action** | Phase 1 human approvals; no coding |

**Canonical prompts:** GOV-MASTER-IMPL-AUTH-001 (apex) · GOV-CONTROL-PROMPT-001 · GOV-READINESS-PROMPT-001 · GOV-GA-IMPL-AUTH-PROMPT-001

No exceptions. No shortcuts. No coding before Gate A. **Documentation ≠ approval.**

---

## Document history

| Version | Date | Change |
|---------|------|--------|
| 1.0 | 2026-07-25 | Initial master governance, Gate A readiness, and implementation control document |
| 1.1 | 2026-07-25 | Gate A ceremony prep + Phase 1 roadmap status references |
| 1.2 | 2026-07-25 | Traceability Gate A requirement; GOV-ARCH-READINESS-001 reference |
| 1.3 | 2026-07-25 | Sprint 0 waves; design/finance/compliance governance; GOV-CONTROL-PROMPT-001 response format |
| 1.4 | 2026-07-25 | Sprint 0 migration framework vs business schema; payment ADR refs; doc≠approval rule |
| 1.5 | 2026-07-25 | Sprint 0 wave alignment (GOV-READINESS-PROMPT-001); Gate A checklist |
| 1.6 | 2026-07-25 | Sprint 0 waves per GOV-GA-IMPL-AUTH-PROMPT-001; authorization sequence |
| 1.7 | 2026-07-25 | Apex prompt GOV-MASTER-IMPL-AUTH-001; provider availability calendar rule |
| 1.8 | 2026-07-25 | Apex prompt v1.1 — expanded role, Sprint 0 waves, blocker dashboard, agent behaviour |
| 1.9 | 2026-07-25 | GOV-BUSINESS-APPROVAL-001 — PO/BO business approvals for 001, 005, 006, 007; 1/7 closed |
| 2.0 | 2026-07-25 | GOV-GATE-B-EVIDENCE-001 — Gate B evidence completion package |

**Supersedes:** Informal status summaries for gate and implementation authorization decisions.  
**Subordinate to:** `MASTER_IMPLEMENTATION_PROMPT_v1.0.md`, ADRs, `FINAL_SCOPE_BASELINE.md`.
