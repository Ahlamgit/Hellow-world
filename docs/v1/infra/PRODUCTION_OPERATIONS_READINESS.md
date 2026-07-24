# KHADAMATI V1 — Production Operations Readiness

**Document ID:** KHAD-V1-PROD-OPS-READINESS  
**Version:** 1.1  
**Date:** 2026-07-24  
**Role:** Cloud Operations Architect  
**BLOCKER-004 status:** **IN PREPARATION**  

**Sources:**  
[`CLOUD_INFRASTRUCTURE_DECISION.md`](./CLOUD_INFRASTRUCTURE_DECISION.md) · [`CLOUD_SIZING_AND_COST_FRAMEWORK.md`](./CLOUD_SIZING_AND_COST_FRAMEWORK.md) · Master Prompt v1.0 · Final Architecture Decisions Complete · Scope Baseline · ADR-024 · ADR-012 · ADR-004 · ADR-006  

```text
DO NOT write production code.
DO NOT deploy infrastructure.
DO NOT create cloud resources.
DO NOT select a cloud provider.

KHADAMATI-specific operational requirements only.
Tooling products remain Pending Infrastructure Approval.
```

---

## Status Snapshot

| Item | State |
|------|-------|
| Cloud architecture | **IN PREPARATION** |
| Infrastructure | **NOT DEPLOYED** |
| Provider | **NOT SELECTED** |
| RPO / RTO | **Pending Infrastructure Approval** |
| BLOCKER-004 COMPLETED | ☐ No — requires provider, budget, RPO/RTO, DevOps approval |

---

## Purpose

Define operational requirements required before **KHADAMATI production launch**, covering:

| Surface / domain |
|------------------|
| Marketplace operations (service-first discovery, bookings) |
| Customer mobile app |
| Provider (Craftsman) mobile app |
| Store dashboard |
| Admin portal (web only) |
| Payment.js / Areeba IXOPAY payment flow |
| Ledger and settlement operations |
| Booking workflows |
| Booking-scoped chat |
| Notification workers |
| Identity verification documents |
| Multi-country / Market expansion readiness |

No implementation · No infrastructure creation · No provider selection.

---

# 1. Monitoring Strategy

Signals are mandatory; concrete products: **Pending Infrastructure Approval**.

## 1.1 Application Layer

| Monitor | KHADAMATI relevance |
|---------|---------------------|
| API availability | Customer, Craftsman, Store, Admin all share API |
| API response time | Search, booking, pay, chat paths |
| Error rates | 5xx / handled domain error spikes |
| Authentication failures | Customer/Provider/Store/Admin (Admin web + MFA) |
| Booking failures | Request → confirm → pay → complete transitions |
| Payment workflow failures | Intent, debit, REQUIRES_ACTION, finalize |

Also track Store catalog/ad inquiry endpoints separately from bookable services (no e-commerce).

## 1.2 Database Layer

| Monitor | Notes |
|---------|-------|
| Database availability | Primary health / failover events |
| Query performance | Slow queries on search, bookings, ledger |
| Connections | Saturation vs pool limits |
| Storage growth | OLTP + indexes + WAL |
| Transaction failures | Deadlocks / abort rates |

### Special attention — financial records

| Monitor | Intent |
|---------|--------|
| Ledger integrity | Unexpected post failures; reconcile job gaps |
| Payment transactions | Stuck Pending; capture/fail imbalance |
| Settlement records | Batch failures; hold/release anomalies |

## 1.3 Cache Layer (Redis)

| Monitor | Intent |
|---------|--------|
| Memory usage | Eviction risk |
| Connection health | Client errors |
| Cache failures | Error rate |
| Queue health | Depth, lag, consumer availability (ADR-012) |

## 1.4 Worker Layer

| Workload | Monitor |
|----------|---------|
| Notification jobs | Failures, lag, channel errors |
| Payment webhook processing | Auth fails, processing errors, lag |
| Retry queues | Exhausted retries / DLQ |
| Settlement jobs | Failures, overdue runs |
| Scheduled availability tasks | Missed schedules; calendar job health |

## 1.5 Payment Operations

| Monitor | Intent |
|---------|--------|
| Payment initialization failures | Intent create errors |
| Payment.js failures | Tokenize / WebView path errors (client signals + server) |
| IXOPAY webhook failures | Signature/auth/process failures |
| Duplicate event protection | Idempotent finalize collisions (safe no-ops vs bugs) |
| Failed transactions | Decline/error rates |
| Reconciliation issues | Pending past threshold; ledger vs gateway mismatch |

---

# 2. Alerting Strategy

## 2.1 Severity levels

| Severity | Meaning | Response expectation |
|----------|---------|----------------------|
| Critical | Outage, money/security risk, data integrity | Immediate on-call |
| Warning | Degradation trending to user/money impact | Same-day triage |
| Info (optional) | Notable but non-urgent | Business hours |

Exact SLAs: **Pending Infrastructure Approval**.

## 2.2 Critical alerts (examples)

| Alert | Owner (primary) | Escalation |
|-------|-----------------|------------|
| Application unavailable | DevOps / Eng on-call | Eng Lead → Product |
| Database unavailable | DevOps | Eng Lead → Architect |
| Payment processing outage | Eng (Payments) + DevOps | Finance + Product |
| Webhook processing failure | Eng (Payments) | DevOps → Finance |
| Ledger inconsistency | Eng + Finance liaison | Architect → Finance |
| Security incident | Security + DevOps | Product / Legal as needed |
| Data access violation | Security | Compliance / Legal |

## 2.3 Warning alerts (examples)

| Alert | Owner | Escalation |
|-------|-------|------------|
| High CPU/memory usage | DevOps | Eng if sustained |
| Slow APIs | Eng | DevOps capacity |
| Queue delays | Eng / DevOps | Payments if pay/notify queues |
| Storage growth | DevOps | Compliance if KYC/finance stores |
| Failed retries | Eng | Payments/Notifications owners |
| Increased booking failures | Eng / Ops | Product |

**Alert owner:** named on-call rotation — **Pending Infrastructure Approval** / Ops assignment.  
**Escalation path:** Primary → secondary on-call → Eng Lead → Product/Finance/Security by domain.  
**Response expectation:** per severity table above.

---

# 3. Logging Strategy

## 3.1 Application logs

| Include | Notes |
|---------|-------|
| API requests | Method, path, status, latency, correlation ID |
| Errors | Stack/safe message; no secrets |
| Authentication events | Success/fail by audience (customer/craftsman/store/admin) |

## 3.2 Audit logs (mandatory)

| Action class | Examples |
|--------------|----------|
| Admin actions | User restrict, verification decisions |
| Finance policy changes | Commission, cancel, refund, withdrawal, settlement (ADR-013) |
| Subscription changes | Plan create/update/assign |
| Commission changes | Rule version activate |
| Settlement actions | Batch run, hold/release overrides (if any) |

## 3.3 Payment logs

| Include | Constraint |
|---------|------------|
| Payment lifecycle events | Pending → Paid/Failed/… |
| Webhook events | Verified receipt metadata |
| Gateway responses | Safe codes/refs only |
| Reconciliation records | Gap/resolution notes |
| **Never** | PAN, CVV, full payment tokens |

## 3.4 Security logs

| Include |
|---------|
| Login attempts |
| MFA events (Admin) |
| Permission / role changes |
| Suspicious activities (rate abuse, webhook auth fail spikes) |

### Requirements

| Requirement | Confirm |
|-------------|---------|
| Retention policy | Align BLOCKER-006 / ADR-022 — numerics Pending |
| Access control | Least privilege; break-glass audited |
| Protection from modification | Append-only / immutable where offered |
| Auditability | Reconstruct who/what/when for finance & admin |

---

# 4. Backup Operations

### Required backups

| Asset | Notes |
|-------|-------|
| Database | PostgreSQL OLTP (bookings, ledger, users) |
| Financial records | Contained in DB; protected retention |
| Provider documents | Object storage |
| Identity verification files | Object storage; restricted |
| Application configuration | Secrets + non-secret config snapshots/export process |

### Operations

| Element | Requirement |
|---------|-------------|
| Backup frequency | DB: daily + PITR if available; object: versioning |
| Backup ownership | DevOps primary; Finance/Compliance informed for money/KYC restores |
| Backup encryption | At rest encryption required |
| Backup verification | Automated success + periodic integrity checks |
| Restore testing process | Documented drill before launch; recurring cadence Pending Approval |

| Metric | Value |
|--------|-------|
| **RPO** | **Pending Infrastructure Approval** |
| **RTO** | **Pending Infrastructure Approval** |

---

# 5. Deployment Operations

## 5.1 Environment promotion

```text
Development
  → Staging
  → Production
```

| Requirement | Confirm |
|-------------|---------|
| Environment isolation | ☑ Required |
| Configuration management | Per-env config; no prod values in Dev |
| Secrets management | Per-env secrets; never in git |

## 5.2 Release process

| Element | Requirement |
|---------|-------------|
| Versioning | Immutable artifact tags |
| Approval process | Staging validation; Production gated approval |
| Deployment window | Prefer low-traffic; avoid settlement/payment batch collisions when possible |
| Rollback strategy | Redeploy previous artifact |
| Emergency rollback | Sev-1 path; Eng Lead authorize; document |

## 5.3 CI/CD operations

| Element | Requirement |
|---------|-------------|
| Build validation | Compile/lint/unit gates |
| Automated testing gates | Required suites green before Staging/Prod promote |
| Deployment approval | Human gate for Production (and money-touching releases) |

Tooling: **Pending Infrastructure Approval**.

---

# 6. Security Operations

## 6.1 Access management

| Control | KHADAMATI note |
|---------|----------------|
| Role-based access | API RBAC + Admin permissions |
| Least privilege | Cloud IAM + app roles |
| Admin portal security | Web only; MFA (ADR-006) |
| MFA enforcement | Mandatory for Admin / Finance Admin |

## 6.2 Secrets management

| Element | Requirement |
|---------|-------------|
| Secret storage | Secrets manager (product Pending Approval) |
| Rotation process | Documented cadence (payment, DB, JWT signing, etc.) |
| Access restrictions | No broad human read of prod payment secrets |

## 6.3 Vulnerability management

| Practice | Requirement |
|----------|-------------|
| Dependency scanning | CI SCA |
| Security updates | Runtime/OS/image patches |
| Patch management | Cadence + emergency CVE path |

## 6.4 Compliance operations

| Data class | Ops consideration |
|------------|-------------------|
| User data | Access control; deletion/retention per ADR-022 |
| Provider documents | Restricted buckets; audit access |
| Identity verification | Least privilege; retention-aware purge jobs |
| Financial records | Never casually purged; legal hold capable |

Multi-country: Market-scoped config; residency/region choices later without rewriting ops model (ADR-001 / ADR-024).

---

# 7. Incident Management

## 7.1 Incident detection

| Source |
|--------|
| Monitoring |
| Alerts |
| Security events |
| Customer / provider reports (support) |
| Store / Admin operator reports |

## 7.2 Severity levels

```text
Critical
  → High
  → Medium
  → Low
```

| Level | Examples |
|-------|----------|
| Critical | Full API outage, DB down, payment outage, breach, ledger inconsistency |
| High | Major booking/pay degradation, webhook pipeline delayed, regional worker outage |
| Medium | Single feature impaired (e.g. notifications delayed), elevated errors |
| Low | Minor ops noise, non-user-facing |

## 7.3 Response process

```text
Detection
  → Assessment
  → Containment
  → Recovery
  → Post-incident review
```

## 7.4 Special incident scenarios

| Scenario | Containment / recovery focus |
|----------|------------------------------|
| Payment outage | Stop new captures if unsafe; preserve Pending; reconcile; Finance notify |
| Data breach | Isolate access; Security lead; Legal/Compliance; preserve logs |
| Database failure | Failover/restore per DR; verify ledger continuity |
| Notification failure | Queue backlog drain; avoid duplicate spam; Ops/Product comms |
| Provider verification failure | Pause auto-approvals if systemic; Admin manual path; storage/OCR vendor check |

---

# 8. Disaster Recovery Operations

| Capability | Requirement |
|------------|-------------|
| Service recovery | Redeploy API/portals/workers from known artifacts |
| Database restoration | Restore / promote per runbook; verify migrations |
| Storage restoration | Object version restore for KYC/media as needed |
| Payment recovery | Reconcile with IXOPAY; idempotent webhook replay; ledger repair via reversing entries only |
| Worker recovery | Redeploy consumers; drain backlog; clear poison with audit |

| Metric | Value |
|--------|-------|
| **RPO** | **Pending Infrastructure Approval** |
| **RTO** | **Pending Infrastructure Approval** |

Single primary region for Lebanon V1; expansion-ready (ADR-024).

---

# 9. Production Approval Checklist

| Area | Status |
|------|--------|
| Monitoring | Pending |
| Alerts | Pending |
| Logging | Pending |
| Backup testing | Pending |
| Restore testing | Pending |
| Deployment process | Pending |
| Security operations | Pending |
| Incident process | Pending |
| Disaster recovery | Pending |

### BLOCKER-004 overall completion still requires

| Gate | Status |
|------|--------|
| Cloud provider selected | Pending |
| Infrastructure budget approved | Pending |
| RPO approved | Pending |
| RTO approved | Pending |
| DevOps approval completed | Pending |

Until those complete: keep BLOCKER-004 = **IN PREPARATION**.

---

# 10. Relation to BLOCKER-004 pack

| Artifact | Role |
|----------|------|
| [`CLOUD_INFRASTRUCTURE_DECISION.md`](./CLOUD_INFRASTRUCTURE_DECISION.md) | What to host |
| [`CLOUD_SIZING_AND_COST_FRAMEWORK.md`](./CLOUD_SIZING_AND_COST_FRAMEWORK.md) | How to size/compare cost |
| **This document (v1.1)** | How to operate KHADAMATI in production |

This document alone does **not** complete BLOCKER-004.

---

# 11. Explicit Non-Goals

- No code · No deployment · No cloud resources · No provider selection  
- No invented RPO/RTO/SLO numbers  
- No application architecture changes  

---

**End of Production Operations Readiness v1.1**
