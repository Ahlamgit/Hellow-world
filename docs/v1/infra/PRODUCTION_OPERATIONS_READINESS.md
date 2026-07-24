# KHADAMATI V1 — Production Operations Readiness

**Document ID:** KHAD-V1-PROD-OPS-READINESS  
**Version:** 1.0  
**Date:** 2026-07-24  
**Role:** Cloud Operations Architect  
**BLOCKER-004 status:** **IN PREPARATION**  

**Sources:**  
[`CLOUD_INFRASTRUCTURE_DECISION.md`](./CLOUD_INFRASTRUCTURE_DECISION.md) · [`CLOUD_SIZING_AND_COST_FRAMEWORK.md`](./CLOUD_SIZING_AND_COST_FRAMEWORK.md) · Master Prompt v1.0 · ADR-024  

```text
DO NOT write production code.
DO NOT deploy infrastructure.
DO NOT create cloud resources.
DO NOT select a cloud provider.

Operational requirements only — tooling products remain
Pending Infrastructure Approval until provider is chosen.
```

---

## Status Snapshot

| Item | State |
|------|-------|
| Cloud architecture | **IN PREPARATION** |
| Infrastructure | **NOT DEPLOYED** |
| Provider | **NOT SELECTED** |
| RPO / RTO | **Pending Infrastructure Approval** |
| BLOCKER-004 COMPLETED | ☐ No — requires provider, budget, RPO/RTO, and approvals |

---

## Purpose

Define **operational requirements** required before production launch, independent of a specific cloud brand.

---

# 1. Monitoring Strategy

Tooling/products: **Pending Infrastructure Approval** (portable signals below are mandatory).

### 1.1 Application

| Signal | Requirement |
|--------|-------------|
| API availability | Health/readiness probes; uptime measurement per environment |
| Response time | Latency percentiles (e.g. p50/p95/p99) on critical routes |
| Errors | 5xx rate, exception rate, dependency failure rate |

### 1.2 Database

| Signal | Requirement |
|--------|-------------|
| Performance | Query latency / slow query visibility |
| Connections | Active/idle vs max; saturation alerts |
| Storage | Used % / free space / growth trend |

### 1.3 Infrastructure

| Signal | Requirement |
|--------|-------------|
| CPU | Per API/worker node or service |
| Memory | Utilization + OOM risk |
| Network | LB errors, egress anomalies, saturation |

### 1.4 Workers

| Signal | Requirement |
|--------|-------------|
| Queue failures | Consumer errors, DLQ depth |
| Retry failures | Exhausted retries; poison messages |
| Lag | Outbox / queue age vs SLO (target Pending Approval) |

### 1.5 Payments

| Signal | Requirement |
|--------|-------------|
| Payment failures | Debit decline/error rates; adapter timeouts |
| Webhook failures | Auth failures, processing errors, lag to finalize |
| Reconcile gaps | Pending payments past threshold |

---

# 2. Alerting Strategy

### 2.1 Critical

| Alert | Intent |
|-------|--------|
| Service unavailable | API/portal down or failing health checks |
| Database failure | Primary unreachable / failover event |
| Payment outage | Gateway/adapter systemic failure or webhook pipeline down |
| Security incident | Suspected breach, mass auth abuse, webhook signature bypass attempts |

**Response:** Immediate page/on-call; incident process §7 (Severity 1).

### 2.2 Warning

| Alert | Intent |
|-------|--------|
| High resource usage | CPU/memory/connection thresholds sustained |
| Queue delays | Worker lag above warning threshold |
| Storage growth | DB/object storage approaching capacity |

**Response:** Ticket + same-day triage; escalate if trending critical.

Threshold numerics: **Pending Infrastructure Approval** (set with sizing model).

---

# 3. Logging Strategy

| Log class | Content (examples) | Notes |
|-----------|-------------------|-------|
| Application logs | Request path, correlation ID, errors (no secrets) | Structured JSON preferred |
| Audit logs | Admin/finance policy changes, privileged actions | Immutable append; protected retention |
| Payment logs | Payment id, gateway refs, safe status codes — **never** PAN/CVV/full tokens | Align Payment.js validation security |
| Security logs | Auth failures, MFA events, webhook auth failures | Restricted access |

### Requirements

| Requirement | Confirm |
|-------------|---------|
| Retention | Per Compliance/Admin policy — **Pending** numeric defaults (BLOCKER-006) |
| Access control | Least privilege; break-glass audited |
| Protection from modification | Append-only / WORM-style where offered; no silent delete of audit/finance logs |
| Correlation | Request/payment/booking IDs across services |

---

# 4. Backup Operations

| Element | Requirement |
|---------|-------------|
| Backup frequency | DB: daily full + continuous PITR if available; object: versioning (Cloud Decision) |
| Backup verification | Automated success checks + periodic integrity validation |
| Restore testing | **Required** before production launch; recurring cadence **Pending Infrastructure Approval** |
| Backup ownership | DevOps primary; Finance/Compliance informed for financial/audit data restores |

| Metric | Value |
|--------|-------|
| **RPO** | **Pending Infrastructure Approval** |
| **RTO** | **Pending Infrastructure Approval** |

Redis: ephemeral — rebuild from DB; not a backup source of truth.

---

# 5. Deployment Operations

| Element | Requirement |
|---------|-------------|
| CI/CD ownership | DevOps / Engineering Lead (process); tooling Pending Approval |
| Release process | Gated Production releases; changelog; migration plan |
| Rollback process | Redeploy previous immutable artifact; DB forward-fix with care |
| Version management | Tagged artifacts; environment promotion record |
| Environment promotion | Dev → Staging → Production only (no hot-patch prod from laptops as norm) |

```text
Development
  → Staging
  → Production
```

| Gate | Rule |
|------|------|
| Staging | Required before Production for API/worker/payment-touching changes |
| Production | Explicit approval; migrate job controlled; feature flags for money where applicable |

---

# 6. Security Operations

| Practice | Requirement |
|----------|-------------|
| Access management | IAM least privilege; MFA for cloud console + Admin Portal (ADR-006) |
| Secret rotation | Documented cadence; no secrets in git/images |
| Vulnerability updates | OS/image/runtime patching cadence |
| Dependency monitoring | SCA on application dependencies in CI |
| Audit review | Periodic review of privileged cloud + Admin finance actions |

Cloud security product names: **Pending Infrastructure Approval**.

---

# 7. Incident Management

### 7.1 Detection

Monitoring + alerting (§1–§2); on-call roster **Pending Infrastructure Approval** / Ops assignment.

### 7.2 Severity levels

| Severity | Examples | Response expectation |
|----------|----------|----------------------|
| Sev-1 Critical | Full outage, DB down, payment outage, security incident | Immediate |
| Sev-2 High | Major degradation, payment partial failure, significant queue backlog | Urgent (hours) |
| Sev-3 Medium | Limited feature impact, elevated errors | Next business day |
| Sev-4 Low | Minor / cosmetic ops issues | Planned |

Exact SLAs: **Pending Infrastructure Approval**.

### 7.3 Response process

1. Detect / declare severity  
2. Assemble responders (Eng + DevOps; Finance if money; Security if Sev-1 security)  
3. Mitigate (rollback, failover, disable flag)  
4. Communicate status (internal; customer comms per Ops)  

### 7.4 Recovery process

Restore service per runbooks; verify payments/ledger consistency after payment incidents; confirm backup restore if data loss.

### 7.5 Post-incident review

Blameless review; action items; update runbooks/alerts; no silent scope changes.

---

# 8. Production Approval Checklist

| Area | Status |
|------|--------|
| Monitoring | Pending |
| Alerts | Pending |
| Logging | Pending |
| Backup testing | Pending |
| Deployment process | Pending |
| Security operations | Pending |
| Incident process | Pending |

Also required for BLOCKER-004 overall completion (Cloud Decision / Sizing):

| Area | Status |
|------|--------|
| Cloud provider | Pending |
| Infrastructure budget | Pending |
| RPO | Pending |
| RTO | Pending |
| DevOps / Business approval | Pending |

---

# 9. Relation to BLOCKER-004

| Artifact | Role |
|----------|------|
| [`CLOUD_INFRASTRUCTURE_DECISION.md`](./CLOUD_INFRASTRUCTURE_DECISION.md) | What to host |
| [`CLOUD_SIZING_AND_COST_FRAMEWORK.md`](./CLOUD_SIZING_AND_COST_FRAMEWORK.md) | How to size/compare cost |
| **This document** | How to operate production |

BLOCKER-004 remains **IN PREPARATION** until cloud provider, budget, RPO/RTO, and approvals are completed.  
This operations document alone does **not** complete BLOCKER-004.

---

# 10. Explicit Non-Goals

- No deployment or resource creation  
- No provider selection  
- No application code  
- No invented SLO/RPO/RTO numbers  

---

**End of Production Operations Readiness v1.0**
